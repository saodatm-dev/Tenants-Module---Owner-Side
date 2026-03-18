using Identity.Domain.Accounts;

namespace Identity.Application.Authentication;

public sealed record AuthenticationResponse(
	string Token,
	DateTime TokenExpiredTime,
	string RefreshToken,
	DateTime RefreshTokenExpiredTime,
	AccountType AccountType);
