namespace Document.Contract.Contracts.Responses;

/// <summary>
/// Paginated response wrapper for contract listings.
/// </summary>
public sealed record PagedContractResponse(
    IReadOnlyList<ContractResponse> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}
