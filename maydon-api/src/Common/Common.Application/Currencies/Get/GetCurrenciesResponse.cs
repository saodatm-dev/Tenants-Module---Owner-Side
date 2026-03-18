namespace Common.Application.Currencies.Get;

public sealed record GetCurrenciesResponse(
	Guid Id,
	string Code,
	string Name,
	short Order);
