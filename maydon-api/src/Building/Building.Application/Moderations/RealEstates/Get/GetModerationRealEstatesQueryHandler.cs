using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstates;
using Building.Domain.RealEstateTypes;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstates.Get;

internal sealed class GetModerationRealEstatesQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetModerationRealEstatesQuery, PagedList<GetModerationRealEstatesResponse>>
{
	public async Task<Result<PagedList<GetModerationRealEstatesResponse>>> Handle(GetModerationRealEstatesQuery request, CancellationToken cancellationToken)
	{
		var query = from realEstate in dbContext.RealEstates
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item =>
				!string.IsNullOrWhiteSpace(request.Filter) && !string.IsNullOrEmpty(item.Number)
				? EF.Functions.Like(item.Number.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
				: true)
			.Where(item => !request.ModerationStatus.HasValue || item.ModerationStatus == request.ModerationStatus.Value)

			let typeName = dbContext.RealEstateTypeTranslates
				.Where(t => t.RealEstateTypeId == realEstate.RealEstateTypeId && t.Field == RealEstateTypeField.Name)
				.Select(t => t.Value)
				.FirstOrDefault()

			select new GetModerationRealEstatesResponse(
				realEstate.Id,
				realEstate.OwnerId,
				realEstate.Number,
				realEstate.BuildingNumber,
				realEstate.FloorNumber,
				realEstate.Address,
				typeName ?? string.Empty,
				realEstate.TotalArea,
				realEstate.RoomsCount,
				realEstate.Status,
				realEstate.ModerationStatus,
				realEstate.CreatedAt,
				realEstate.Images != null && realEstate.Images.Any() ? realEstate.Images.Select(image => image.ObjectName) : null);

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		// Resolve image URLs
		var resolvedItems = new List<GetModerationRealEstatesResponse>(responsesPage.Count);
		foreach (var item in responsesPage)
		{
			var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(item.ObjectNames, cancellationToken);
			resolvedItems.Add(item with { ObjectNames = resolvedImages });
		}

		return new PagedList<GetModerationRealEstatesResponse>(resolvedItems, request.Page, request.PageSize, totalCount);
	}
}
