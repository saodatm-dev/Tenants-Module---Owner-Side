using Core.Application.Abstractions.Behaviors;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Events;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Application;

public static class DependencyInjection
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddCoreApplication(Type[] assemblies)
		{
			services.Scan(scan => scan.FromAssembliesOf(assemblies)
				.AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
					.AsImplementedInterfaces()
					.WithScopedLifetime()
				.AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
					.AsImplementedInterfaces()
					.WithScopedLifetime()
				.AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
					.AsImplementedInterfaces()
					.WithScopedLifetime());

			services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationDecorator.QueryHandler<,>));
			services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
			services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

			services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
			services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
			services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

			services.Scan(scan => scan.FromAssembliesOf(assemblies)
				.AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
				.AsImplementedInterfaces()
				.WithScopedLifetime());

			// Auto-register integration event handlers from all module assemblies
			services.Scan(scan => scan.FromAssembliesOf(assemblies)
				.AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventHandler<>)), publicOnly: false)
				.AsImplementedInterfaces()
				.WithScopedLifetime());

			foreach (var item in assemblies)
			{
				services.AddValidatorsFromAssembly(item.Assembly, includeInternalTypes: true);
			}

			services.TryAddTransient<ISharedViewLocalizer, ApplicationSharedViewLocalizer>();
			return services;
		}
	}
}
