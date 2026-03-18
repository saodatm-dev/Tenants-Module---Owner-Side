using Building.Application.Core.Validators;
using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Listings.GetByPolygon;

internal sealed class GetListingsByPolygonQueryValidator : AbstractValidator<GetListingsByPolygonQuery>
{
	public GetListingsByPolygonQueryValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.TopLeftLatitude)
			.Latitude(sharedViewLocalizer);

		RuleFor(item => item.BottomRightLatitude)
			.Latitude(sharedViewLocalizer);

		RuleFor(item => item.TopLeftLongitude)
			.Longitude(sharedViewLocalizer);

		RuleFor(item => item.BottomRightLongitude)
			.Longitude(sharedViewLocalizer);
	}
}
