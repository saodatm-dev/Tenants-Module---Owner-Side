using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RealEstateTypes.Update;

internal sealed class UpdateRealEstateTypeCommandValidator : AbstractValidator<UpdateRealEstateTypeCommand>
{
	public UpdateRealEstateTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateTypeCommand.Id)).Description);

		RuleFor(item => item.Names)
			.Must(item => item.All(n => !string.IsNullOrWhiteSpace(n.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateTypeCommand.Names)).Description);

		RuleFor(item => item.Descriptions)
			.Must(item => item.All(n => !string.IsNullOrWhiteSpace(n.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRealEstateTypeCommand.Descriptions)).Description);
	}
}
