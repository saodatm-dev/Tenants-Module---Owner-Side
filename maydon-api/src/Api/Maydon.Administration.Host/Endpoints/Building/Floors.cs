using Building.Application.Floors.Create;
using Building.Application.Floors.Get;
using Building.Application.Floors.GetById;
using Building.Application.Floors.Remove;
using Building.Application.Floors.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class Floors : IEndpoint
{
	string IEndpoint.GroupName => FloorPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetFloorsQuery query,
			IQueryHandler<GetFloorsQuery, PagedList<GetFloorsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetFloorsResponse>>()
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetFloorByIdQuery, GetFloorByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetFloorByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<GetFloorByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();

		app.MapPost("/", async (
			CreateFloorCommand command,
			ICommandHandler<CreateFloorCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(FloorPermissions.PermissionFloorCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateFloorCommand command,
			ICommandHandler<UpdateFloorCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(FloorPermissions.PermissionFloorUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveFloorCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveFloorCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(FloorPermissions.PermissionFloorRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
