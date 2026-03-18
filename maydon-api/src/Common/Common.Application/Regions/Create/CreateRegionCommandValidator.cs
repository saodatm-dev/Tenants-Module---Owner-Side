using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Regions.Create;

internal sealed class CreateRegionCommandValidator : AbstractValidator<CreateRegionCommand>
{
	public CreateRegionCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRegionCommand.Translates)).Description);
}
