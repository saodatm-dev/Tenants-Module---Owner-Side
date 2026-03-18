using Document.Contract.Contracts.Commands;
using FluentValidation;

namespace Document.Application.Features.Contracts.Commands.SyncFromDidox;

public sealed class SyncContractFromDidoxCommandValidator : AbstractValidator<SyncContractFromDidoxCommand>
{
    public SyncContractFromDidoxCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("ContractId is required");
    }
}
