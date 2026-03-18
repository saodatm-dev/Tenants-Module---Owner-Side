using Core.Application.Abstractions.Messaging;
using Identity.Application.Accounts.Activate;
using Identity.Application.Accounts.Change;
using Identity.Application.Accounts.CreateClient;
using Identity.Application.Accounts.CreateOwner;
using Identity.Application.Accounts.Deactivate;
using Identity.Application.Accounts.My;
using Identity.Application.Authentication;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Identity;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Accounts : IEndpoint
{
	string IEndpoint.GroupName => AccountPermissions.GroupName;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/my", async (
			IQueryHandler<GetMyAccountsQuery, IEnumerable<GetMyAccountsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetMyAccountsQuery(), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<IEnumerable<GetMyAccountsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/change/{key}", async (
			string key,
			HttpContext httpContext,
			ICommandHandler<ChangeAccountCommand, AuthenticationResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new ChangeAccountCommand(key), cancellationToken);
			if (result.IsSuccess)
				httpContext.SetCookieValue(result.Value.Token);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			//.HasPermission(AccountPermissions.PermissionAccountChange.PermissionName)
			.Produces<AuthenticationResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/create-owner", async (
			HttpContext httpContext,
			ICommandHandler<CreateOwnerAccountCommand, AuthenticationResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new CreateOwnerAccountCommand(), cancellationToken);
			if (result.IsSuccess)
				httpContext.SetCookieValue(result.Value.Token);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			//.HasPermission(AccountPermissions.PermissionAccountCreateOwner.PermissionName)
			.Produces<AuthenticationResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/create-client", async (
		   HttpContext httpContext,
		   ICommandHandler<CreateClientAccountCommand, AuthenticationResponse> handler,
		   CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new CreateClientAccountCommand(), cancellationToken);
			if (result.IsSuccess)
				httpContext.SetCookieValue(result.Value.Token);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			//.HasPermission(AccountPermissions.PermissionAccountCreateClient.PermissionName)
			.Produces<AuthenticationResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/{userId:guid}/deactivate", async (
			Guid userId,
			ICommandHandler<DeactivateAccountCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new DeactivateAccountCommand(userId), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/{userId:guid}/activate", async (
			Guid userId,
			ICommandHandler<ActivateAccountCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new ActivateAccountCommand(userId), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
