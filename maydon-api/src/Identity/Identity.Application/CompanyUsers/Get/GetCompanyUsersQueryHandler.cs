using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.CompanyUsers.Get;

internal sealed class GetCompanyUsersQueryHandler(
	IExecutionContextProvider executionContextProvider,
	ISharedViewLocalizer sharedViewLocalizer,
	IIdentityDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetCompanyUsersQuery, PagedList<GetCompanyUsersResponse>>
{
	public async Task<Result<PagedList<GetCompanyUsersResponse>>> Handle(GetCompanyUsersQuery request, CancellationToken cancellationToken)
	{
		// Only owners can view company users
		if (!executionContextProvider.IsOwner)
			return Result.Failure<PagedList<GetCompanyUsersResponse>>(sharedViewLocalizer.InvitationNoPermission(nameof(GetCompanyUsersQuery)));

		var tenantId = executionContextProvider.TenantId;

		var query = dbContext.CompanyUsers
			.AsNoTrackingWithIdentityResolution()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.User)
			.Where(item => item.CompanyId == tenantId)
			.Where(item => !string.IsNullOrEmpty(request.Filter)
				? EF.Functions.Like((item.User.FirstName ?? "") + " " + (item.User.LastName ?? ""), $"%{request.Filter.ToLowerInvariant()}%") ||
				  EF.Functions.Like(item.User.PhoneNumber ?? "", $"%{request.Filter.ToLowerInvariant()}%")
				: true)
			.LeftJoin(
				dbContext.Accounts
					.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter, IApplicationDbContext.IsActiveFilter])
					.Include(item => item.Role),
				companyUser => new { companyUser.CompanyId, companyUser.UserId },
				account => new { CompanyId = account.TenantId, account.UserId },
				(companyUser, account) => new { companyUser, account })
			.OrderByDescending(item => item.companyUser.IsOwner)
			.ThenBy(item => item.companyUser.User.FirstName)
			.ThenBy(item => item.companyUser.User.LastName)
			.Select(item => new GetCompanyUsersResponse(
				item.companyUser.UserId,
				((item.companyUser.User.FirstName ?? "") + " " + (item.companyUser.User.LastName ?? "")).Trim(),
				item.companyUser.User.PhoneNumber ?? "",
				item.companyUser.User.ObjectName,
				item.account != null && item.account.Role != null ? item.account.Role.Name : null,
				item.companyUser.IsOwner,
				item.account != null && item.account.IsActive,
				item.companyUser.CreatedAt));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		var keys = responsesPage.Select(item => item.Photo).ToList();
		var resolvedUrls = await fileUrlResolver.ResolveManyAsync(keys, cancellationToken);
		var resolvedPage = responsesPage.Select((item, i) =>
			item with { Photo = resolvedUrls[i] }).ToList();

		return new PagedList<GetCompanyUsersResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
