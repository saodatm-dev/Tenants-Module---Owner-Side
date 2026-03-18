using Building.Application.Units.Create;
using Building.Application.Units.Get;
using Building.Application.Units.GetById;
using Building.Application.Units.Remove;
using Building.Application.Units.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Infrastructure.Extensions;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Building;

namespace Maydon.Host.Endpoints.Building;

internal sealed class RealEstateUnits : IEndpoint
{
	string IEndpoint.GroupName => RealEstateUnitPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetRealEstateUnitsQuery query,
			IQueryHandler<GetRealEstateUnitsQuery, PagedList<GetRealEstateUnitsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<PagedList<GetRealEstateUnitsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetRealEstateUnitByIdQuery, GetRealEstateUnitByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetRealEstateUnitByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<GetRealEstateUnitByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest)
			.AddEndpointFilter<AntiforgeryFilter>();

		app.MapPost("/", async (
			CreateUnitCommand command,
			ICommandHandler<CreateUnitCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RealEstateUnitPermissions.PermissionRealEstateUnitCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateUnitCommand command,
			ICommandHandler<UpdateUnitCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RealEstateUnitPermissions.PermissionRealEstateUnitUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveUnitCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveUnitCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(RealEstateUnitPermissions.PermissionRealEstateUnitUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
