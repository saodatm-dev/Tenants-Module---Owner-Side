using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Identity.Application.Invitations.Accept;
using Identity.Application.Invitations.Cancel;
using Identity.Application.Invitations.Create;
using Identity.Application.Invitations.Get;
using Identity.Application.Invitations.GetById;
using Identity.Application.Invitations.Reject;
using Identity.Application.Invitations.Remove;
using Identity.Application.Invitations.Update;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Identity;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Invitations : IEndpoint
{
	string IEndpoint.GroupName => InvitationPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetInvitationsQuery query,
			IQueryHandler<GetInvitationsQuery, PagedList<GetInvitationsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(InvitationPermissions.PermissionInvitationList.PermissionName)
			.Produces<PagedList<GetInvitationsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetInvitationByIdQuery, GetInvitationByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetInvitationByIdQuery(id), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(InvitationPermissions.PermissionInvitationGetById.PermissionName)
			.Produces<GetInvitationByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateInvitationCommand command,
			ICommandHandler<CreateInvitationCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(InvitationPermissions.PermissionInvitationCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateInvitationCommand command,
			ICommandHandler<UpdateInvitationCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(InvitationPermissions.PermissionInvitationUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveInvitationCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveInvitationCommand(id), cancellationToken);

			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(InvitationPermissions.PermissionInvitationRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/accept/{id:guid}", async (
			Guid id,
			ICommandHandler<AcceptInvitationCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new AcceptInvitationCommand(id), cancellationToken);

			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(InvitationPermissions.PermissionInvitationUpdate.PermissionName)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/cancel/{id:guid}", async (
			Guid id,
			ICommandHandler<CancelInvitationCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new CancelInvitationCommand(id), cancellationToken);

			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(InvitationPermissions.PermissionInvitationUpdate.PermissionName)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/reject", async (
			RejectInvitationCommand command,
			ICommandHandler<RejectInvitationCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(InvitationPermissions.PermissionInvitationUpdate.PermissionName)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest);
	}
}
