using Building.Application.Dashboard;
using Core.Application.Abstractions.Messaging;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Building;

internal sealed class Dashboard : IEndpoint
{
	string IEndpoint.GroupName => "buildings";

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/statistics", async (
			IQueryHandler<DashboardStatisticsQuery, DashboardStatisticsResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new DashboardStatisticsQuery(), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<DashboardStatisticsResponse>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
