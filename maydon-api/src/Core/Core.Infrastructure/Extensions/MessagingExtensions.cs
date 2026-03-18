using Core.Application.Abstractions.Messaging;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Jobs;
using Core.Infrastructure.Messaging;
using Core.Infrastructure.Messaging.Behaviors;
using Core.Infrastructure.Monitoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// Integration event infrastructure DI extensions.
/// </summary>
public static class MessagingExtensions
{
    /// <summary>
    /// Registers complete integration event infrastructure using Redis Streams.
    /// Includes handler pipeline, behaviors, and observability.
    /// Prerequisites: Redis connection must be registered (via AddRedisInfrastructure).
    /// </summary>
    public static IServiceCollection AddIntegrationEvents(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddScoped<IIntegrationEventPublisher, RedisStreamsEventPublisher>();

        services.Configure<RedisStreamsCleanupOptions>(configuration.GetSection(RedisStreamsCleanupOptions.SectionName));
        services.Configure<IntegrationEventOptions>(configuration.GetSection(IntegrationEventOptions.SectionName));
        services.AddHostedService<RedisStreamsCleanupWorker>();
        services.AddHostedService<IntegrationEventConsumerWorker>();

        services.AddScoped<IIntegrationEventHandlerPipeline, IntegrationEventHandlerPipeline>();

        services.AddSingleton<IIdempotencyStore, RedisIdempotencyStore>();
        services.AddSingleton<IntegrationEventMetrics>();

        services.AddSingleton<IGlobalIntegrationEventBehavior, ValidationBehavior>();
        services.AddSingleton<IGlobalIntegrationEventBehavior, LoggingBehavior>();
        services.AddSingleton<IGlobalIntegrationEventBehavior, MetricsBehavior>();
        services.AddSingleton<IGlobalIntegrationEventBehavior, RetryBehavior>();

        return services;
    }
}
