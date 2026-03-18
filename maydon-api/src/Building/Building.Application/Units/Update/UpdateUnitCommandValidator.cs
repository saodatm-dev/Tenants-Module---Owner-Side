using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Units.Update;

internal sealed class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
{
	public UpdateUnitCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateUnitCommand.Id)).Description);

		RuleFor(x => x.RealEstateId)
			.NotEqual(Guid.Empty)
			.When(item => item.RealEstateId is not null)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateUnitCommand.RealEstateId)).Description);

		RuleFor(x => x.RealEstateTypeId)
			.NotEqual(Guid.Empty)
			.When(item => item.RealEstateTypeId is not null)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateUnitCommand.RealEstateTypeId)).Description);

		RuleFor(x => x.FloorId)
			.NotEqual(Guid.Empty)
			.When(x => x.FloorId.HasValue)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateUnitCommand.FloorId)).Description);

		RuleFor(x => x.RoomId)
			.NotEqual(Guid.Empty)
			.When(x => x.FloorId.HasValue)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateUnitCommand.RoomId)).Description);

		RuleFor(x => x.RenovationId)
			.NotEqual(Guid.Empty)
			.When(x => x.FloorId.HasValue)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateUnitCommand.RenovationId)).Description);

		RuleFor(x => x.FloorNumber)
			.Must(item => item >= -5 && item < 100)
			.When(x => x.FloorNumber.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateUnitCommand.FloorNumber)).Description);

		RuleFor(x => x.TotalArea)
			.GreaterThan(0)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateUnitCommand.TotalArea)).Description);

		RuleFor(x => x.CeilingHeight)
			.GreaterThan(0)
			.LessThanOrEqualTo(10)
			.When(x => x.CeilingHeight.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateUnitCommand.CeilingHeight)).Description);

		RuleFor(x => x.RoomNumber)
			.MaximumLength(50)
			.When(x => !string.IsNullOrEmpty(x.RoomNumber))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateUnitCommand.RoomNumber), 50).Description);

		RuleFor(x => x.Plan)
			.MaximumLength(200)
			.When(x => !string.IsNullOrEmpty(x.Plan))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateUnitCommand.Plan), 200).Description);
	}
}
