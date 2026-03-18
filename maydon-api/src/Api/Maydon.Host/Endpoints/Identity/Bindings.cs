using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;
using Identity.Application.Bindings.EImzo;
using Identity.Application.Bindings.OneId;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Identity;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Bindings : IEndpoint
{
	string IEndpoint.GroupName => BindingPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("/oneid", async (
			OneIdBindingCommand command,
			HttpContext httpContext,
			ICommandHandler<OneIdBindingCommand, bool> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(BindingPermissions.PermissionBindingOneId.PermissionName)
			.Produces<AuthenticationResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/eimzo", async (
			EImzoBindingCommand command,
			HttpContext httpContext,
			ICommandHandler<EImzoBindingCommand, bool> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(BindingPermissions.PermissionBindingEimzo.PermissionName)
			.Produces<AuthenticationResponse>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
