namespace Core.Application.Abstractions.Services.Minio;

public sealed record FileManagerRequest(
	string BucketName,
	string Name,
	string Type,
	long Length,
	Stream Stream);
