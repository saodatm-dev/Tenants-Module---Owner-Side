using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.MeterTariffs.Remove;

internal sealed class RemoveMeterTariffCommandValidator : AbstractValidator<RemoveMeterTariffCommand>
{
	public RemoveMeterTariffCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveMeterTariffCommand.Id)).Description);
	}
}
