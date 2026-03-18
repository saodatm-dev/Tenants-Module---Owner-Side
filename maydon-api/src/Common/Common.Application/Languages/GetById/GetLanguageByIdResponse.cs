namespace Common.Application.Languages.GetById;

public sealed record GetLanguageByIdResponse(
	Guid Id,
	string Name,
	string ShortCode,
	int Order);
