using Core.Infrastructure.Authorization;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Authorization;

internal sealed class IdentitySessionProvider(IIdentityDbContext dbContext) : ISessionProvider
{
	public Task<bool> IsActual(Guid sessionId, Guid accountId) =>
		dbContext.Sessions.AsNoTracking().AnyAsync(item => item.Id == sessionId && item.AccountId == accountId && !item.IsTerminated);
}
