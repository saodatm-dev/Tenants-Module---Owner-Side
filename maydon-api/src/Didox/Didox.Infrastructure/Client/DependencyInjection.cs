using Didox.Infrastructure.Client.Extensions;
using Didox.Application.Abstractions.Client;
using Didox.Application.Abstractions.Client.Account;
using Didox.Application.Abstractions.Client.Document;
using Didox.Application.Abstractions.Client.Eimzo;
using Didox.Application.Abstractions.Client.JsonDocument;
using Didox.Application.Abstractions.Client.Login;
using Didox.Application.Abstractions.Client.Registration;
using Didox.Infrastructure.Client.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using DidoxClient = Didox.Infrastructure.Client.Features.DidoxClient;

namespace Didox.Infrastructure.Client;

/// <summary>
/// Dependency injection configuration for Didox Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Didox Infrastructure services to the service collection.
    /// Registers HTTP clients, options, antiforgery, and business services.
    /// </summary>
    public static IServiceCollection AddDidoxClient(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Register and validate options
        services.AddOptions(configuration);

        // Register antiforgery services
        services.AddAntiforgery(configuration);

        // Configure HTTP client defaults with resilience policies
        services.AddHttpClientConfiguration();

        // Register Didox HTTP client with resilience and authentication token forwarding
        services.AddHttpClient<DidoxClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<DidoxOptions>>().Value;

            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }
        })
        .ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();

            if (!env.IsDevelopment())
                return new HttpClientHandler();

            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        })
        .AddAuthToken();

        // Register Interfaces to the concrete DidoxClient
        services.AddScoped<IDidoxClient>(sp => sp.GetRequiredService<DidoxClient>());
        services.AddScoped<IDidoxLoginClient>(sp => sp.GetRequiredService<DidoxClient>());
        services.AddScoped<IDidoxAccountClient>(sp => sp.GetRequiredService<DidoxClient>());
        services.AddScoped<IDidoxDocumentClient>(sp => sp.GetRequiredService<DidoxClient>());
        services.AddScoped<IDidoxJsonDocumentClient>(sp => sp.GetRequiredService<DidoxClient>());
        services.AddScoped<IDidoxEimzoClient>(sp => sp.GetRequiredService<DidoxClient>());
        services.AddScoped<IDidoxRegistrationClient>(sp => sp.GetRequiredService<DidoxClient>());

        return services;
    }
}
