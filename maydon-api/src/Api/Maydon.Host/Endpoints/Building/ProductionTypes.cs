using Building.Application.ProductionTypes.Get;
using Building.Domain;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Building;

internal sealed class ProductionTypes : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetProductionTypesQuery query,
			IQueryHandler<GetProductionTypesQuery, PagedList<GetProductionTypesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetProductionTypesResponse>>()
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();
	}
}
