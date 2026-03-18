using Identity.Application.Core.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application;

public static class DependencyInjection
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddIdentityApplication()
		{
			services.AddOptions<ApplicationOptions>()
				.BindConfiguration(nameof(ApplicationOptions))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			//services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
			//	.AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
			//		.AsImplementedInterfaces()
			//		.WithScopedLifetime()
			//	.AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
			//		.AsImplementedInterfaces()
			//		.WithScopedLifetime()
			//	.AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
			//		.AsImplementedInterfaces()
			//		.WithScopedLifetime());

			//services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
			//services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

			//services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
			//services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
			//services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

			//services.Decorate(typeof(ICommandHandler<,>), typeof(TransactionDecorator.CommandHandler<,>));
			//services.Decorate(typeof(ICommandHandler<>), typeof(TransactionDecorator.CommandBaseHandler<>));

			//services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
			//	.AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
			//	.AsImplementedInterfaces()
			//	.WithScopedLifetime());

			//services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);



			return services;
		}
	}
}
