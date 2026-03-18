using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Rooms.Create;

internal sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
	public CreateRoomCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.RealEstateId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRoomCommand.RealEstateId)).Description);

		RuleFor(item => item.RoomTypeId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRoomCommand.RoomTypeId)).Description);

		RuleFor(item => item.TotalArea)
			.NotEmpty()
			.GreaterThan(0)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRoomCommand.TotalArea)).Description);
	}
}
