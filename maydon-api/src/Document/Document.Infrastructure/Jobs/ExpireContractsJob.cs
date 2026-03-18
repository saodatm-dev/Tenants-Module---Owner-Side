using Document.Application.Abstractions.Data;
using Document.Contract.Gateways;
using Document.Domain.Contracts.Enums;
using Document.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Document.Infrastructure.Jobs;

/// <summary>
/// Daily background job that expires contracts past their SignatureDeadline.
/// Targets contracts with OwnerSigned or PendingSignature status whose deadline has passed.
/// Updates both local contract state and Didox provider state.
/// </summary>
public sealed class ExpireContractsJob(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpireContractsJob> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ExpireContractsJob started. Checking every {Interval}", Interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(Interval, stoppingToken);
                await ExpireOverdueContractsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during contract expiration check");
            }
        }

        logger.LogInformation("ExpireContractsJob stopped");
    }

    private async Task ExpireOverdueContractsAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IDocumentDbContext>();
        var providerService = scope.ServiceProvider.GetRequiredService<IDocumentProviderService>();

        var now = DateTime.UtcNow;

        // Find contracts that are OwnerSigned (or PendingSignature) with expired deadline
        var overdueContracts = await dbContext.Contracts
            .Include(c => c.IntegrationStates)
            .Where(c => (c.Status == ContractStatus.OwnerSigned ||
                         c.Status == ContractStatus.PendingSignature) &&
                        c.SignatureDeadline != null &&
                        c.SignatureDeadline < now)
            .ToListAsync(ct);

        if (overdueContracts.Count == 0)
        {
            logger.LogDebug("No overdue contracts found");
            return;
        }

        logger.LogInformation("Found {Count} overdue contracts to expire", overdueContracts.Count);

        foreach (var contract in overdueContracts)
        {
            try
            {
                contract.MarkExpired();

                // Update Didox provider state to Expired
                var didoxState = contract.IntegrationStates
                    .FirstOrDefault(s => s.ProviderName == "Didox");

                if (didoxState?.ExternalId is not null)
                {
                    contract.AddOrUpdateProviderState(
                        "Didox",
                        didoxState.ExternalId,
                        ExternalSyncStatus.Expired,
                        "Contract expired — signature deadline passed");

                    logger.LogInformation(
                        "Marked Didox provider state as Expired for contract {ContractId} (ExternalId={ExternalId})",
                        contract.Id, didoxState.ExternalId);
                }

                logger.LogInformation("Expired contract {ContractId} (deadline {Deadline})",
                    contract.Id, contract.SignatureDeadline);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to expire contract {ContractId}", contract.Id);
            }
        }

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully expired {Count} contracts", overdueContracts.Count);
    }
}
