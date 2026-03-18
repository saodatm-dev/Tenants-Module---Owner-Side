using Building.Application.Core.Validators;
using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Complexes.GetByPolygon;

internal sealed class GetComplexesByPolygonQueryValidator : AbstractValidator<GetComplexesByPolygonQuery>
{
	public GetComplexesByPolygonQueryValidator(ISharedViewLocalizer sharedViewLocalizer)
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
