using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Identity.Application.Users.GetAll;
using Identity.Application.Users.GetById;
using Identity.Application.Users.Profile;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Identity;

namespace Maydon.Administration.Host.Endpoints.Identity;

internal sealed class Users : IEndpoint
{
	string IEndpoint.GroupName => UserPermissions.GroupName;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetAllUsersQuery query,
			IQueryHandler<GetAllUsersQuery, PagedList<GetAllUsersResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(UserPermissions.PermissionUserList.PermissionName)
			.Produces<PagedList<GetAllUsersResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
			Guid id,
			IQueryHandler<GetUserByIdQuery, GetUserByIdResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetUserByIdQuery(id), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(UserPermissions.PermissionUserGetById.PermissionName)
			.Produces<GetUserByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/profile", async (
			IQueryHandler<ProfileQuery, ProfileResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new ProfileQuery(), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<ProfileResponse>()
			.Produces(StatusCodes.Status400BadRequest);


	}
}
