using Building.Application.MeterTariffs.Create;
using Building.Application.MeterTariffs.Get;
using Building.Application.MeterTariffs.GetById;
using Building.Application.MeterTariffs.Remove;
using Building.Application.MeterTariffs.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class MeterTariffs : IEndpoint
{
	string IEndpoint.GroupName => MeterTariffPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetMeterTariffsQuery query,
			IQueryHandler<GetMeterTariffsQuery, PagedList<GetMeterTariffsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetMeterTariffsResponse>>()
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetMeterTariffByIdQuery, GetMeterTariffByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetMeterTariffByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<GetMeterTariffByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateMeterTariffCommand command,
			ICommandHandler<CreateMeterTariffCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(MeterTariffPermissions.PermissionMeterTariffCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateMeterTariffCommand command,
			ICommandHandler<UpdateMeterTariffCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(MeterTariffPermissions.PermissionMeterTariffUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveMeterTariffCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveMeterTariffCommand(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(MeterTariffPermissions.PermissionMeterTariffRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
