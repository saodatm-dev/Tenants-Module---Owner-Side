using FluentValidation;
using Core.Application.Resources;
using Didox.Application.Contracts.DidoxAccounts.Commands;

namespace Didox.Application.Features.DidoxAccounts.Commands.Update;

public class UpdateDidoxAccountCommandValidator : AbstractValidator<UpdateDidoxAccountCommand>
{
    public UpdateDidoxAccountCommandValidator(
        ISharedViewLocalizer sharedViewLocalizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(sharedViewLocalizer
                .IsRequired(nameof(UpdateDidoxAccountCommand.Id))
                .Description);

        RuleFor(x => x.Login)
            .MaximumLength(30)
            .WithMessage(sharedViewLocalizer
                .MaximumLength(nameof(UpdateDidoxAccountCommand.Login), 30)
                .Description)
            .When(x => !string.IsNullOrWhiteSpace(x.Login));

        RuleFor(x => x.Password)
            .MaximumLength(8)
            .WithMessage(sharedViewLocalizer
                .MaximumLength(nameof(UpdateDidoxAccountCommand.Password), 8)
                .Description)
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }
}
