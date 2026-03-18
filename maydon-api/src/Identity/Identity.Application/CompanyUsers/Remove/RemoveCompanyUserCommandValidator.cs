using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.CompanyUsers.Remove;

internal sealed class RemoveCompanyUserCommandValidator : AbstractValidator<RemoveCompanyUserCommand>
{
	public RemoveCompanyUserCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.UserId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveCompanyUserCommand.UserId)).Description);
	}
}
