using Didox.Application.Services;
using Didox.Application.Services.Auth;
using Document.Contract.Gateways;
using Microsoft.Extensions.DependencyInjection;

namespace Didox.Application;

/// <summary>
/// Dependency injection configuration for Didox Application layer.
/// Note: CQRS handlers and validators are registered via AddCoreApplication in Maydon.Host.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDidoxApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IDidoxAuthService, DidoxAuthService>();
        services.AddScoped<IDocumentProviderService, DidoxDocumentProviderService>();
        
        return services;
    }
}
