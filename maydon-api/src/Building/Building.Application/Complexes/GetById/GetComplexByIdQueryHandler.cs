using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Complexes.GetById;

internal sealed class GetComplexByIdQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetComplexByIdQuery, GetComplexByIdResponse>
{
	public async Task<Result<GetComplexByIdResponse>> Handle(GetComplexByIdQuery request, CancellationToken cancellationToken)
	{
		var complex = await dbContext.Complexes
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Include(item => item.Descriptions)
			.Include(item => item.Images)
			.Select(item => new GetComplexByIdResponse(
				item.Id,
				dbContext.GetRegionName(item.RegionId),
				dbContext.GetDistrictName(item.DistrictId),
				item.Name,
				item.Descriptions != null && item.Descriptions.Any() ? item.Descriptions.First().Value : string.Empty,
				item.IsCommercial,
				item.IsLiving,
				item.Location != null ? item.Location.X : null,
				item.Location != null ? item.Location.Y : null,
				item.Address,
				item.Images != null ? item.Images.Select(item => item.ObjectName) : null))
			.FirstOrDefaultAsync(cancellationToken);

		if (complex is null)
			return Result<GetComplexByIdResponse>.None;

		var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(complex.Images, cancellationToken);
		return complex with { Images = resolvedImages };
	}
}

