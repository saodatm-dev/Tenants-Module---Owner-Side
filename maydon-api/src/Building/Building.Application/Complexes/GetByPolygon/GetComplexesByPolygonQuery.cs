using Core.Application.Abstractions.Messaging;

namespace Building.Application.Complexes.GetByPolygon;

public sealed record GetComplexesByPolygonQuery(
	double TopLeftLatitude,                                                                     // north
	double TopLeftLongitude,                                                                    // east
	double BottomRightLatitude,                                                                 // south
	double BottomRightLongitude) : IQuery<IEnumerable<GetComplexesByPolygonResponse>>;           // west
