using Building.Application.Core.Validators;
using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RealEstates.Create;

internal sealed class CreateRealEstateCommandValidator : AbstractValidator<CreateRealEstateCommand>
{
	public CreateRealEstateCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.RealEstateTypeId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateCommand.RealEstateTypeId)).Description);

		RuleFor(item => item.RenovationId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateCommand.RenovationId)).Description);

		RuleFor(item => item.TotalArea)
			.TotalArea(sharedViewLocalizer);

		RuleFor(item => item.BuildingId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateCommand.BuildingId)).Description);

		RuleFor(item => item.FloorId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateCommand.FloorId)).Description);

		RuleFor(item => item.FloorNumber)
			.FloorNumber(sharedViewLocalizer);

		RuleFor(item => item.LivingArea)
			.LivingArea(sharedViewLocalizer);

		RuleFor(item => item.CeilingHeight)
			.Must(item => item != null ? item > 0 && item <= 10 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateCommand.CeilingHeight)).Description);

		RuleFor(item => item.TotalFloors)
			.FloorNumber(sharedViewLocalizer);

		RuleFor(item => item.RoomsCount)
			.RoomsCount(sharedViewLocalizer);

		RuleFor(item => item.Rooms)
			.Rooms(sharedViewLocalizer);

		RuleFor(item => item.RegionId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateCommand.RegionId)).Description);

		RuleFor(item => item.DistrictId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateCommand.DistrictId)).Description);

		RuleFor(item => item.Latitude)
			.NullableLatitude(sharedViewLocalizer);

		RuleFor(item => item.Longitude)
			.NullableLongitude(sharedViewLocalizer);
	}
}
