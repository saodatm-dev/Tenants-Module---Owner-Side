using Building.Application.Renovations.Create;
using Building.Application.Renovations.Get;
using Building.Application.Renovations.GetById;
using Building.Application.Renovations.Remove;
using Building.Application.Renovations.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class Renovations : IEndpoint
{
	string IEndpoint.GroupName => RenovationPermissions.GroupName;

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
			.AllowAnonymous()
			.Produces<PagedList<GetRenovationsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetRenovationByIdQuery, GetRenovationByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetRenovationByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<GetRenovationByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateRenovationCommand command,
			ICommandHandler<CreateRenovationCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RenovationPermissions.PermissionRenovationCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateRenovationCommand command,
			ICommandHandler<UpdateRenovationCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RenovationPermissions.PermissionRenovationUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveRenovationCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveRenovationCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(RenovationPermissions.PermissionRenovationRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
