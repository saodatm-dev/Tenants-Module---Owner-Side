using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RoomTypes.Update;

internal sealed class UpdateRoomTypeCommandValidator : AbstractValidator<UpdateRoomTypeCommand>
{
	public UpdateRoomTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoomTypeCommand.Id)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoomTypeCommand.Translates)).Description);
	}
}
