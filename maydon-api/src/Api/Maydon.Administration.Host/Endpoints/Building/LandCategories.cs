using Building.Application.LandCategories.Create;
using Building.Application.LandCategories.Get;
using Building.Application.LandCategories.GetById;
using Building.Application.LandCategories.Remove;
using Building.Application.LandCategories.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class LandCategories : IEndpoint
{
	string IEndpoint.GroupName => LandCategoryPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetLandCategoriesQuery query,
			IQueryHandler<GetLandCategoriesQuery, PagedList<GetLandCategoriesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<PagedList<GetLandCategoriesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetLandCategoryByIdQuery, GetLandCategoryByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetLandCategoryByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<GetLandCategoryByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateLandCategoryCommand command,
			ICommandHandler<CreateLandCategoryCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(LandCategoryPermissions.PermissionLandCategoryCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateLandCategoryCommand command,
			ICommandHandler<UpdateLandCategoryCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(LandCategoryPermissions.PermissionLandCategoryUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveLandCategoryCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveLandCategoryCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(LandCategoryPermissions.PermissionLandCategoryRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
