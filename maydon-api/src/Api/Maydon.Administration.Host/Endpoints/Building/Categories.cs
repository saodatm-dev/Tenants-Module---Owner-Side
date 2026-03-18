using Building.Application.Categories.Create;
using Building.Application.Categories.Get;
using Building.Application.Categories.GetById;
using Building.Application.Categories.Remove;
using Building.Application.Categories.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;

namespace Maydon.Administration.Host.Endpoints.Building;


internal sealed class Categories : IEndpoint
{
	string IEndpoint.GroupName => CategoryPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetCategoriesQuery query,
			IQueryHandler<GetCategoriesQuery, PagedList<GetCategoriesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<PagedList<GetCategoriesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetCategoryByIdQuery(id), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<GetCategoryByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/", async (
			CreateCategoryCommand command,
			ICommandHandler<CreateCategoryCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(CategoryPermissions.PermissionCategoryCreate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/", async (
			UpdateCategoryCommand command,
			ICommandHandler<UpdateCategoryCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(CategoryPermissions.PermissionCategoryUpdate.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveCategoryCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveCategoryCommand(id), cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(CategoryPermissions.PermissionCategoryRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
