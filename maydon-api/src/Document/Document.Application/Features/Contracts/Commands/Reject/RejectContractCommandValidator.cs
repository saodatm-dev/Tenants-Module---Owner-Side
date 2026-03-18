using Document.Contract.Contracts.Commands;
using FluentValidation;

namespace Document.Application.Features.Contracts.Commands.Reject;

public sealed class RejectContractCommandValidator : AbstractValidator<RejectContractCommand>
{
    private static readonly HashSet<string> AllowedParties = ["owner", "client"];

    public RejectContractCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("ContractId is required");

        RuleFor(x => x.Party)
            .NotEmpty()
            .WithMessage("Party is required")
            .Must(p => AllowedParties.Contains(p.ToLowerInvariant()))
            .When(x => !string.IsNullOrEmpty(x.Party))
            .WithMessage("Party must be 'owner' or 'client'");

        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .When(x => x.Reason is not null)
            .WithMessage("Rejection reason must not exceed 1000 characters");
    }
}
