using Building.Application.MeterTypes.Get;
using Building.Application.MeterTypes.GetById;
using Building.Domain;
using Core.Application.Abstractions.Messaging;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Building;

internal sealed class MeterTypes : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetMeterTypesQuery query,
			IQueryHandler<GetMeterTypesQuery, IEnumerable<GetMeterTypesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<IEnumerable<GetMeterTypesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetMeterTypeByIdQuery, GetMeterTypeByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetMeterTypeByIdQuery(id), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<GetMeterTypeByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
