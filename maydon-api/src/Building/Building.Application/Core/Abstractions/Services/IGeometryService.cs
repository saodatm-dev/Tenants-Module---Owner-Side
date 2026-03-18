using NetTopologySuite.Geometries;

namespace Building.Application.Core.Abstractions.Services;

public interface IGeometryService
{
	Point CreatePoint(double latitude, double longitude);
	Polygon CreatePolygon(double startLatitude, double startLongitude, double endLatitude, double endLongitude);
	Polygon CreatePolygon(Coordinate[] coordinates);
}
