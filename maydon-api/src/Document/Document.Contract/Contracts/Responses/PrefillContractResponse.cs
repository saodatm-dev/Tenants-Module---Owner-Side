namespace Document.Contract.Contracts.Responses;

/// <summary>
/// Response for template pre-fill — returns resolved JSONB body + manual field list.
/// </summary>
public sealed record PrefillContractResponse
{
    /// <summary>
    /// The template body with auto-resolved placeholders substituted.
    /// </summary>
    public required string ResolvedBody { get; init; }

    /// <summary>
    /// Flat dictionary of all resolved placeholder values for the UI to display.
    /// </summary>
    public required IReadOnlyDictionary<string, object?> PlaceholderValues { get; init; }
}
