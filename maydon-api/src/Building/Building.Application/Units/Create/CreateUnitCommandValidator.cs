using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Units.Create;

internal sealed class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
	public CreateUnitCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(x => x.RealEstateId)
			.NotEqual(Guid.Empty)
			.When(item => item.RealEstateId is not null)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateUnitCommand.RealEstateId)).Description);

		RuleFor(x => x.RealEstateTypeId)
			.NotEqual(Guid.Empty)
			.When(item => item.RealEstateTypeId is not null)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateUnitCommand.RealEstateTypeId)).Description);

		RuleFor(x => x.FloorId)
			.NotEqual(Guid.Empty)
			.When(x => x.FloorId.HasValue)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateUnitCommand.FloorId)).Description);

		RuleFor(x => x.RoomId)
			.NotEqual(Guid.Empty)
			.When(x => x.FloorId.HasValue)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateUnitCommand.RoomId)).Description);

		RuleFor(x => x.RenovationId)
			.NotEqual(Guid.Empty)
			.When(x => x.FloorId.HasValue)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateUnitCommand.RenovationId)).Description);

		RuleFor(x => x.FloorNumber)
			.Must(item => item >= -5 && item < 100)
			.When(x => x.FloorNumber.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateUnitCommand.FloorNumber)).Description);

		RuleFor(x => x.TotalArea)
			.GreaterThan(0)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateUnitCommand.TotalArea)).Description);

		RuleFor(x => x.CeilingHeight)
			.GreaterThan(0)
			.LessThanOrEqualTo(10)
			.When(x => x.CeilingHeight.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateUnitCommand.CeilingHeight)).Description);

		RuleFor(x => x.RoomNumber)
			.MaximumLength(50)
			.When(x => !string.IsNullOrEmpty(x.RoomNumber))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateUnitCommand.RoomNumber), 50).Description);

		RuleFor(x => x.Plan)
			.MaximumLength(200)
			.When(x => !string.IsNullOrEmpty(x.Plan))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateUnitCommand.Plan), 200).Description);
	}
}
