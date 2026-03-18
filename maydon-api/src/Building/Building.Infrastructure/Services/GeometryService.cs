using Building.Application.Core.Abstractions.Services;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Building.Infrastructure.Services;

internal sealed class GeometryService : IGeometryService
{
	private readonly GeometryFactory factory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);

	public Point CreatePoint(double latitude, double longitude) =>
		factory.CreatePoint(new Coordinate(latitude, longitude));
	//factory.CreatePoint(new Coordinate(longitude, latitude));

	public Polygon CreatePolygon(
		double startLatitude,
		double startLongitude,
		double endLatitude,
		double endLongitude)
	{
		var ne = new Coordinate(startLatitude, startLongitude);
		var nw = new Coordinate(startLatitude, endLongitude);
		var se = new Coordinate(endLatitude, startLongitude);
		var sw = new Coordinate(endLatitude, endLongitude);

		//var ne = new Coordinate(startLongitude, startLatitude);
		//var nw = new Coordinate(startLongitude, endLatitude);
		//var se = new Coordinate(endLongitude, startLatitude);
		//var sw = new Coordinate(endLongitude, endLatitude);

		var linearRing = new LinearRing(new[] { ne, nw, sw, se, ne }); // Must close the ring

		return factory.CreatePolygon(linearRing);
	}

	public Polygon CreatePolygon(Coordinate[] coordinates)
	{
		// Ensure polygon is closed (first point = last point)
		if (!coordinates[0].Equals(coordinates[^1]))
		{
			coordinates = [.. coordinates, coordinates[0]];
		}

		var ring = factory.CreateLinearRing(coordinates);
		return factory.CreatePolygon(ring);
	}

	public bool IsWithinDistance(Point point1, Point point2, double meters)
	{
		return point1.IsWithinDistance(point2, meters);
	}
}
