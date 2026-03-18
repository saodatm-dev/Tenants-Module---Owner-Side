namespace Maydon.Host.Infrastructure;

internal sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context)
	{
		// Prevent MIME type sniffing
		context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

		// Clickjacking protection
		context.Response.Headers.Append("X-Frame-Options", "DENY");

		// XSS protection (legacy browsers)
		context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

		// Referrer policy
		context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

		// Permissions policy
		context.Response.Headers.Append("Permissions-Policy",
			"accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

		// Content Security Policy (adjust as needed)
		context.Response.Headers.Append("Content-Security-Policy",
			"default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self'; frame-ancestors 'none';");

		await next(context);
	}
}

internal static class SecurityHeaderExtensions
{
	extension(IApplicationBuilder app)
	{
		internal IApplicationBuilder UseSecurityHeader()
		{
			app.UseMiddleware<SecurityHeadersMiddleware>();
			return app;
		}
	}
}
