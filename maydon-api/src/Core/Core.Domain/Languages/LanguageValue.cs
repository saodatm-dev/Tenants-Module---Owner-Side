namespace Core.Domain.Languages;

public sealed record LanguageValue(
	Guid LanguageId,
	string LanguageShortCode,
	string Value);
