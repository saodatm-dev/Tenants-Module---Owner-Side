namespace Core.Application.Abstractions.Services.Minio;

public sealed record FileStreamResponse(
	Stream Stream,
	string Name,
	string ContentType,
	long Size) : IAsyncDisposable
{
	public async ValueTask DisposeAsync()
	{
		await Stream.DisposeAsync();
	}
}
