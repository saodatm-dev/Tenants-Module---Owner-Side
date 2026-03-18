using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Districts.Create;

internal sealed class CreateDistrictCommandValidator : AbstractValidator<CreateDistrictCommand>
{
	public CreateDistrictCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.RegionId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateDistrictCommand.RegionId)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateDistrictCommand.Translates)).Description);
	}
}
