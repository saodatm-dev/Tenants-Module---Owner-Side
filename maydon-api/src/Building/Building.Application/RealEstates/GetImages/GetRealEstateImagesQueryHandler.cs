using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.GetImages;

public sealed class GetRealEstateImagesQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetRealEstateImagesQuery, IEnumerable<GetRealEstateImagesResponse>>
{
	public async Task<Result<IEnumerable<GetRealEstateImagesResponse>>> Handle(GetRealEstateImagesQuery request, CancellationToken cancellationToken)
	{
		var images = await dbContext.RealEstateImages
			.AsNoTracking()
			.Where(x => x.RealEstateId == request.RealEstateId)
			.Select(item => new { item.Id, item.ObjectName })
			.ToListAsync(cancellationToken);

		var result = new List<GetRealEstateImagesResponse>();
		foreach (var img in images)
		{
			var url = await fileUrlResolver.ResolveUrlAsync(img.ObjectName, cancellationToken);
			if (url is not null)
				result.Add(new GetRealEstateImagesResponse(img.Id, url));
		}

		return result;
	}
}
