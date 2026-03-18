using Document.Contract.ContractTemplates.Commands;
using FluentValidation;

namespace Document.Application.Features.ContractTemplates.Commands.Create;

public sealed class CreateContractTemplateCommandValidator : AbstractValidator<CreateContractTemplateCommand>
{
    public CreateContractTemplateCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is required.");

        RuleFor(x => x.Page)
            .NotNull().WithMessage("Page configuration is required.");

        RuleFor(x => x.Theme)
            .NotNull().WithMessage("Theme configuration is required.");

        RuleFor(x => x.Bodies)
            .NotNull().WithMessage("Bodies are required.");

        RuleFor(x => x.Scope)
            .IsInEnum().WithMessage("Invalid scope value.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid category value.");
    }
}
