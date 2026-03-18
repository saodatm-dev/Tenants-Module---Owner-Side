using Building.Application.Core.Validators;
using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RealEstates.Update;

internal sealed class UpdateRealEstateCommandValidator : AbstractValidator<UpdateRealEstateCommand>
{
	public UpdateRealEstateCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateCommand.Id)).Description);

		RuleFor(item => item.RealEstateTypeId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateCommand.RealEstateTypeId)).Description);

		RuleFor(item => item.RenovationId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateCommand.RenovationId)).Description);

		RuleFor(item => item.TotalArea)
			.TotalArea(sharedViewLocalizer);

		RuleFor(item => item.BuildingId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateCommand.BuildingId)).Description);

		RuleFor(item => item.FloorId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateCommand.FloorId)).Description);

		RuleFor(item => item.FloorNumber)
			.FloorNumber(sharedViewLocalizer);

		RuleFor(item => item.LivingArea)
			.LivingArea(sharedViewLocalizer);

		RuleFor(item => item.CeilingHeight)
			.Must(item => item != null ? item > 0 && item <= 10 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateCommand.CeilingHeight)).Description);

		RuleFor(item => item.TotalFloors)
			.FloorNumber(sharedViewLocalizer);

		RuleFor(item => item.RoomsCount)
			.RoomsCount(sharedViewLocalizer);

		RuleFor(item => item.Rooms)
			.Rooms(sharedViewLocalizer);

		RuleFor(item => item.RegionId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateCommand.RegionId)).Description);

		RuleFor(item => item.DistrictId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateCommand.DistrictId)).Description);

		RuleFor(item => item.Latitude)
			.NullableLatitude(sharedViewLocalizer);

		RuleFor(item => item.Longitude)
			.NullableLongitude(sharedViewLocalizer);
	}
}
