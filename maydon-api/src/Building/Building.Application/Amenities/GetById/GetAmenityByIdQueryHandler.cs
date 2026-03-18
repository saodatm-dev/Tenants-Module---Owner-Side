using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Amenities.GetById;

internal sealed class GetAmenityByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetAmenityByIdQuery, GetAmenityByIdResponse>
{
	public async Task<Result<GetAmenityByIdResponse>> Handle(GetAmenityByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.Amenities
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Include(item => item.Translates)
			.Select(item => new GetAmenityByIdResponse(
				item.Id,
				item.IconUrl,
				item.Translates.Select(t => new LanguageValue(t.LanguageId, t.LanguageShortCode, t.Value))))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
