using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Rooms.Get;

internal sealed class GetRoomsQueryValidator : AbstractValidator<GetRoomsQuery>
{
	public GetRoomsQueryValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.RealEstateId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(GetRoomsQuery.RealEstateId)).Description);
	}
}
