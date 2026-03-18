namespace Maydon.Host.Endpoints.Identity.TestsBox;

public sealed record MyIdOptions
{
    public required string BaseUrl { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}