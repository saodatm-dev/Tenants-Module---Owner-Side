namespace Building.Infrastructure.Options;

public sealed record MinioApiOptions
{
	public const string SectionName = "MinioApi";

	public string BaseUrl { get; init; } = string.Empty;
	public string DefaultBucket { get; init; } = "appartments";
}
