using System.Globalization;
using System.Text.RegularExpressions;
using Core.Application.Resources;
using Microsoft.AspNetCore.Localization;

namespace Maydon.Host.Extensions;

internal static class LocalizationExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddLocalizationInternal()
		{
			services.AddLocalization(options => options.ResourcesPath = "Resources");
			services.Configure<RequestLocalizationOptions>(options =>
			{
				var supportedCultures = new[]
				{
				new CultureInfo(ISharedViewLocalizer.UzbekTwoLetter),
				new CultureInfo(ISharedViewLocalizer.RussianTwoLetter),
				new CultureInfo(ISharedViewLocalizer.EnglishTwoLetter)
			};

				options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(ISharedViewLocalizer.UzbekTwoLetter);
				options.SupportedCultures = supportedCultures;
				options.SupportedUICultures = supportedCultures;
				options.RequestCultureProviders.Insert(0, new RouteCultureProvider(new RequestCulture(supportedCultures[0])));
			});

			services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
			return services;
		}
	}
}

file sealed class RouteCultureProvider : IRequestCultureProvider
{
	private const string LanguageTwoLetterRegexPattern = @"^[a-z]{2}(-[A-Z]{2})*$";
	private CultureInfo defaultCulture;
	private CultureInfo defaultUICulture;

	public RouteCultureProvider(RequestCulture requestCulture)
	{
		this.defaultCulture = requestCulture.Culture;
		this.defaultUICulture = requestCulture.UICulture;
	}

	public Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
	{
		// Test any culture in route
		if (httpContext.Request.Path.ToString().Length <= 1)
		{
			// Set default Culture and default UICulture
			return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(this.defaultCulture.TwoLetterISOLanguageName, this.defaultUICulture.TwoLetterISOLanguageName));
		}

		var culture = ISharedViewLocalizer.UzbekTwoLetter;

		if (httpContext.Request.Cookies.TryGetValue(ISharedViewLocalizer.LanguageKey, out var cookieValue))
			culture = cookieValue;

		if (httpContext.Request.Headers.TryGetValue(ISharedViewLocalizer.LanguageKey, out var langHeader) && !string.IsNullOrWhiteSpace(langHeader.ToString()))
			culture = langHeader;
		else if (httpContext.Request.Headers.TryGetValue("Accept-Language", out var acceptLang) && !string.IsNullOrWhiteSpace(acceptLang.ToString()))
			culture = ParseAcceptLanguage(acceptLang.ToString()) ?? culture;

		if (httpContext.Request.Query.TryGetValue(ISharedViewLocalizer.LanguageKey, out var queryValue))
			culture = queryValue;

		// Test if the culture is properly formatted
		if (string.IsNullOrWhiteSpace(culture) || !Regex.IsMatch(culture, LanguageTwoLetterRegexPattern) ||
			(!string.Equals(culture, ISharedViewLocalizer.UzbekTwoLetter, StringComparison.OrdinalIgnoreCase) &&
			!string.Equals(culture, ISharedViewLocalizer.RussianTwoLetter, StringComparison.OrdinalIgnoreCase) &&
			!string.Equals(culture, ISharedViewLocalizer.EnglishTwoLetter, StringComparison.OrdinalIgnoreCase)))
		{
			// Set default Culture and default UICulture
			return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(this.defaultCulture.TwoLetterISOLanguageName, this.defaultUICulture.TwoLetterISOLanguageName));
		}

		// Set Culture and UICulture from route culture parameter
		return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture, culture));
	}

	private static string? ParseAcceptLanguage(string headerValue)
	{
		string[] supportedLanguages = [ISharedViewLocalizer.UzbekTwoLetter, ISharedViewLocalizer.RussianTwoLetter, ISharedViewLocalizer.EnglishTwoLetter];
		foreach (var part in headerValue.Split(','))
		{
			var lang = part.Split(';')[0].Trim();
			if (lang.Length >= 2)
			{
				var twoLetter = lang[..2].ToLowerInvariant();
				if (supportedLanguages.Any(s => string.Equals(s, twoLetter, StringComparison.OrdinalIgnoreCase)))
					return twoLetter;
			}
		}
		return null;
	}
}
