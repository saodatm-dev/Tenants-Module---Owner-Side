using Core.Application.Abstractions.Messaging;

namespace Building.Application.Listings.GetByPolygon;

public sealed record GetListingsByPolygonQuery(
	double TopLeftLatitude,                                                                     // north
	double TopLeftLongitude,                                                                    // east
	double BottomRightLatitude,                                                                 // south
	double BottomRightLongitude) : IQuery<IEnumerable<GetListingsByPolygonResponse>>;           // west

