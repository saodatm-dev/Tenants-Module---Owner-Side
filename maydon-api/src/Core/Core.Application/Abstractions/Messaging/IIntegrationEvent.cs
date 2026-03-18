namespace Core.Application.Abstractions.Messaging;

public interface IIntegrationEvent
{
	Guid EventId { get; }
	DateTime OccurredOn { get; }
}
