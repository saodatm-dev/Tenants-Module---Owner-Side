using Building.Application.Buildings.Create;
using Building.Application.Buildings.Get;
using Building.Application.Buildings.GetById;
using Building.Application.Buildings.Remove;
using Building.Application.Buildings.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Infrastructure.Extensions;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Building;

namespace Maydon.Host.Endpoints.Building;

internal sealed class Buildings : IEndpoint
{
	string IEndpoint.GroupName => BuildingPermissions.GroupName;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetBuildingsQuery query,
			IQueryHandler<GetBuildingsQuery, PagedList<GetBuildingsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetBuildingsResponse>>()
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetBuildingByIdQuery, GetBuildingByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetBuildingByIdQuery(id), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<GetBuildingByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest)
			.AddEndpointFilter<AntiforgeryFilter>()
			.AllowAnonymous();

		app.MapPost("/", async (
			CreateBuildingCommand command,
			ICommandHandler<CreateBuildingCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(BuildingPermissions.PermissionBuildingCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateBuildingCommand command,
			ICommandHandler<UpdateBuildingCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(BuildingPermissions.PermissionBuildingUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveBuildingCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveBuildingCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(BuildingPermissions.PermissionBuildingRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
