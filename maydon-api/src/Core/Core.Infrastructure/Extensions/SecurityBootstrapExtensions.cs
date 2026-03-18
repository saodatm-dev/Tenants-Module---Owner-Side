using Core.Application.Abstractions.Security;
using Core.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// Encryption service DI extensions.
/// </summary>
public static class SecurityBootstrapExtensions
{
    /// <summary>
    /// Adds encryption services: KhKhDoumiEncryptor (IStringEncryptor) and BinaryEncryptor (IBinaryEncryptor).
    /// </summary>
    public static IServiceCollection AddEncryption(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<EncryptionOptions>(configuration.GetSection("Security:Encryption"));
        services.AddSingleton<IStringEncryptor, KhKhDoumiEncryptor>();
        services.AddSingleton<IBinaryEncryptor, BinaryEncryptor>();

        return services;
    }
}
