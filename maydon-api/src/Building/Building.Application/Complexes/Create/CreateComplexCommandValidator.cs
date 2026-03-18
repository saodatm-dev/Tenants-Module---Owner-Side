using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Complexes.Create;

internal sealed class CreateComplexCommandValidator : AbstractValidator<CreateComplexCommand>
{
	public CreateComplexCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.RegionId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateComplexCommand.RegionId)).Description);

		RuleFor(item => item.DistrictId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateComplexCommand.DistrictId)).Description);

		RuleFor(item => item.Name)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateComplexCommand.Name)).Description)
			.MaximumLength(200)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateComplexCommand.Name), 200).Description);

		RuleFor(item => item.Address)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Address))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateComplexCommand.Address), 500).Description);
	}
}
