using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Buildings.Update;

internal sealed class UpdateBuildingCommandValidator : AbstractValidator<UpdateBuildingCommand>
{
	private const short MinimumLatitudeValue = -90;
	private const short MaximumLatitudeValue = 90;

	private const short MinimumLongitudeValue = -180;
	private const short MaximumLongitudeValue = 180;
	public UpdateBuildingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.Id)).Description);

		RuleFor(item => item.Number)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.Number)).Description)
			.MaximumLength(50)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateBuildingCommand.Number), 50).Description);

		RuleFor(item => item.Address)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Address))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateBuildingCommand.Address), 500).Description);

		RuleFor(item => item.RegionId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.RegionId)).Description);

		RuleFor(item => item.DistrictId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.DistrictId)).Description);

		RuleFor(item => item.ComplexId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.ComplexId)).Description);

		RuleFor(item => item.TotalSquare)
			.Must(item => item != null ? item > 0 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.TotalSquare)).Description);

		RuleFor(item => item.FloorsCount)
			.Must(item => item != null ? item > 0 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.FloorsCount)).Description);

		RuleFor(item => item.Latitude)
			.Must(item => item != null ? MinimumLatitudeValue <= item && item <= MaximumLatitudeValue : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.Latitude)).Description);

		RuleFor(item => item.Longitude)
			.Must(item => item != null ? MinimumLongitudeValue <= item && item <= MaximumLongitudeValue : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBuildingCommand.Longitude)).Description);
	}
}
