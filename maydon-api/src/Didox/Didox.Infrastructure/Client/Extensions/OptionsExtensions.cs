using Didox.Infrastructure.Client.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Didox.Infrastructure.Client.Extensions;

/// <summary>
/// Extension methods for configuring options with validation.
/// </summary>
internal static class OptionsExtensions
{
    /// <summary>
    /// Adds and validates Didox-related options with data annotations and startup validation.
    /// </summary>
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        
        services.AddOptions<DidoxOptions>()
            .Bind(configuration.GetSection(DidoxOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(o =>
                    !string.IsNullOrWhiteSpace(o.BaseUrl) &&
                    !string.IsNullOrWhiteSpace(o.PartnerToken),
                "Didox configuration is invalid")
            .ValidateOnStart();

        return services;
    }
}
