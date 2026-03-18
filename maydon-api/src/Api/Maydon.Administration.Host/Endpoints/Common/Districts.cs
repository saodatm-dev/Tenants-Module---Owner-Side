using Common.Application.Districts.Create;
using Common.Application.Districts.Get;
using Common.Application.Districts.GetById;
using Common.Application.Districts.Remove;
using Common.Application.Districts.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Common;

namespace Maydon.Administration.Host.Endpoints.Common;

internal sealed class Districts : IEndpoint
{
	string IEndpoint.GroupName => DistrictPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetDistrictsQuery query,
			IQueryHandler<GetDistrictsQuery, PagedList<GetDistrictsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetDistrictsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetDistrictByIdQuery, GetDistrictByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetDistrictByIdQuery(id), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<GetDistrictByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateDistrictCommand command,
			ICommandHandler<CreateDistrictCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(DistrictPermissions.PermissionDistrictCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateDistrictCommand command,
			ICommandHandler<UpdateDistrictCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(DistrictPermissions.PermissionDistrictUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveDistrictCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveDistrictCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(DistrictPermissions.PermissionDistrictRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
