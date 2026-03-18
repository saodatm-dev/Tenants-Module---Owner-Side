using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Application.Abstractions.Services;

/// <summary>
/// Service responsible for capturing entity state and publishing version events.
/// </summary>
public interface IVersioningService
{
	Task PublishVersionSnapshotAsync<TEntity>(TEntity entity, EntityChangeType changeType, CancellationToken cancellationToken = default)
		where TEntity : IVersionedEntity;

	Task PublishDeletionSnapshotAsync(Guid entityId, string entityType, int versionNumber, CancellationToken cancellationToken = default);
}
