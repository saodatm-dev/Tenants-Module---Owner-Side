using Building.Application.Moderations.Get;
using Building.Application.Moderations.Listings.Accept;
using Building.Application.Moderations.Listings.Block;
using Building.Application.Moderations.Listings.Get;
using Building.Application.Moderations.Listings.GetById;
using Building.Application.Moderations.Listings.Reject;
using Building.Application.Moderations.RealEstates.Accept;
using Building.Application.Moderations.RealEstates.Block;
using Building.Application.Moderations.RealEstates.Get;
using Building.Application.Moderations.RealEstates.GetById;
using Building.Application.Moderations.RealEstates.Reject;
using Building.Application.Moderations.RealEstateUnits.Accept;
using Building.Application.Moderations.RealEstateUnits.Block;
using Building.Application.Moderations.RealEstateUnits.GetById;
using Building.Application.Moderations.RealEstateUnits.Reject;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class Moderations : IEndpoint
{
	string IEndpoint.GroupName => ModerationPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		#region Listings

		app.MapGet("/listings", async (
			[AsParameters] GetModerationListingsQuery query,
			IQueryHandler<GetModerationListingsQuery, PagedList<GetModerationListingsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationListingGet.PermissionName)
			.Produces<PagedList<GetModerationListingsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/listings/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetModerationListingByIdQuery, GetModerationListingByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetModerationListingByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationListingGetById.PermissionName)
			.Produces<GetModerationListingByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/listings/accept", async (
			AcceptModerationListingCommand command,
			ICommandHandler<AcceptModerationListingCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationListingAccept.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/listings/reject", async (
			RejectModerationListingCommand command,
			ICommandHandler<RejectModerationListingCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationListingReject.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/listings/block", async (
			BlockModerationListingCommand command,
			ICommandHandler<BlockModerationListingCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationListingBlock.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		#endregion

		#region RealEstates

		app.MapGet("/realestates", async (
			[AsParameters] GetModerationRealEstatesQuery query,
			IQueryHandler<GetModerationRealEstatesQuery, PagedList<GetModerationRealEstatesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateGet.PermissionName)
			.Produces<PagedList<GetModerationRealEstatesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/realestates/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetModerationRealEstateByIdQuery, GetModerationRealEstateByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetModerationRealEstateByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateGetById.PermissionName)
			.Produces<GetModerationRealEstateByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/realestates/accept", async (
			AcceptModerationRealEstateCommand command,
			ICommandHandler<AcceptModerationRealEstateCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateAccept.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/realestates/reject", async (
			RejectModerationRealEstateCommand command,
			ICommandHandler<RejectModerationRealEstateCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateReject.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/realestates/block", async (
			BlockModerationRealEstateCommand command,
			ICommandHandler<BlockModerationRealEstateCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateBlock.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		#endregion

		#region Real estate units

		app.MapGet("/realestateunits", async (
			[AsParameters] GetModerationRealEstateUnitsQuery query,
			IQueryHandler<GetModerationRealEstateUnitsQuery, PagedList<GetModerationRealEstateUnitsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateUnitGet.PermissionName)
			.Produces<PagedList<GetModerationRealEstateUnitsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/realestateunits/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetModerationRealEstateUnitByIdQuery, GetModerationRealEstateUnitByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetModerationRealEstateUnitByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateUnitGetById.PermissionName)
			.Produces<GetModerationRealEstateUnitByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/realestateunits/accept", async (
			AcceptModerationRealEstateUnitCommand command,
			ICommandHandler<AcceptModerationRealEstateUnitCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateUnitAccept.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/realestateunits/reject", async (
			RejectModerationRealEstateUnitCommand command,
			ICommandHandler<RejectModerationRealEstateUnitCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateUnitReject.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/realestateunits/block", async (
			BlockModerationRealEstateUnitCommand command,
			ICommandHandler<BlockModerationRealEstateUnitCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ModerationPermissions.PermissionModerationRealEstateUnitBlock.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		#endregion
	}
}
