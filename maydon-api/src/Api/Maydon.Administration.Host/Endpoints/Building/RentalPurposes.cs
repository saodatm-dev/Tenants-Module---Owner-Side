using Building.Application.RentalPurposes.Create;
using Building.Application.RentalPurposes.Get;
using Building.Application.RentalPurposes.Remove;
using Building.Application.RentalPurposes.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class RentalPurposes : IEndpoint
{
	string IEndpoint.GroupName => RentalPurposePermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetRentalPurposesQuery query,
			IQueryHandler<GetRentalPurposesQuery, PagedList<GetRentalPurposesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RentalPurposePermissions.PermissionRentalPurposeList.PermissionName)
			.Produces<PagedList<GetRentalPurposesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateRentalPurposeCommand command,
			ICommandHandler<CreateRentalPurposeCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RentalPurposePermissions.PermissionRentalPurposeCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateRentalPurposeCommand command,
			ICommandHandler<UpdateRentalPurposeCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RentalPurposePermissions.PermissionRentalPurposeUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveRentalPurposeCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveRentalPurposeCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(RentalPurposePermissions.PermissionRentalPurposeRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
