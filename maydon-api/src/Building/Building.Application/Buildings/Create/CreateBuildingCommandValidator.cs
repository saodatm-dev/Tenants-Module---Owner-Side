using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Buildings.Create;

internal sealed class CreateBuildingCommandValidator : AbstractValidator<CreateBuildingCommand>
{
	private const short MinimumLatitudeValue = -90;
	private const short MaximumLatitudeValue = 90;

	private const short MinimumLongitudeValue = -180;
	private const short MaximumLongitudeValue = 180;
	public CreateBuildingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Number)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBuildingCommand.Number)).Description)
			.MaximumLength(50)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateBuildingCommand.Number), 50).Description);

		RuleFor(item => item.Address)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Address))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateBuildingCommand.Address), 500).Description);

		RuleFor(item => item.RegionId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBuildingCommand.RegionId)).Description);

		RuleFor(item => item.DistrictId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBuildingCommand.DistrictId)).Description);

		RuleFor(item => item.ComplexId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBuildingCommand.ComplexId)).Description);

		RuleFor(item => item.TotalArea)
			.Must(item => item != null ? item > 0 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBuildingCommand.TotalArea)).Description);

		RuleFor(item => item.FloorsCount)
			.Must(item => item != null ? item > 0 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBuildingCommand.FloorsCount)).Description);

		RuleFor(item => item.Latitude)
			.Must(item => item != null ? MinimumLatitudeValue <= item && item <= MaximumLatitudeValue : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBuildingCommand.Latitude)).Description);

		RuleFor(item => item.Longitude)
			.Must(item => item != null ? MinimumLongitudeValue <= item && item <= MaximumLongitudeValue : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBuildingCommand.Longitude)).Description);
	}
}
