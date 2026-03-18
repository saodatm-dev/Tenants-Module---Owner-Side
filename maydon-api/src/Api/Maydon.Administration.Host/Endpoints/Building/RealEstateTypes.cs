using Building.Application.RealEstateTypes.Create;
using Building.Application.RealEstateTypes.Get;
using Building.Application.RealEstateTypes.GetById;
using Building.Application.RealEstateTypes.Remove;
using Building.Application.RealEstateTypes.Update;
using Core.Application.Abstractions.Messaging;
using Core.Infrastructure.Extensions;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class RealEstateTypes : IEndpoint
{
	string IEndpoint.GroupName => RealEstateTypePermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetRealEstateTypesQuery query,
			IQueryHandler<GetRealEstateTypesQuery, IEnumerable<GetRealEstateTypesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<IEnumerable<GetRealEstateTypesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetRealEstateTypeByIdQuery, GetRealEstateTypeByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetRealEstateTypeByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RealEstateTypePermissions.PermissionRealEstateTypeUpdate.PermissionName)
			.Produces<GetRealEstateTypeByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest)
			.AddEndpointFilter<AntiforgeryFilter>();

		app.MapPost("/", async (
			CreateRealEstateTypeCommand command,
			ICommandHandler<CreateRealEstateTypeCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RealEstateTypePermissions.PermissionRealEstateTypeCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateRealEstateTypeCommand command,
			ICommandHandler<UpdateRealEstateTypeCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(RealEstateTypePermissions.PermissionRealEstateTypeUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveRealEstateTypeCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveRealEstateTypeCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(RealEstateTypePermissions.PermissionRealEstateTypeRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
