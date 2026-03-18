using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Versioning;

namespace Core.Application.Abstractions.Repositories;

/// <summary>
/// Repository interface for accessing entity version history from cold storage (MongoDB).
/// </summary>
public interface IVersionRepository
{
	Task SaveVersionAsync(EntityVersionChangedEvent @event, CancellationToken cancellationToken = default);
	Task<PagedList<VersionMetadata>> GetHistoryAsync(Guid entityId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
	Task<string?> GetVersionDataAsync(Guid entityId, int versionNumber, CancellationToken cancellationToken = default);
}
