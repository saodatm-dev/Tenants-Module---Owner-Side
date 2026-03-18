using Core.Application.Abstractions.Services.Minio;

namespace Core.Infrastructure.Services;

internal sealed class FileUrlResolver(IFileManager fileManager) : IFileUrlResolver
{
	public async Task<string?> ResolveUrlAsync(string? key, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(key))
			return null;

		var result = await fileManager.GetPresignedUrlAsync(key, 3600, cancellationToken);
		return result.IsSuccess ? result.Value : null;
	}

	public async Task<IEnumerable<string>> ResolveUrlsAsync(
		IEnumerable<string>? keys, CancellationToken cancellationToken = default)
	{
		if (keys is null || !keys.Any())
			return [];

		var tasks = keys.Select(key => ResolveUrlAsync(key, cancellationToken));
		var results = await Task.WhenAll(tasks);
		return results.Where(url => url is not null)!;
	}

	public async Task<string?[]> ResolveManyAsync(
		IReadOnlyList<string?> keys, CancellationToken cancellationToken = default)
	{
		if (keys.Count == 0)
			return [];

		var tasks = keys.Select(key => ResolveUrlAsync(key, cancellationToken));
		return await Task.WhenAll(tasks);
	}
}

