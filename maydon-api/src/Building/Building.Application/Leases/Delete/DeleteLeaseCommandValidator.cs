using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Leases.Delete;

internal sealed class DeleteLeaseCommandValidator : AbstractValidator<DeleteLeaseCommand>
{
    public DeleteLeaseCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
        RuleFor(item => item.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(DeleteLeaseCommand.Id)).Description);
}
