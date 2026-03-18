namespace Identity.Infrastructure.Options;

public sealed record OneIdOptions
{
	public required string Uri { get; init; }
	public required string ClientId { get; init; }
	public required string ClientSecret { get; init; }
}
