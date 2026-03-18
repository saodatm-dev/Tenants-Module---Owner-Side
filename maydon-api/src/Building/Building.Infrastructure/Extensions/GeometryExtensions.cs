using Building.Application.Core.Abstractions.Services;
using Building.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Building.Infrastructure.Extensions;

internal static class GeometryExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddGeometryService()
		{
			services.TryAddSingleton<IGeometryService, GeometryService>();
			return services;
		}
	}
}
