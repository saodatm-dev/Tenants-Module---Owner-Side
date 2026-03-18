using Document.Contract.Contracts.Commands;
using FluentValidation;

namespace Document.Application.Features.Contracts.Commands.ExportDidox;

public sealed class ExportContractToDidoxCommandValidator : AbstractValidator<ExportContractToDidoxCommand>
{
    public ExportContractToDidoxCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("ContractId is required");
    }
}
