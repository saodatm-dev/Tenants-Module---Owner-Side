using Core.Domain.Roles;
using Identity.Domain.Accounts;

namespace Identity.Application.Core.Abstractions.Authentication;

public interface ITokenProvider
{
	(string, DateTime) Create(Account account, Guid sessionId, RoleType roleType);
	string CreateRefreshToken();
}
