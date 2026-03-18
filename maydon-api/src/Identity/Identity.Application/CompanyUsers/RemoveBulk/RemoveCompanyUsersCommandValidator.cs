using Core.Application.Resources;
using FluentValidation;
using Identity.Application.CompanyUsers.RemoveBulk;

namespace Identity.Domain.CompanyUsers.RemoveBulk;

internal sealed class RemoveCompanyUsersCommandValidator : AbstractValidator<RemoveCompanyUsersCommand>
{
	public RemoveCompanyUsersCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.UserIds)
			.NotEmpty()
			.Must(command => command.All(userId => userId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveCompanyUsersCommand.UserIds)).Description);
	}
}
