using FluentValidation;
using Core.Application.Resources;
using Didox.Application.Contracts.DidoxAccounts.Commands;

namespace Didox.Application.Features.DidoxAccounts.Commands.Create;

public class CreateDidoxAccountCommandValidator : AbstractValidator<CreateDidoxAccountCommand>
{
    public CreateDidoxAccountCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .NotNull()
            .WithName(sharedViewLocalizer.IsRequired(nameof(CreateDidoxAccountCommand.Login)).Description);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .WithName(sharedViewLocalizer.IsRequired(nameof(CreateDidoxAccountCommand.Password)).Description);
        
        RuleFor(x => x.Login)
            .MaximumLength(30)
            .WithMessage(sharedViewLocalizer.MaximumLength(nameof(CreateDidoxAccountCommand.Login),30).Description);
        
        RuleFor(x => x.Password)
            .MaximumLength(30)
            .WithMessage(sharedViewLocalizer.MaximumLength(nameof(CreateDidoxAccountCommand.Password),30).Description);
    }
}

