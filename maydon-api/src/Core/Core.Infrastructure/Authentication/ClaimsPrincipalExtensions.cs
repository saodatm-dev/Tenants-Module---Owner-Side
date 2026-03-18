using System.Security.Claims;
using Core.Application.Abstractions.Authentication;
using Core.Application.Resources;
using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
	extension(ClaimsPrincipal principal)
	{
		public Guid GetSessionId()
		{
			string? claimValue = principal?.FindFirstValue(IExecutionContextProvider.SessionIdKey);

			if (string.IsNullOrWhiteSpace(claimValue) || !Guid.TryParse(claimValue, out Guid sessionId) || sessionId == Guid.Empty)
				throw new AuthorizationException("User", "Unauthorized user");

			return sessionId;
		}
		public Guid GetAccountId()
		{
			string? claimValue = principal?.FindFirstValue(IExecutionContextProvider.AccountIdKey);

			if (string.IsNullOrWhiteSpace(claimValue) || !Guid.TryParse(claimValue, out Guid accountId) || accountId == Guid.Empty)
				throw new AuthorizationException("User", "Unauthorized user");

			return accountId;
		}


		public Guid GetTenantId()
		{
			string? claimValue = principal?.FindFirstValue(IExecutionContextProvider.TenantIdKey);

			if (!string.IsNullOrWhiteSpace(claimValue) && Guid.TryParse(claimValue, out Guid tenantId) && tenantId != Guid.Empty)
				return tenantId;

			return Guid.Empty;
		}

		public Guid GetUserId()
		{
			string? claimValue = principal?.FindFirstValue(IExecutionContextProvider.UserIdKey);

			if (!string.IsNullOrWhiteSpace(claimValue) && Guid.TryParse(claimValue, out Guid userId) && userId != Guid.Empty)
				return userId;

			return Guid.Empty;
		}

		public Guid GetRoleId()
		{
			string? claimValue = principal?.FindFirstValue(IExecutionContextProvider.RoleIdKey);

			if (string.IsNullOrWhiteSpace(claimValue) || !Guid.TryParse(claimValue, out Guid roleId) || roleId == Guid.Empty)
				throw new AuthorizationException("User", "Unauthorized user");

			return roleId;
		}

		public bool IsIndividual => bool.TryParse(principal?.FindFirstValue(IExecutionContextProvider.IsIndividualKey), out var value) && value;
		public bool IsOwner => bool.TryParse(principal?.FindFirstValue(IExecutionContextProvider.IsOwnerKey), out var value) && value;
		public int? AccountType
		{
			get
			{
				if (int.TryParse(principal?.FindFirstValue(IExecutionContextProvider.AccountTypeKey), out var value))
					return value;

				return null;
			}
		}
	}

	extension(IHttpContextAccessor httpContextAccessor)
	{
		public string GetLanguageShortCode()
		{
			if (httpContextAccessor.HttpContext is null)
				return ISharedViewLocalizer.UzbekTwoLetter;

			var request = httpContextAccessor.HttpContext.Request;

			if (request.Cookies.TryGetValue(ISharedViewLocalizer.LanguageKey, out var languageShortName) && IsSupportedLanguage(languageShortName))
				return languageShortName;

			if (request.Headers.TryGetValue(ISharedViewLocalizer.LanguageKey, out var langHeader) && IsSupportedLanguage(langHeader.ToString()))
				return langHeader.ToString();

			if (request.Headers.TryGetValue("Accept-Language", out var acceptLang) && !string.IsNullOrWhiteSpace(acceptLang.ToString()))
			{
				var parsed = ParseAcceptLanguage(acceptLang.ToString());
				if (parsed is not null)
					return parsed;
			}

			return ISharedViewLocalizer.UzbekTwoLetter;
		}

		private static bool IsSupportedLanguage(string? lang) =>
			!string.IsNullOrWhiteSpace(lang) &&
			(string.Equals(lang, ISharedViewLocalizer.UzbekTwoLetter, StringComparison.OrdinalIgnoreCase) ||
			 string.Equals(lang, ISharedViewLocalizer.RussianTwoLetter, StringComparison.OrdinalIgnoreCase) ||
			 string.Equals(lang, ISharedViewLocalizer.EnglishTwoLetter, StringComparison.OrdinalIgnoreCase));

		private static string? ParseAcceptLanguage(string headerValue)
		{
			foreach (var part in headerValue.Split(','))
			{
				var lang = part.Split(';')[0].Trim();
				if (lang.Length >= 2)
				{
					var twoLetter = lang[..2].ToLowerInvariant();
					if (IsSupportedLanguage(twoLetter))
						return twoLetter;
				}
			}
			return null;
		}
	}
}
