using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Categories.Remove;

internal sealed class RemoveCategoryCommandValidator : AbstractValidator<RemoveCategoryCommand>
{
	public RemoveCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveCategoryCommand.Id)).Description);
}
