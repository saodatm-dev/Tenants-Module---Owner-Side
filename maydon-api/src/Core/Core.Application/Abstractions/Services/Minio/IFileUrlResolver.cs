namespace Core.Application.Abstractions.Services.Minio;

public interface IFileUrlResolver
{
	Task<string?> ResolveUrlAsync(string? key, CancellationToken cancellationToken = default);
	Task<IEnumerable<string>> ResolveUrlsAsync(IEnumerable<string>? keys, CancellationToken cancellationToken = default);
	Task<string?[]> ResolveManyAsync(IReadOnlyList<string?> keys, CancellationToken cancellationToken = default);
}

