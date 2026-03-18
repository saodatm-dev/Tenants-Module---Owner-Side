using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RealEstateTypes.Create;

internal sealed class CreateRealEstateTypeCommandValidator : AbstractValidator<CreateRealEstateTypeCommand>
{
	public CreateRealEstateTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Names)
			.Must(item => item.All(n => !string.IsNullOrWhiteSpace(n.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateTypeCommand.Names)).Description);

		RuleFor(item => item.Descriptions)
			.Must(item => item.All(n => !string.IsNullOrWhiteSpace(n.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateTypeCommand.Descriptions)).Description);
	}
}
