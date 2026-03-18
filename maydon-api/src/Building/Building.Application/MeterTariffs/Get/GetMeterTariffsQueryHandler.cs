using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTariffs.Get;

internal sealed class GetMeterTariffsQueryHandler(
	IBuildingDbContext dbContext) : IQueryHandler<GetMeterTariffsQuery, PagedList<GetMeterTariffsResponse>>
{
	public async Task<Result<PagedList<GetMeterTariffsResponse>>> Handle(GetMeterTariffsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.MeterTariffs
			.AsNoTrackingWithIdentityResolution()
			.Where(item =>
				(request.Type != null ? item.Type == request.Type : true) &&
				(request.IsActual != null ? item.IsActual == request.IsActual : true) &&
				(request.From != null ? item.ValidFrom == request.From : true) &&
				(request.To != null ? item.ValidUntil == request.To : true))
			.OrderByDescending(item => item.IsActual)
			.ThenByDescending(item => item.ValidFrom)
			.Select(item =>
				new GetMeterTariffsResponse(
					item.Id,
					item.MeterTypeId,
					dbContext.GetMeterTypeNameById(item.MeterTypeId),
					item.Price,
					item.Type,
					item.IsActual,
					item.MinLimit,
					item.MaxLimit,
					item.FixedPrice,
					item.Season,
					item.SocialNormLimit));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetMeterTariffsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
