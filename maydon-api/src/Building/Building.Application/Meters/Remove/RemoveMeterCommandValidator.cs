using Core.Application.Resources;
using Core.Domain.Providers;
using FluentValidation;

namespace Building.Application.Meters.Remove;

internal sealed class RemoveMeterCommandValidator : AbstractValidator<RemoveMeterCommand>
{
	public RemoveMeterCommandValidator(
		ISharedViewLocalizer sharedViewLocalizer,
		IDateTimeProvider dateTimeProvider)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveMeterCommand.Id)).Description);
	}
}
