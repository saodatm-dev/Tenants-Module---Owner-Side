using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

public static class AntiforgeryExtensions
{
	internal const string AntiforgeryHeaderName = "X-CSRF-TOKEN";
	public const string AntiforgeryCookieName = "XSRF-TOKEN";
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddAntiforgeryInternal()
		{
			return services.AddAntiforgery(options =>
			{
				//options.HeaderName = AntiforgeryHeaderName;
				//options.Cookie.Name = AntiforgeryCookieName;
				//options.Cookie.HttpOnly = true;                          // Set to false if you need to read it from JavaScript
				//options.Cookie.SameSite = SameSiteMode.Strict;
				//options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use in production with HTTPS
				//options.Cookie.Path = "/";
				options.SuppressXFrameOptionsHeader = false;             // Suppress exceptions (use with caution)
																		 //options.FormFieldName = "__RequestVerificationToken";
			});
		}
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
