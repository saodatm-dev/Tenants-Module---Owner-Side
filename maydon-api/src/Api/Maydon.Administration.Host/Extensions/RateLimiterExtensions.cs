using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;

namespace Maydon.Administration.Host.Extensions;

internal static class RateLimiterExtensions
{
	internal const string RateLimiterPolicyName = "JWT";
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddRateLimiterInternal()
		{
			services.AddRateLimiter(options =>
			{
				options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
				options.OnRejected = async (context, cancellationToken) =>
				{
					if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
						context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);

					context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
					await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
				};
				options.AddPolicy(RateLimiterPolicyName, httpContext =>
				{
					var accessToken = httpContext.GetTokenAsync("access_token").Result;
					var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

					return !string.IsNullOrEmpty(accessToken)
						? RateLimitPartition.GetFixedWindowLimiter(accessToken, options =>
							new FixedWindowRateLimiterOptions
							{
								QueueLimit = 5,
								PermitLimit = 100,
								QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
								AutoReplenishment = true,
								Window = TimeSpan.FromSeconds(10),
							})
						: RateLimitPartition.GetFixedWindowLimiter($"anonymous_{ipAddress}", options =>
							new FixedWindowRateLimiterOptions
							{
								QueueLimit = 2,
								PermitLimit = 30,
								QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
								Window = TimeSpan.FromSeconds(10),
							});
				});
			});
			return services;
		}
	}
}
