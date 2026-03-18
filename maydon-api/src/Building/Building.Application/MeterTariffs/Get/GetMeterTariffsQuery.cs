using System.ComponentModel;
using Building.Domain.MeterTariffs;
using Core.Application.Pagination;

namespace Building.Application.MeterTariffs.Get;

public sealed record GetMeterTariffsQuery(
	[property: DefaultValue(null)] MeterTariffType? Type = null,
	[property: DefaultValue(null)] bool? IsActual = null,
	[property: DefaultValue(null)] DateOnly? From = null,
	[property: DefaultValue(null)] DateOnly? To = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetMeterTariffsResponse>(Page, PageSize);
