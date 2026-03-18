namespace Document.Contract.ContractTemplates.Responses;

/// <summary>
/// Grouped placeholder catalog for frontend block editor sidebar.
/// </summary>
public sealed record PlaceholderCatalogResponse
{
    public required IReadOnlyList<PlaceholderGroup> Groups { get; init; }
}

public sealed record PlaceholderGroup
{
    public required string Category { get; init; }
    public required string Label { get; init; }
    public required IReadOnlyList<PlaceholderItem> Items { get; init; }
}

public sealed record PlaceholderItem
{
    public required string Key { get; init; }
    public required string Label { get; init; }
    public required string Type { get; init; }
    public required bool AutoResolvable { get; init; }
}
