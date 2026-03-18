using Core.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Authentication;

internal sealed class ExecutionContextProvider(IHttpContextAccessor httpContextAccessor) : IExecutionContextProvider
{
	public Guid AccountId => httpContextAccessor
			.HttpContext?
			.User
			.GetAccountId() ??
		throw new ApplicationException("User context is unavailable");
	public Guid SessionId => httpContextAccessor
			.HttpContext?
			.User
			.GetSessionId() ??
		throw new ApplicationException("User context is unavailable");

	public Guid TenantId => httpContextAccessor
			.HttpContext?
			.User
			.GetTenantId() ?? Guid.Empty;

	public Guid UserId =>
		httpContextAccessor
			.HttpContext?
			.User
			.GetUserId() ?? Guid.Empty;
	public Guid RoleId =>
		httpContextAccessor
			.HttpContext?
			.User
			.GetRoleId() ??
		throw new ApplicationException("User context is unavailable");
	public bool IsIndividual => httpContextAccessor.HttpContext?.User.IsIndividual ?? false;
	public bool IsOwner => httpContextAccessor.HttpContext?.User.IsOwner ?? false;
	public int? AccountType => httpContextAccessor.HttpContext?.User.AccountType;
	public bool IsAuthorized => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
	public string LanguageShortCode => httpContextAccessor.GetLanguageShortCode();

	public string IpAddress
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(field))
				return field;

			var remoteIpAddress = httpContextAccessor
				.HttpContext?
				.Connection
				.RemoteIpAddress;

			if (remoteIpAddress is null)
				return string.Empty;

			if (remoteIpAddress.IsIPv4MappedToIPv6)
				field = remoteIpAddress.MapToIPv4().ToString();

			field = remoteIpAddress.ToString();

			return field;
		}
	}
	public string UserAgent
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(field))
				return field;

			field = httpContextAccessor
				.HttpContext?
				.Request
				.Headers["User-Agent"]
				.ToString();

			return field;
		}
	}

}
