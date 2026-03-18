using Core.Infrastructure.Extensions;
using Identity.Application.Core.Options;
using Microsoft.Extensions.Options;

namespace Maydon.Host.Extensions;

internal static class CookieExtensions
{
	extension(HttpContext httpContext)
	{
		internal void SetCookieValue(string token)
		{
			var applicationOptions = httpContext.RequestServices.GetRequiredService<IOptions<ApplicationOptions>>().Value;
			var environment = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
			var cookieName = applicationOptions?.CookieName ?? AuthenticationExtensions.DefaultCookieName;
			var cookieExpirationInMinutes = applicationOptions?.CookieExpirationInMinutes ?? 20;

			httpContext.Response.Cookies.Append(
				cookieName,
				token,
				new CookieOptions
				{
					HttpOnly = true,
					Secure = true,
					SameSite = environment.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Strict,
					Path = "/",
					Expires = DateTimeOffset.UtcNow.AddMinutes(cookieExpirationInMinutes),
				});
		}

		internal void RemoveCookieValue()
		{
			var applicationOptions = httpContext.RequestServices.GetRequiredService<IOptions<ApplicationOptions>>().Value;
			var environment = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
			var cookieName = applicationOptions?.CookieName ?? AuthenticationExtensions.DefaultCookieName;
			var cookieExpirationInMinutes = applicationOptions?.CookieExpirationInMinutes ?? 20;

			httpContext.Response.Cookies.Delete(
				cookieName,
				new CookieOptions
				{
					HttpOnly = true,
					Secure = true,
					SameSite = environment.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Strict,
					Path = "/",
					Expires = DateTimeOffset.UtcNow.AddMinutes(cookieExpirationInMinutes),
				});
		}
	}
}
