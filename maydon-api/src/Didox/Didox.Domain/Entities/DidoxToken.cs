namespace Didox.Domain.Entities;

/// <summary>
/// Represents a Didox authentication token for owner
/// </summary>
public sealed class DidoxToken
{
    /// <summary>
    /// Unique identifier for the token record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Didox authentication token
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Owner/User ID who owns this token
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// UTC timestamp when token was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public long ExpiresIn { get; set; }
}
