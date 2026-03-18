using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Floors.Get;

internal sealed class GetFloorsQueryValidator : AbstractValidator<GetFloorsQuery>
{
	public GetFloorsQueryValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item)
			.Must(item => (item.BuildingId is not null && item.BuildingId != Guid.Empty) ||
				(item.RealEstateId is not null && item.RealEstateId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue("Parent").Description);
	}
}
