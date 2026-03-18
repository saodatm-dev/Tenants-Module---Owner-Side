using Core.Application.Abstractions.Messaging;

namespace Core.Application.Abstractions.Jobs;

/// <summary>
/// Interface for versioning workflow processing.
/// </summary>
public interface IVersioningWorkflowProcessor
{
	Task ExecuteAsync(EntityVersionChangedEvent @event, CancellationToken cancellationToken);
}
