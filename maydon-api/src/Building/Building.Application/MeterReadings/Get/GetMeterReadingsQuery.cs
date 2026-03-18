using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.MeterReadings.Get;

public sealed record GetMeterReadingsQuery(
	Guid RealEstateId,
	[property: DefaultValue(null)] Guid? MeterId = null,
	[property: DefaultValue(null)] Guid? MeterTypeId = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetMeterReadingsResponse>(Page, PageSize);
