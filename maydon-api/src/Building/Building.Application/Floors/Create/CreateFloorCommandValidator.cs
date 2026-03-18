using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Floors.Create;

internal sealed class CreateFloorCommandValidator : AbstractValidator<CreateFloorCommand>
{
	private const short MinimumFloorsNumber = -3;
	private const short MaximumFloorsNumber = 100;
	public CreateFloorCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item)
			.Must(item => (item.BuildingId is not null && item.BuildingId != Guid.Empty) ||
				(item.RealEstateId is not null && item.RealEstateId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue("Parent").Description);

		RuleFor(item => item.Number)
			.Must(item => MinimumFloorsNumber <= item && item <= MaximumFloorsNumber)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateFloorCommand.Number)).Description);
	}
}
