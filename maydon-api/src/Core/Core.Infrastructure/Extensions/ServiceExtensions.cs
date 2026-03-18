using Core.Application.Abstractions.Notifications;
using Core.Application.Abstractions.Services;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Providers;
using Core.Infrastructure.Authentication;
using Core.Infrastructure.Services;
using Core.Infrastructure.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Infrastructure.Extensions;

internal static class ServiceExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddServicesInternal()
		{
			services.TryAddSingleton<IFileManager, FileManager>();
			services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

			services.TryAddScoped<IBackgroundUserContext, BackgroundUserContext>();

			services.AddSignalR();
			services.TryAddScoped<IEntityChangeNotificationHandler, SignalREntityChangeNotificationHandler>();
			services.TryAddSingleton<IFileUrlResolver, FileUrlResolver>();

			return services;
		}
	}
}
