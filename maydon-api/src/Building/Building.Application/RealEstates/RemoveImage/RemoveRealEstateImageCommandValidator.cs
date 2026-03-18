using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RealEstates.RemoveImage;

internal sealed class RemoveRealEstateImageCommandValidator : AbstractValidator<RemoveRealEstateImageCommand>
{
	public RemoveRealEstateImageCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(RemoveRealEstateImageCommand.Id)).Description);
	}
}
