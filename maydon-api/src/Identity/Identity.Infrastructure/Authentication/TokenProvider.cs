using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Application.Abstractions.Authentication;
using Core.Domain.Providers;
using Core.Domain.Roles;
using Core.Infrastructure.Options;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Domain.Accounts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Authentication;

internal sealed class TokenProvider(
	IDateTimeProvider dateTimeProvider,
	IOptions<JwtOptions> options) : ITokenProvider
{
	private readonly JwtOptions jwtOptions = options.Value;
	public (string, DateTime) Create(Account account, Guid sessionId, RoleType roleType)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var tokenExpiration = dateTimeProvider.UtcNow.AddMinutes(jwtOptions.ExpirationInMinutes);

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(
			[
				new Claim(IExecutionContextProvider.AccountIdKey,  $"{account.Id}"),
				new Claim(IExecutionContextProvider.SessionIdKey,  $"{sessionId}"),
				new Claim(IExecutionContextProvider.TenantIdKey, $"{account.TenantId}"),
				new Claim(IExecutionContextProvider.UserIdKey,  $"{account.UserId}"),
				new Claim(IExecutionContextProvider.RoleIdKey,  $"{account.RoleId}"),
				new Claim(IExecutionContextProvider.IsOwnerKey, $"{roleType == RoleType.Owner}"),
				new Claim(IExecutionContextProvider.AccountTypeKey, $"{(int)account.Type}"),
				new Claim(IExecutionContextProvider.IsIndividualKey, $"{account.TenantId == account.UserId}")
			]),
			Expires = tokenExpiration,
			SigningCredentials = credentials,
			Issuer = jwtOptions.Issuer,
			Audience = jwtOptions.Audience
		};

		var handler = new JsonWebTokenHandler();

		string token = handler.CreateToken(tokenDescriptor);

		return (token, tokenExpiration);
	}

	public string CreateRefreshToken()
	{
		Span<byte> randomNumber = stackalloc byte[64];
		using var randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}
