using FluentValidation;
using Core.Application.Resources;
using Didox.Application.Contracts.DidoxAccounts.Commands;

namespace Didox.Application.Features.DidoxAccounts.Commands.Delete;

public class DeleteDidoxAccountCommandValidator : AbstractValidator<DeleteDidoxAccountCommand>
{
    public DeleteDidoxAccountCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(sharedViewLocalizer.IsRequired(nameof(DeleteDidoxAccountCommand.Id)).Description);
    }
}

