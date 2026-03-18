using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.RealEstates.Accept;

internal sealed class AcceptModerationRealEstateCommandValidator : AbstractValidator<AcceptModerationRealEstateCommand>
{
	public AcceptModerationRealEstateCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(AcceptModerationRealEstateCommand.Id)).Description);
	}
}
