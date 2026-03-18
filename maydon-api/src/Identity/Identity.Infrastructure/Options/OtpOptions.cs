namespace Identity.Infrastructure.Options;

public sealed record OtpOptions
{
	public required string Uri { get; init; }
	public required string ClientId { get; init; }
	public required string Secret { get; init; }
	public required string UzContent { get; init; }
	public required string RuContent { get; init; }
	public required string EnContent { get; init; }
}
