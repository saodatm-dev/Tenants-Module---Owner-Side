using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RealEstates.UploadImage;

internal sealed class UploadRealEstateImageCommandValidator : AbstractValidator<UploadRealEstateImageCommand>
{
	public UploadRealEstateImageCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(x => x.RealEstateId)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UploadRealEstateImageCommand.RealEstateId)).Description);

		RuleFor(x => x.Images)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UploadRealEstateImageCommand.Images)).Description);
	}
}
