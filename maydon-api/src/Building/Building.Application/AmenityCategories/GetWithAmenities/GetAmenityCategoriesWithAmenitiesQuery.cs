using Core.Application.Abstractions.Messaging;

namespace Building.Application.AmenityCategories.GetWithAmenities;

public sealed record GetAmenityCategoriesWithAmenitiesQuery() : IQuery<IEnumerable<GetAmenityCategoriesWithAmenitiesResponse>>;
