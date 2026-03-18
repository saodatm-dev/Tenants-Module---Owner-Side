using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Languages.Remove;

internal sealed class RemoveLanguageCommandValidator : AbstractValidator<RemoveLanguageCommand>
{
	public RemoveLanguageCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveLanguageCommand.Id)).Description);
}
