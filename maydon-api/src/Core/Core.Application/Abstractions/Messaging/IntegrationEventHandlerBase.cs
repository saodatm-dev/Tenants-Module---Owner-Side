using Microsoft.Extensions.Logging;

namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Base class for integration event handlers providing common infrastructure.
/// </summary>
public abstract class IntegrationEventHandlerBase<TEvent> : IIntegrationEventHandler<TEvent>
	where TEvent : class, IIntegrationEvent
{
	protected readonly ILogger Logger;

	protected IntegrationEventHandlerBase(ILogger logger)
	{
		Logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public Task Handle(TEvent @event, CancellationToken cancellationToken = default) =>
		HandleCoreAsync(@event, cancellationToken);

	protected abstract Task HandleCoreAsync(TEvent @event, CancellationToken cancellationToken);
}

/// <summary>
/// Base class for idempotent event handlers. Prevents duplicate processing.
/// </summary>
public abstract class IdempotentIntegrationEventHandlerBase<TEvent> : IntegrationEventHandlerBase<TEvent>
	where TEvent : class, IIntegrationEvent
{
	private readonly IIdempotencyStore _idempotencyStore;

	protected IdempotentIntegrationEventHandlerBase(
		IIdempotencyStore idempotencyStore,
		ILogger logger) : base(logger)
	{
		_idempotencyStore = idempotencyStore ?? throw new ArgumentNullException(nameof(idempotencyStore));
	}

	protected sealed override async Task HandleCoreAsync(TEvent @event, CancellationToken cancellationToken)
	{
		if (await _idempotencyStore.WasProcessedAsync(@event.EventId))
		{
			Logger.LogDebug("Event {EventType} with ID {EventId} already processed, skipping",
				typeof(TEvent).Name, @event.EventId);
			return;
		}

		await HandleIdempotentAsync(@event, cancellationToken);
		await _idempotencyStore.MarkAsProcessedAsync(@event.EventId, TimeSpan.FromHours(24));
	}

	protected abstract Task HandleIdempotentAsync(TEvent @event, CancellationToken cancellationToken);
}

/// <summary>
/// Idempotency store abstraction
/// </summary>
public interface IIdempotencyStore
{
	Task<bool> WasProcessedAsync(Guid eventId);
	Task MarkAsProcessedAsync(Guid eventId, TimeSpan expiry);
}
