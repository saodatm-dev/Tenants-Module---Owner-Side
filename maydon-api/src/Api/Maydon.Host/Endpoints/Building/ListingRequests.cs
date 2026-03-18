using Building.Application.ListingRequests.Accept;
using Building.Application.ListingRequests.Cancel;
using Building.Application.ListingRequests.Create;
using Building.Application.ListingRequests.Get;
using Building.Application.ListingRequests.GetOwnerListingRequests;
using Building.Application.ListingRequests.Receive;
using Building.Application.ListingRequests.Reject;
using Building.Application.ListingRequests.Remove;
using Building.Application.ListingRequests.Send;
using Building.Application.ListingRequests.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Building;

namespace Maydon.Host.Endpoints.Building;

internal sealed class ListingRequests : IEndpoint
{
	string IEndpoint.GroupName => ListingRequestPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/client", async (
			[AsParameters] GetClientListingRequestsQuery query,
			IQueryHandler<GetClientListingRequestsQuery, PagedList<GetClientListingRequestsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetClientListingRequestsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/owner", async (
			[AsParameters] GetOwnerListingRequestsQuery query,
			IQueryHandler<GetOwnerListingRequestsQuery, PagedList<GetOwnerListingRequestsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetOwnerListingRequestsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateListingRequestCommand command,
			ICommandHandler<CreateListingRequestCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingRequestPermissions.PermissionListingRequestCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateListingRequestCommand command,
			ICommandHandler<UpdateListingRequestCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingRequestPermissions.PermissionListingRequestUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveListingRequestCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveListingRequestCommand(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingRequestPermissions.PermissionListingRequestRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/{id:guid}/send", async (
			Guid id,
			ICommandHandler<SendListingRequestCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new SendListingRequestCommand(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingRequestPermissions.PermissionListingRequestSend.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/{id:guid}/receive", async (
			Guid id,
			ICommandHandler<ReceiveListingRequestCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new ReceiveListingRequestCommand(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingRequestPermissions.PermissionListingRequestReceive.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/{id:guid}/accept", async (
			Guid id,
			ICommandHandler<AcceptListingRequestCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new AcceptListingRequestCommand(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingRequestPermissions.PermissionListingRequestAccept.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/{id:guid}/reject", async (
			Guid id,
			RejectListingRequestCommand command,
			ICommandHandler<RejectListingRequestCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command with { Id = id }, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingRequestPermissions.PermissionListingRequestReject.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/{id:guid}/cancel", async (
			Guid id,
			ICommandHandler<CancelListingRequestCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new CancelListingRequestCommand(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingRequestPermissions.PermissionListingRequestCancel.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
