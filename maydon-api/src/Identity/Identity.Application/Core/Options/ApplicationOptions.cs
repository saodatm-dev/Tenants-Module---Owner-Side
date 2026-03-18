namespace Identity.Application.Core.Options;

public sealed record ApplicationOptions
{
    public required string AccountKey { get; init; }
    public required string InvitationKey { get; init; }
    public required string UserStateKey { get; init; }
    public required ushort UserStateExpiredTimeInMinutes { get; init; }
    public required ushort RefreshTokenExpiredTimeInDays { get; init; }

    public required string CookieName { get; init; }
    public required ushort CookieExpirationInMinutes { get; init; }  // in minutes
}
