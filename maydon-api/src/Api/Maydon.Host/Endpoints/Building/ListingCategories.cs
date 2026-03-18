using Building.Application.ListingCategories.Get;
using Building.Application.ListingCategories.GetById;
using Building.Domain;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Building;

internal sealed class ListingCategories : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetListingCategoriesQuery query,
			IQueryHandler<GetListingCategoriesQuery, PagedList<GetListingCategoriesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<PagedList<GetListingCategoriesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetListingCategoriesByIdQuery, GetListingCategoriesByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetListingCategoriesByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<GetListingCategoriesByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
