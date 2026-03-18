using Building.Application.MeterTypes.Create;
using Building.Application.MeterTypes.Get;
using Building.Application.MeterTypes.GetById;
using Building.Application.MeterTypes.Remove;
using Building.Application.MeterTypes.Update;
using Core.Application.Abstractions.Messaging;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class MeterTypes : IEndpoint
{
	string IEndpoint.GroupName => MeterTypePermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetMeterTypesQuery query,
			IQueryHandler<GetMeterTypesQuery, IEnumerable<GetMeterTypesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<IEnumerable<GetMeterTypesResponse>>()
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetMeterTypeByIdQuery, GetMeterTypeByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetMeterTypeByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<GetMeterTypeByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateMeterTypeCommand command,
			ICommandHandler<CreateMeterTypeCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(MeterTypePermissions.PermissionMeterTypeCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateMeterTypeCommand command,
			ICommandHandler<UpdateMeterTypeCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(MeterTypePermissions.PermissionMeterTypeUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveMeterTypeCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveMeterTypeCommand(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(MeterTypePermissions.PermissionMeterTypeRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
