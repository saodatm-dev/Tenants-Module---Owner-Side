using Document.Contract.ContractTemplates.Commands;
using FluentValidation;

namespace Document.Application.Features.ContractTemplates.Commands.Delete;

public sealed class DeleteContractTemplateCommandValidator : AbstractValidator<DeleteContractTemplateCommand>
{
    public DeleteContractTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Template ID is required.");
    }
}
