using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Accounts.My;

internal sealed class GetMyAccountsQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	ICryptor cryptor,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetMyAccountsQuery, IEnumerable<GetMyAccountsResponse>>
{
	public async Task<Result<IEnumerable<GetMyAccountsResponse>>> Handle(GetMyAccountsQuery query, CancellationToken cancellationToken)
	{
		var accounts = await dbContext.Accounts
			.AsNoTrackingWithIdentityResolution()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.User)
			.Where(item => item.UserId == executionContextProvider.UserId)
			.LeftJoin(
				dbContext.Companies.AsNoTracking(),
				account => account.TenantId,
				company => company.Id,
				(account, company) => new { account, company })
		.Select(item => new GetMyAccountsResponse(
				item.company != null ? item.company.ObjectName : item.account.User.ObjectName,
				item.account.User.FirstName,
				item.account.User.LastName,
				item.account.User.MiddleName,
				item.company != null ? item.company.Name : null,
				item.account.Type,
				item.account.Id == executionContextProvider.AccountId,
				cryptor.EncryptAccount(item.account.Id, executionContextProvider.SessionId)))
			.ToListAsync(cancellationToken);

		if (!accounts.Any())
			return Result.Success(Enumerable.Empty<GetMyAccountsResponse>());

		var keys = accounts.Select(item => item.Photo).ToList();
		var resolvedUrls = await fileUrlResolver.ResolveManyAsync(keys, cancellationToken);
		var resolved = accounts.Select((item, i) =>
			item with { Photo = resolvedUrls[i] ?? string.Empty }).ToList();

		return resolved;
	}
}
