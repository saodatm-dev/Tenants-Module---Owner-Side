using Building.Application.MeterTariffs.Get;
using Building.Application.MeterTariffs.GetById;
using Building.Domain;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Building;

internal sealed class MeterTariffs : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetMeterTariffsQuery query,
			IQueryHandler<GetMeterTariffsQuery, PagedList<GetMeterTariffsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetMeterTariffsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetMeterTariffByIdQuery, GetMeterTariffByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetMeterTariffByIdQuery(id), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<GetMeterTariffByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
