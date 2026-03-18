using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Rooms.Update;

internal sealed class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
	public UpdateRoomCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoomCommand.Id)).Description);

		RuleFor(item => item.RealEstateId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoomCommand.RealEstateId)).Description);

		RuleFor(item => item.RoomTypeId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoomCommand.RoomTypeId)).Description);

		RuleFor(item => item.TotalArea)
			.NotEmpty()
			.GreaterThan(0)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoomCommand.TotalArea)).Description);
	}
}
