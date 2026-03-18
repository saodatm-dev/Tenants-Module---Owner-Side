namespace Core.Infrastructure.Options;

internal sealed record CoreOptions
{
	public required string MinIOSecret { get; init; }
}
