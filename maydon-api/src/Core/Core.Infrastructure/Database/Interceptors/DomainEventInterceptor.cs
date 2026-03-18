using Core.Domain.Entities;
using Core.Domain.Events;
using Core.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Core.Infrastructure.Database.Interceptors;

public sealed class DomainEventInterceptor(IDomainEventPublisher publisher) : SaveChangesInterceptor
{
	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
	{
		if (eventData.Context is not null)
			await PrePublishDomainEventsAsync(eventData.Context);

		var saveChangesResult = await base.SavingChangesAsync(eventData, result, cancellationToken);

		if (eventData.Context is not null)
			await PostPublishDomainEventsAsync(eventData.Context);

		return saveChangesResult;
	}

	private Task PrePublishDomainEventsAsync(DbContext dbContext)
	{
		var entities = GetDomainEventEntities(dbContext);

		var domainEvents = entities
		   .SelectMany(entity =>
		   {
			   var preEvents = entity.DomainEvents
				   .Where(item => item.GetType().IsAssignableTo(typeof(IPrePublishDomainEvent)));

			   var postDomainEvents = entity.DomainEvents
				   .Where(item => item.GetType().IsAssignableTo(typeof(IPostPublishDomainEvent)));
			   if (postDomainEvents is null || !postDomainEvents.Any())
				   entity.ClearDomainEvents();

			   return preEvents;
		   })
		   .ToList();

		return publisher.PublishAsync(domainEvents);
	}

	private Task PostPublishDomainEventsAsync(DbContext context)
	{
		var entities = GetDomainEventEntities(context);

		var domainEvents = entities
		   .SelectMany(entity =>
		   {
			   var postEvents = entity.DomainEvents
				   .Where(item => item.GetType().IsAssignableTo(typeof(IPostPublishDomainEvent)));

			   entity.ClearDomainEvents();

			   return postEvents;
		   })
		   .ToList();

		return publisher.PublishAsync(domainEvents);
	}

	private static IEnumerable<IHasDomainEvents> GetDomainEventEntities(DbContext dbContext) =>
		dbContext.ChangeTracker
			.Entries<Entity>()
			.Select(e => (IHasDomainEvents)e.Entity)
			.Concat(
				dbContext.ChangeTracker
					.Entries<AggregateRoot<Guid>>()
					.Select(e => (IHasDomainEvents)e.Entity))
			.Distinct();
}
