using Document.Contract.Contracts.Commands;
using FluentValidation;

namespace Document.Application.Features.Contracts.Commands.Create;

public sealed class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.TemplateId)
            .NotEmpty()
            .WithMessage("TemplateId is required");

        RuleFor(x => x.LeaseId)
            .NotEmpty()
            .WithMessage("LeaseId is required");

        RuleFor(x => x.Language)
            .NotEmpty()
            .MaximumLength(5)
            .WithMessage("Language must be specified (e.g. 'uz', 'ru', 'en')");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Contract body is required");
    }
}
