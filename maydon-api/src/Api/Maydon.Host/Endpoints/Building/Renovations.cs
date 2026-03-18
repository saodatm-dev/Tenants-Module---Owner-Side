using Building.Application.Renovations.Get;
using Building.Domain;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Building;

internal sealed class Renovations : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetRenovationsQuery query,
			IQueryHandler<GetRenovationsQuery, PagedList<GetRenovationsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetRenovationsResponse>>()
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();
	}
}
