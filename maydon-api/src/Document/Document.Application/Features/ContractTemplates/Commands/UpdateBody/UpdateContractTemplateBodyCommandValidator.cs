using Document.Contract.ContractTemplates.Commands;
using FluentValidation;

namespace Document.Application.Features.ContractTemplates.Commands.UpdateBody;

public sealed class UpdateContractTemplateBodyCommandValidator : AbstractValidator<UpdateContractTemplateBodyCommand>
{
    private static readonly HashSet<string> AllowedLanguages = ["ru", "uz", "en"];

    public UpdateContractTemplateBodyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Template ID is required.");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language code is required.")
            .Must(l => AllowedLanguages.Contains(l))
            .WithMessage("Language must be one of: ru, uz, en.");

        RuleFor(x => x.Blocks)
            .NotNull().WithMessage("Blocks are required.");
    }
}
