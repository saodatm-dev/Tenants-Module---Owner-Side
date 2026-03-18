using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.MeterTypes.Remove;

internal sealed class RemoveMeterTypeCommandValidator : AbstractValidator<RemoveMeterTypeCommand>
{
	public RemoveMeterTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveMeterTypeCommand.Id)).Description);
	}
}
