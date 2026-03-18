using Core.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// Redis connection infrastructure extensions.
/// </summary>
internal static class RedisExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the shared Redis connection multiplexer.
        /// Expects a connection string at <c>ConnectionStrings:Redis</c>.
        /// </summary>
        internal IServiceCollection AddRedisInfrastructure(IConfiguration configuration)
        {
            services.AddOptions<RedisOptions>()
                .Bind(configuration.GetSection(RedisOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new InvalidOperationException(
                    "Redis connection string is required. Configure 'ConnectionStrings:Redis' in appsettings.json.");
            }
            
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
                var config = ConfigurationOptions.Parse(redisConnectionString);
                
                config.ConnectTimeout = options.ConnectTimeoutMs;
                config.SyncTimeout = options.SyncTimeoutMs;
                config.AbortOnConnectFail = false;
                config.ConnectRetry = options.MaxRetryAttempts;
                config.ChannelPrefix = RedisChannel.Literal(options.ChannelPrefix);
                config.DefaultDatabase = 0;
                
                return ConnectionMultiplexer.Connect(config);
            });

            return services;
        }
    }
}
