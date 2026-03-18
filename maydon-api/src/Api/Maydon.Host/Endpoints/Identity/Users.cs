using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Identity.Application.Users.Get;
using Identity.Application.Users.GetById;
using Identity.Application.Users.GetPermissions;
using Identity.Application.Users.Profile;
using Identity.Application.Users.UpdateProfile;
using Identity.Domain;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Users : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;
	private const string PermissionUserList = "users:list";
	private const string PermissionUserGetById = "users:get-by-id";
	//private const string PermissionUserCreate = "users:create";
	//private const string PermissionUserUpdate = "users:update";
	//private const string PermissionUserRemove = "users:remove";
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetUsersQuery query,
			IQueryHandler<GetUsersQuery, PagedList<GetUsersResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(PermissionUserList)
			.Produces<PagedList<GetUsersResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
		   Guid id,
		   IQueryHandler<GetUserByIdQuery, GetUserByIdResponse> handler,
		   CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetUserByIdQuery(id), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(PermissionUserGetById)
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

		app.MapGet("/permissions", async (
			IQueryHandler<GetPermissionsQuery, IEnumerable<GetPermissionsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetPermissionsQuery(), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<IEnumerable<GetPermissionsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/profile", async (
			UpdateProfileCommand command,
			ICommandHandler<UpdateProfileCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		//app.MapPost("/switch", async (
		//	HttpContext httpContext,
		//	ICommandHandler<SwitchAccountCommand, AuthenticationResponse> handler,
		//	CancellationToken cancellationToken) =>
		//{
		//	var result = await handler.Handle(new SwitchAccountCommand(), cancellationToken);
		//	if (result.IsSuccess)
		//		httpContext.SetCookieValue(result.Value.Token);

		//	return result.Match(Results.Ok, CustomResults.Problem);
		//})
		//	.Produces<AuthenticationResponse>()
		//	.Produces(StatusCodes.Status400BadRequest);
	}
}
