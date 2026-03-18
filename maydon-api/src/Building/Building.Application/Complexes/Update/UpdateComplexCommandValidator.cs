using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Complexes.Update;

internal sealed class UpdateComplexCommandValidator : AbstractValidator<UpdateComplexCommand>
{
	public UpdateComplexCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateComplexCommand.Id)).Description);

		RuleFor(item => item.RegionId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateComplexCommand.RegionId)).Description);

		RuleFor(item => item.DistrictId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateComplexCommand.DistrictId)).Description);

		RuleFor(item => item.Name)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateComplexCommand.Name)).Description)
			.MaximumLength(200)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateComplexCommand.Name), 200).Description);

		RuleFor(item => item.Address)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Address))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateComplexCommand.Address), 500).Description);
	}
}
