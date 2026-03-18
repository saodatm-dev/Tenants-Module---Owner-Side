using Core.Application.Abstractions.Jobs;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Repositories;
using Core.Infrastructure.Jobs;
using Core.Infrastructure.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// Versioning infrastructure DI extensions.
/// </summary>
public static class VersioningExtensions
{
    /// <summary>
    /// Adds versioning infrastructure: MongoDB client, version repository, workflow processor,
    /// and the EntityVersionChangedHandler for processing version events from Redis Streams.
    /// </summary>
    public static IServiceCollection AddVersioning(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var mongoConnectionString = configuration.GetConnectionString("MongoDB")
                                    ?? throw new InvalidOperationException("MongoDB connection string is not configured.");

        var mongoUrl = MongoUrl.Create(mongoConnectionString);
        var databaseName = mongoUrl.DatabaseName
                           ?? throw new InvalidOperationException("MongoDB database name is not specified in connection string.");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        services.AddSingleton<IVersionRepository, MongoVersionRepository>();
        services.AddScoped<IVersioningWorkflowProcessor, VersioningWorkflowProcessor>();

        // Register the integration event handler so the pipeline can dispatch version events.
        // This handler lives in Core.Infrastructure, so it's not picked up by the application-layer
        // assembly scanning in AddCoreApplication.
        services.AddScoped<IIntegrationEventHandler<EntityVersionChangedEvent>, EntityVersionChangedHandler>();

        return services;
    }
}
