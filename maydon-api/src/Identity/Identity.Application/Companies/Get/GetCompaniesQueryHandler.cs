using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Companies.Get;

internal sealed class GetCompaniesQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetCompaniesQuery, PagedList<GetCompaniesResponse>>
{
	public async Task<Result<PagedList<GetCompaniesResponse>>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
	{
		if (executionContextProvider.IsIndividual)
			return PagedList<GetCompaniesResponse>.Empty(request.Page, request.PageSize);

		var query = dbContext.Companies
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter)
				? EF.Functions.Like(item.Name.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") ||
				  (!string.IsNullOrEmpty(item.Tin) ? EF.Functions.Like(item.Tin, $"%{request.Filter.ToLowerInvariant()}%") : false)
				: true)
			.OrderBy(item => item.Name)
			.Select(item => new GetCompaniesResponse(
				item.Id,
				item.Name,
				item.Tin,
				item.IsVerified,
				item.ObjectName))
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

		return new PagedList<GetCompaniesResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
