namespace Core.Infrastructure.Options;

public sealed record JwtOptions
{
	public required string Key { get; init; }
	public required string Issuer { get; init; }
	public required string Audience { get; init; }
	public required int ExpirationInMinutes { get; init; }
}
