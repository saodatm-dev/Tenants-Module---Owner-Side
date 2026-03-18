using Building.Application.Complexes.Create;
using Building.Application.Complexes.Get;
using Building.Application.Complexes.GetById;
using Building.Application.Complexes.Remove;
using Building.Application.Complexes.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Infrastructure.Extensions;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;


internal sealed class Complexes : IEndpoint
{
	string IEndpoint.GroupName => ComplexPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetComplexesQuery query,
			IQueryHandler<GetComplexesQuery, PagedList<GetComplexesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetComplexesResponse>>()
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetComplexByIdQuery, GetComplexByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetComplexByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<GetComplexByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest)
			.AddEndpointFilter<AntiforgeryFilter>()
			.AllowAnonymous();

		app.MapPost("/", async (
			CreateComplexCommand command,
			ICommandHandler<CreateComplexCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ComplexPermissions.PermissionComplexCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateComplexCommand command,
			ICommandHandler<UpdateComplexCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(ComplexPermissions.PermissionComplexUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveComplexCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveComplexCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(ComplexPermissions.PermissionComplexRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
