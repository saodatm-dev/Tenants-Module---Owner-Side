using Core.Application.Abstractions.Pdf;
using Core.Infrastructure.Pdf;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// Dependency injection extensions for PDF rendering services.
/// </summary>
public static class PdfRenderingExtensions
{
    /// <summary>
    /// Adds PDF rendering services using Gotenberg.
    /// </summary>
    public static IServiceCollection AddPdfRendering(this IServiceCollection services, string gotenbergBaseUrl)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(gotenbergBaseUrl);
        
        services.AddHttpClient<IPdfRenderer, GotenbergPdfRenderer>(client =>
        {
            client.BaseAddress = new Uri(gotenbergBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
