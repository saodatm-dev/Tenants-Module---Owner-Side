using Document.Contract.ContractTemplates.Commands;
using FluentValidation;

namespace Document.Application.Features.ContractTemplates.Commands.Update;

public sealed class UpdateContractTemplateCommandValidator : AbstractValidator<UpdateContractTemplateCommand>
{
    public UpdateContractTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Template ID is required.");

        When(x => x.Code is not null, () =>
        {
            RuleFor(x => x.Code!)
                .NotEmpty().WithMessage("Code cannot be empty.")
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");
        });

        When(x => x.Scope.HasValue, () =>
        {
            RuleFor(x => x.Scope!.Value)
                .IsInEnum().WithMessage("Invalid scope value.");
        });

        When(x => x.Category.HasValue, () =>
        {
            RuleFor(x => x.Category!.Value)
                .IsInEnum().WithMessage("Invalid category value.");
        });
    }
}
