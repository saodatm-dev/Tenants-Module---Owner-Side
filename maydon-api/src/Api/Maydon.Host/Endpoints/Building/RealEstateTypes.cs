using Building.Application.RealEstateTypes.Get;
using Core.Application.Abstractions.Messaging;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Building;

namespace Maydon.Host.Endpoints.Building;

internal sealed class RealEstateTypes : IEndpoint
{
	string IEndpoint.GroupName => RealEstatePermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetRealEstateTypesQuery query,
			IQueryHandler<GetRealEstateTypesQuery, IEnumerable<GetRealEstateTypesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<IEnumerable<GetRealEstateTypesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
