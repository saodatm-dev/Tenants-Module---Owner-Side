namespace Document.Contract.Gateways;

/// <summary>
/// Lightweight read-only projection of a user for document operations.
/// </summary>
public sealed record UserSnapshot(
    Guid Id,
    string? FirstName,
    string? LastName,
    string? MiddleName,
    string? Tin,
    string? Pinfl,
    string? Address)
{
    public string FullName =>
        $"{FirstName ?? string.Empty} {LastName ?? string.Empty} {MiddleName ?? string.Empty}".Trim();
}
