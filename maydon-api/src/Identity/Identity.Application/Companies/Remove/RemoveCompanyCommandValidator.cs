using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Companies.Remove;

internal sealed class RemoveCompanyCommandValidator : AbstractValidator<RemoveCompanyCommand>
{
	public RemoveCompanyCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveCompanyCommand.Id)).Description);
	}
}
