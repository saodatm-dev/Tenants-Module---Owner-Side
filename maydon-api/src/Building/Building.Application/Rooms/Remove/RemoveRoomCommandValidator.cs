using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Rooms.Remove;

internal sealed class RemoveRoomCommandValidator : AbstractValidator<RemoveRoomCommand>
{
	public RemoveRoomCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveRoomCommand.Id)).Description);
}
