using Building.Application.Categories.Get;
using Building.Application.Categories.GetById;
using Building.Domain;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Building;

internal sealed class Categories : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetCategoriesQuery query,
			IQueryHandler<GetCategoriesQuery, PagedList<GetCategoriesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<PagedList<GetCategoriesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetCategoryByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<GetCategoryByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
