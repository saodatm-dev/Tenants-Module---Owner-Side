namespace Core.Application.Abstractions.Services.Minio;

public sealed record FileManagerResponse(
	byte[] Source,
	string Name,
	string Type,
	long lenght);
