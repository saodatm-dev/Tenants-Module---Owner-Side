using Core.Application.Abstractions.Notifications;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// SignalR setup extensions for versioning.
/// </summary>
public static class SignalRExtensions
{
    /// <summary>
    /// Adds SignalR with Redis backplane and versioning infrastructure.
    /// </summary>
    public static IServiceCollection AddVersioningSignalR(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<RedisOptions>()
            .Bind(configuration.GetSection(RedisOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    
        var redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>() 
                           ?? throw new InvalidOperationException("Redis configuration is missing");
        
        var connectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string is required");

        services.AddSignalR()
            .AddStackExchangeRedis(connectionString, options =>
            {
                options.Configuration.ChannelPrefix = RedisChannel.Literal($"{redisOptions.ChannelPrefix}signalr:");
            });
    
        services.AddScoped<IEntityChangeNotificationHandler, SignalREntityChangeNotificationHandler>();

        return services;
    }

    /// <summary>
    /// Maps the VersioningHub endpoint.
    /// </summary>
    public static IApplicationBuilder MapVersioningHub(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<VersioningHub>("/hubs/versioning");
        });

        return app;
    }
}
