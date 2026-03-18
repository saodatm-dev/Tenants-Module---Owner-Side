using Building.Application.Listings.Create;
using Building.Application.Listings.Get;
using Building.Application.Listings.GetById;
using Building.Application.Listings.GetMainPage;
using Building.Application.Listings.My;
using Building.Application.Listings.Remove;
using Building.Application.Listings.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Building;

namespace Maydon.Host.Endpoints.Building;

internal sealed class Listings : IEndpoint
{
	string IEndpoint.GroupName => ListingPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetListingsQuery query,
			IQueryHandler<GetListingsQuery, PagedList<GetListingsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<PagedList<GetListingsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/main-page", async (
			IQueryHandler<GetMainPageListingsQuery, IEnumerable<GetMainPageListingsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetMainPageListingsQuery(), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<IEnumerable<GetMainPageListingsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);


		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetListingByIdQuery, GetListingByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetListingByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<GetListingByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/my", async (
			[AsParameters] GetMyListingsQuery query,
			IQueryHandler<GetMyListingsQuery, PagedList<GetMyListingsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetMyListingsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateListingCommand command,
			ICommandHandler<CreateListingCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingPermissions.PermissionListingCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateListingCommand command,
			ICommandHandler<UpdateListingCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingPermissions.PermissionListingUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveListingCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveListingCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(ListingPermissions.PermissionListingRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
