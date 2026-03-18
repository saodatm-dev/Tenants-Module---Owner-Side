using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Infrastructure.Authentication;
using Identity.Infrastructure.Cryptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Identity.Infrastructure.Extensions;

internal static class AuthenticationExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddAuthenticationInternal()
		{
			services.TryAddSingleton<ITokenProvider, TokenProvider>();
			services.TryAddSingleton<IPasswordHasher, PasswordHasher>();
			services.TryAddSingleton<ICryptor, Cryptor>();

			return services;
		}
	}
}
