using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Floors.Update;

internal sealed class UpdateFloorCommandValidator : AbstractValidator<UpdateFloorCommand>
{
	private const short MinimumFloorsNumber = -3;
	private const short MaximumFloorsNumber = 100;
	public UpdateFloorCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateFloorCommand.Id)).Description);

		RuleFor(item => item)
			.Must(item => (item.BuildingId is not null && item.BuildingId != Guid.Empty) ||
				(item.RealEstateId is not null && item.RealEstateId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue("Parent").Description);

		RuleFor(item => item.Number)
			.Must(item => MinimumFloorsNumber <= item && item <= MaximumFloorsNumber)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateFloorCommand.Number)).Description);
	}
}
