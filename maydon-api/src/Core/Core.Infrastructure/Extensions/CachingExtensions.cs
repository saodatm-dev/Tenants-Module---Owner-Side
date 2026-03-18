using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// FusionCache configuration extensions with sensible defaults and optional Redis L2 support.
/// </summary>
public static class CachingExtensions
{
    public static readonly TimeSpan ShortDuration = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan MediumDuration = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan LongDuration = TimeSpan.FromHours(2);

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers FusionCache with production defaults and optional Redis L2 backplane.
        /// Configuration section: <c>FusionCache:*</c>.
        /// </summary>
        public IServiceCollection AddFusionCacheDefaults(IConfiguration configuration, string? cacheName = null)
        {
            var enableRedis = configuration.GetValue("FusionCache:EnableRedis", false);
            var defaultDurationMinutes = configuration.GetValue("FusionCache:DefaultDurationMinutes", 30);
            var failSafeMaxDurationHours = configuration.GetValue("FusionCache:FailSafeMaxDurationHours", 24);
            var factorySoftTimeoutMs = configuration.GetValue("FusionCache:FactorySoftTimeoutMs", 500);
            var factoryHardTimeoutSeconds = configuration.GetValue("FusionCache:FactoryHardTimeoutSeconds", 5);

            var builder = services.AddFusionCache()
                .WithDefaultEntryOptions(options =>
                {
                    options.Duration = TimeSpan.FromMinutes(defaultDurationMinutes);

                    options.IsFailSafeEnabled = true;
                    options.FailSafeMaxDuration = TimeSpan.FromHours(failSafeMaxDurationHours);
                    options.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);

                    options.FactorySoftTimeout = TimeSpan.FromMilliseconds(factorySoftTimeoutMs);
                    options.FactoryHardTimeout = TimeSpan.FromSeconds(factoryHardTimeoutSeconds);

                    options.JitterMaxDuration = TimeSpan.FromSeconds(2);

                    options.EagerRefreshThreshold = 0.9f;
                });

            if (enableRedis)
            {
                var redisConnection = configuration.GetConnectionString("Redis");
                if (!string.IsNullOrWhiteSpace(redisConnection))
                {
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = redisConnection;
                        options.InstanceName = cacheName ?? "maydon";
                    });

                    builder.WithSystemTextJsonSerializer();
                }
            }

            return services;
        }

        /// <summary>
        /// Registers FusionCache with custom entry options.
        /// </summary>
        public IServiceCollection AddFusionCacheWithOptions(Action<FusionCacheEntryOptions> configureOptions, string? cacheName = null)
        {
            services.AddFusionCache()
                .WithDefaultEntryOptions(configureOptions);

            return services;
        }
    }
}
