using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Didox.Infrastructure.Client.Extensions;

/// <summary>
/// Extension methods for configuring antiforgery services.
/// </summary>
internal static class AntiforgeryExtensions
{
    /// <summary>
    /// Antiforgery token header name for CSRF protection
    /// </summary>
    internal const string AntiforgeryHeaderName = "X-CSRF-TOKEN";

    /// <summary>
    /// Antiforgery cookie name for CSRF protection
    /// </summary>
    public const string AntiforgeryCookieName = "XSRF-TOKEN";

    /// <summary>
    /// Adds and configures antiforgery services for CSRF protection.
    /// </summary>
    public static IServiceCollection AddAntiforgery(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        return services.AddAntiforgery(options =>
        {
            // Configuration options are commented out but available for future use
            // options.HeaderName = AntiforgeryHeaderName;
            // options.Cookie.Name = AntiforgeryCookieName;
            // options.Cookie.HttpOnly = true;
            // options.Cookie.SameSite = SameSiteMode.Strict;
            // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            // options.Cookie.Path = "/";
            options.SuppressXFrameOptionsHeader = false;
        });
    }
}

public sealed class ValidateAntiforgeryTokenFilter(IAntiforgery antiforgery) : IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var method = context.HttpContext.Request.Method;
		if (method == HttpMethod.Post.Method ||
			method == HttpMethod.Put.Method ||
			method == HttpMethod.Delete.Method ||
			method == HttpMethod.Patch.Method)
		{
			try
			{
				await antiforgery.ValidateRequestAsync(context.HttpContext);
			}
			catch (AntiforgeryValidationException)
			{
				throw;
			}
		}

		return await next(context);
	}
}

public sealed class AntiforgeryFilter(IAntiforgery antiforgery) : IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var result = await next(context);

		var method = context.HttpContext.Request.Method;
		if (method == HttpMethod.Get.Method)
			antiforgery.GetAndStoreTokens(context.HttpContext);

		return result;
	}
}
