using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RoomTypes.Remove;

internal sealed class RemoveRoomTypeCommandValidator : AbstractValidator<RemoveRoomTypeCommand>
{
	public RemoveRoomTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveRoomTypeCommand.Id)).Description);
}
