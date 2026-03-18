using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Accounts.Get;

internal sealed class GetAccountsQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	ICryptor cryptor,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetAccountsQuery, PagedList<GetAccountsResponse>>
{
	public async Task<Result<PagedList<GetAccountsResponse>>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Accounts
			.AsNoTrackingWithIdentityResolution()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.User)
			.Include(item => item.Company)
			.Where(item => !string.IsNullOrEmpty(request.Filter)
				? (item.Company != null && EF.Functions.Like(item.Company.Name.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")) ||
				  EF.Functions.Like(item.User.FullName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
				: true)
			.Select(item => new GetAccountsResponse(
				item.Company != null ? item.Company.ObjectName ?? item.User.ObjectName : item.User.ObjectName,
				item.Company != null ? item.Company.Name ?? item.User.FullName : item.User.FullName,
				item.Type,
				cryptor.EncryptAccount(item.Id, executionContextProvider.SessionId)));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		var keys = responsesPage.Select(item => item.Photo).ToList();
		var resolvedUrls = await fileUrlResolver.ResolveManyAsync(keys, cancellationToken);
		var resolvedPage = responsesPage.Select((item, i) =>
			item with { Photo = resolvedUrls[i] }).ToList();

		return new PagedList<GetAccountsResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
