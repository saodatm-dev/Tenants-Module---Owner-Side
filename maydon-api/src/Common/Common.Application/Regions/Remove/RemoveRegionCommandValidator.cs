using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Regions.Remove;

internal sealed class RemoveRegionCommandValidator : AbstractValidator<RemoveRegionCommand>
{
	public RemoveRegionCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveRegionCommand.Id)).Description);
}
