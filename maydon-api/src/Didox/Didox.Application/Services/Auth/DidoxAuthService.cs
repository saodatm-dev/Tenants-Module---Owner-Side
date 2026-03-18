// CachingExtensions.ShortDuration is in Core.Infrastructure — we inline its value here
using Core.Domain.Results;
using Core.Application.Resources;
using Core.Application.Abstractions.Security;
using Core.Application.Abstractions.Authentication;
using Didox.Application.Abstractions.Database;
using Didox.Application.Mappings;
using Didox.Application.Abstractions.Client;
using Didox.Application.Contracts.DidoxAccounts.Responses;
using Didox.Application.Contracts.DidoxClient.Contracts.Registration;
using Didox.Domain.Entities;
using Document.Contract.Gateways;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace Didox.Application.Services.Auth;

/// <summary>
/// Token service that can be used outside of HTTP context (e.g. background jobs) to obtain an active Didox token for a specific owner.
/// </summary>
public sealed class DidoxAuthService(
    IServiceScopeFactory scopeFactory,
    IFusionCache cache,
    IDidoxDbContext dbContext,
    IStringEncryptor encrypter,
    IDidoxClient didoxClient,
    IUserLookupService userLookupService,
    IExecutionContextProvider executionContextProvider,
    ISharedViewLocalizer sharedLocalizer,
    ILogger<DidoxAuthService> logger)
    : IDidoxAuthService
{
    // ------------------------------------------------------------------
    // TOKEN ACCESS — BACKGROUND SAFE
    // ------------------------------------------------------------------
    public async Task<Result<string>> GetActiveTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!executionContextProvider.IsAuthorized)
        {
            logger.LogWarning("Unable to get active token. User not logged in.");
            return Result.Failure<string>(sharedLocalizer.UnAuthorizedAccess(nameof(GetActiveTokenAsync)));
        }

        return await GetActiveTokenAsync(executionContextProvider.UserId, cancellationToken);
    }

    public async Task<Result<string>> GetActiveTokenAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var cacheKey = $"didox_token_{ownerId}";

        var token = await cache.GetOrSetAsync(
            cacheKey,
            async ct =>
            {
                using var scope = scopeFactory.CreateScope();

                var scopedDb = scope.ServiceProvider.GetRequiredService<IDidoxDbContext>();
                var scopedClient = scope.ServiceProvider.GetRequiredService<IDidoxClient>();
                var scopedEncrypter = scope.ServiceProvider.GetRequiredService<IStringEncryptor>();

                var result = await GetOrRefreshTokenInternalAsync(
                    scopedDb,
                    scopedClient,
                    scopedEncrypter,
                    ownerId,
                    ct);

                return result.IsSuccess ? result.Value : null;
            },
            options => options
                .SetDuration(TimeSpan.FromMinutes(5))
                .SetFactoryTimeouts(TimeSpan.FromSeconds(10))
                .SetFailSafe(true), token: cancellationToken);

        return token is null
            ? Result.Failure<string>(sharedLocalizer.DidoxRequestFailed("Unable to acquire token"))
            : Result.Success(token);
    }

    // ------------------------------------------------------------------
    // REGISTRATION — REQUEST ONLY (NO BACKGROUND)
    // ------------------------------------------------------------------
    public async Task<Result<DidoxAccountResponse>> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
    {
        if (!executionContextProvider.IsAuthorized)
        {
            logger.LogWarning("Unable to register user. User not logged in.");
            return Result.Failure<DidoxAccountResponse>(sharedLocalizer.UnAuthorizedAccess(nameof(RegisterAsync)));
        }

        var ownerId = executionContextProvider.UserId;
        var ownerResult = await userLookupService.GetByIdAsync(ownerId, cancellationToken);

        if (ownerResult.IsFailure)
        {
            logger.LogWarning("Owner not found. OwnerId={OwnerId}", ownerId);
            return Result.Failure<DidoxAccountResponse>(sharedLocalizer.OwnerNotFound(ownerId.ToString()));
        }

        var owner = ownerResult.Value;

        var response = await didoxClient.RegisterUserAsync(request, cancellationToken);

        if (!response.Success)
        {
            logger.LogWarning("Failed to register user. Response={Response}", response);
            return Result.Failure<DidoxAccountResponse>(sharedLocalizer.DidoxRequestFailed(response.ErrorMessage ?? response.StatusCode.ToString()));
        }

        var account = new DidoxAccount
        {
            OwnerId = ownerId,
            Login = owner.Tin ?? owner.Pinfl ?? string.Empty,
            Password = encrypter.Encrypt(request.Password),
            Pinfl = owner.Pinfl,
            Tin = owner.Tin
        };

        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return account.ToResponse();
    }

    // ------------------------------------------------------------------
    // INTERNAL TOKEN LOGIC — PURE, SCOPE-SAFE
    // ------------------------------------------------------------------

    private async Task<Result<string>> GetOrRefreshTokenInternalAsync(
        IDidoxDbContext db,
        IDidoxClient client,
        IStringEncryptor enc,
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var existingToken = await db.Tokens
            .FirstOrDefaultAsync(t => t.OwnerId == ownerId, cancellationToken);

        if (existingToken is not null && !IsTokenExpired(existingToken))
            return Result.Success(existingToken.Token);

        if (existingToken is not null)
        {
            db.Tokens.Remove(existingToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.OwnerId == ownerId, cancellationToken);

        if (account is null)
        {
            logger.LogWarning("Didox account not found. OwnerId={OwnerId}", ownerId);

            return Result.Failure<string>(sharedLocalizer.ResourceNotFound("DidoxAccount", nameof(ownerId)));
        }

        var loginResponse = await client.LoginWithPasswordAsync(account.Login, enc.Decrypt(account.Password), cancellationToken);

        if (!loginResponse.Success || loginResponse.Data is null)
        {
            logger.LogError("Didox login failed. StatusCode={StatusCode}, Error={Error}", loginResponse.StatusCode, loginResponse.ErrorMessage);
            return Result.Failure<string>(sharedLocalizer.DidoxRequestFailed(loginResponse.StatusCode.ToString()));
        }

        var newToken = new DidoxToken
        {
            Id = Guid.NewGuid(),
            Token = loginResponse.Data.Token,
            OwnerId = ownerId,
            ExpiresIn = DateTimeOffset.UtcNow.AddHours(5).ToUnixTimeMilliseconds(),
            CreatedDate = DateTimeOffset.UtcNow.UtcDateTime
        };

        db.Tokens.Add(newToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(newToken.Token);
    }

    private static bool IsTokenExpired(DidoxToken token) =>
        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= token.ExpiresIn;
}
