using Building.Application.AmenityCategories.Get;
using Building.Application.AmenityCategories.GetWithAmenities;
using Building.Domain;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Building;

internal sealed class AmenityCategories : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetAmenityCategoriesQuery query,
			IQueryHandler<GetAmenityCategoriesQuery, PagedList<GetAmenityCategoriesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetAmenityCategoriesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/amenities-with-categories", async (
			IQueryHandler<GetAmenityCategoriesWithAmenitiesQuery, IEnumerable<GetAmenityCategoriesWithAmenitiesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetAmenityCategoriesWithAmenitiesQuery(), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<IEnumerable<GetAmenityCategoriesWithAmenitiesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
