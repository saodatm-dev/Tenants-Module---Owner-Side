namespace Common.Application.Languages.Get;

public sealed record GetLanguagesResponse(
	Guid Id,
	string Name,
	string ShortCode,
	int Order);
