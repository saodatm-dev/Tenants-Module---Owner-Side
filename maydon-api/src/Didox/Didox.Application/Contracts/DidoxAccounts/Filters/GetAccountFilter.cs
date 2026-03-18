namespace Didox.Application.Contracts.DidoxAccounts.Filters;

public record GetAccountFilter
{
    public Guid? OwnerId { get; set; }
    public string? Tin { get; set; }
    public string? Pinfl { get; set; }
};
