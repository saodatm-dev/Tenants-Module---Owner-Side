using Microsoft.Extensions.Localization;

namespace Core.Application.Resources;

public interface ISharedViewLocalizer
{
	const string UzbekTwoLetter = "uz", RussianTwoLetter = "ru", EnglishTwoLetter = "en", LanguageKey = "lang";
	LocalizedString this[string key] { get; }
	LocalizedString GetLocalizedString(string key);
	IStringLocalizer Localizer { get; }
}

