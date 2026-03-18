using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Regions.Update;

internal sealed class UpdateRegionCommandValidator : AbstractValidator<UpdateRegionCommand>
{
	public UpdateRegionCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRegionCommand.Id)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRegionCommand.Translates)).Description);
	}
}
