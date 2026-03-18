using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Users.Get;

internal sealed class GetUsersQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetUsersQuery, PagedList<GetUsersResponse>>
{
	public async Task<Result<PagedList<GetUsersResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		if (executionContextProvider.IsIndividual)
			return PagedList<GetUsersResponse>.Empty(request.Page, request.PageSize);

		var query = dbContext.CompanyUsers
			.Include(item => item.Company)
			.Include(item => item.User)
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter)
				? (!string.IsNullOrEmpty(item.User.FirstName) ? EF.Functions.Like(item.User.FirstName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.User.LastName) ? EF.Functions.Like(item.User.LastName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.User.MiddleName) ? EF.Functions.Like(item.User.MiddleName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false)
				: true)
			.OrderBy(item => item.User.LastName)
			.Select(item => new GetUsersResponse(
				item.UserId,
				item.User.FirstName,
				item.User.LastName,
				item.User.MiddleName,
				item.User.ObjectName))
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

		return new PagedList<GetUsersResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
