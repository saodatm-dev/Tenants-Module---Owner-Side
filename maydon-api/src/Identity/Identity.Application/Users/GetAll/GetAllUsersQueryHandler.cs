using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Users.GetAll;

internal sealed class GetAllUsersQueryHandler(
	IIdentityDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetAllUsersQuery, PagedList<GetAllUsersResponse>>
{
	public async Task<Result<PagedList<GetAllUsersResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Users
			.AsNoTrackingWithIdentityResolution()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter)
				? (!string.IsNullOrEmpty(item.FirstName) ? EF.Functions.Like(item.FirstName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.LastName) ? EF.Functions.Like(item.LastName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.MiddleName) ? EF.Functions.Like(item.MiddleName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.PhoneNumber) ? EF.Functions.Like(item.PhoneNumber, $"%{request.Filter}%") : false)
				: true)
			.OrderBy(item => item.LastName)
			.Select(item => new GetAllUsersResponse(
				item.Id,
				item.PhoneNumber,
				item.FirstName,
				item.LastName,
				item.MiddleName,
				item.ObjectName,
				item.Accounts.Count,
				item.IsVerified,
				item.RegisterType,
				item.IsActive))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		var keys = responsesPage.Select(item => item.Photo).ToList();
		var resolvedUrls = await fileUrlResolver.ResolveManyAsync(keys, cancellationToken);
		var resolvedPage = responsesPage.Select((item, i) =>
			item with { Photo = resolvedUrls[i] }).ToList();

		return new PagedList<GetAllUsersResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
