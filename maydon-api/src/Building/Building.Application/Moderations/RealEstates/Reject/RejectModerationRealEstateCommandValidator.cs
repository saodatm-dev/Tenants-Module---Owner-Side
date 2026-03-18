using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.RealEstates.Reject;

internal sealed class RejectModerationRealEstateCommandValidator : AbstractValidator<RejectModerationRealEstateCommand>
{
	public RejectModerationRealEstateCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RejectModerationRealEstateCommand.Id)).Description);
	}
}
