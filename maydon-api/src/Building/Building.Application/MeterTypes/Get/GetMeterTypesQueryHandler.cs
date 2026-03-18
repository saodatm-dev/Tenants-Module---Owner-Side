using Building.Application.Core.Abstractions.Data;
using Building.Domain.MeterTypes;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTypes.Get;

internal sealed class GetMeterTypesQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetMeterTypesQuery, IEnumerable<GetMeterTypesResponse>>
{
	public async Task<Result<IEnumerable<GetMeterTypesResponse>>> Handle(GetMeterTypesQuery request, CancellationToken cancellationToken) =>
		await dbContext.MeterTypes
		.Where(item => item.IsActive)
		.Select(item => new GetMeterTypesResponse(
			item.Id,
			item.Translates
				.Where(translate => translate.Field == MeterTypeField.Name)
				.Select(translate => translate.Value)
				.FirstOrDefault() ?? string.Empty,
			item.Translates
				.Where(translate => translate.Field == MeterTypeField.Description)
				.Select(translate => translate.Value)
				.FirstOrDefault() ?? string.Empty,
		item.Icon))
	.ToListAsync(cancellationToken);
}
