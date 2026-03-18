using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RoomTypes.Create;

internal sealed class CreateRoomTypeCommandValidator : AbstractValidator<CreateRoomTypeCommand>
{
	public CreateRoomTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRoomTypeCommand.Translates)).Description);
}
