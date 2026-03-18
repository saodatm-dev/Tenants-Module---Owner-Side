using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;
using Identity.Application.Authentication.Login.PhoneNumber;
using Identity.Application.Authentication.RefreshToken;
using Identity.Domain;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;

namespace Maydon.Administration.Host.Endpoints.Identity;

internal sealed class Authentication : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("/phone-number", async (
		   PhoneNumberLoginCommand command,
		   HttpContext httpContext,
		   ICommandHandler<PhoneNumberLoginCommand, AuthenticationResponse> handler,
		   CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			if (result.IsSuccess)
				httpContext.SetCookieValue(result.Value.Token);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<AuthenticationResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/refresh-token", async (
			RefreshTokenCommand command,
			HttpContext httpContext,
			ICommandHandler<RefreshTokenCommand, AuthenticationResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			if (result.IsSuccess)
				httpContext.SetCookieValue(result.Value.Token);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<AuthenticationResponse>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
