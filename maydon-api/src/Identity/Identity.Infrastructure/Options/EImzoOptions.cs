namespace Identity.Infrastructure.Options;

public sealed record EImzoOptions
{
	//public required string RegisteredHost { get; init; }
	//public required string XRealIP { get; init; }
	public required string Host { get; init; }
	//public required int Port { get; init; }
	//public required bool IsHttps { get; init; } = true;
}
