using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Complexes.GetImages;

public sealed class GetComplexImagesQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetComplexImagesQuery, IEnumerable<string>>
{
	public async Task<Result<IEnumerable<string>>> Handle(GetComplexImagesQuery request, CancellationToken cancellationToken)
	{
		var keys = await dbContext.ComplexImages
			.AsNoTracking()
			.Where(item => item.ComplexId == request.Id)
			.Select(item => item.ObjectName)
			.ToListAsync(cancellationToken);

		return (await fileUrlResolver.ResolveUrlsAsync(keys, cancellationToken)).ToList();
	}
}

