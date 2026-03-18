using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;
using Identity.Application.Authentication.Authorize.EImzoMobile;
using Identity.Application.Authentication.Authorize.OneId;
using Identity.Application.Authentication.Challange;
using Identity.Application.Authentication.ForgotPassword.PhoneNumber;
using Identity.Application.Authentication.ForgotPassword.PhoneNumberConfirm;
using Identity.Application.Authentication.Logout;
using Identity.Application.Authentication.RefreshToken;
using Identity.Application.Core.Abstractions.Services.EImzo;
using Identity.Domain;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Authentication : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/challenge", async (
				IQueryHandler<GetEImzoChallengeQuery, ChallengeResponse> handler,
				CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetEImzoChallengeQuery(), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<ChallengeResponse>()
			.Produces(StatusCodes.Status400BadRequest);


		app.MapPost("/logout", async (
			LogoutCommand command,
			HttpContext httpContext,
			ICommandHandler<LogoutCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			if (result.IsSuccess)
				httpContext.RemoveCookieValue();
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.Produces(StatusCodes.Status200OK)
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
			.AllowAnonymous()
			.Produces<AuthenticationResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/forgot-password", async (
			PhoneNumberForgotPasswordCommand command,
			ICommandHandler<PhoneNumberForgotPasswordCommand, string> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<string>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/forgot-password-confirm", async (
			PhoneNumberForgotPasswordConfirmCommand command,
			HttpContext httpContext,
			ICommandHandler<PhoneNumberForgotPasswordConfirmCommand, AuthenticationResponse> handler,
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

        app.MapPost("/eimzo-mobile", async (
                [FromBody] EImzoMobileAuthCommand command,
                HttpContext httpContext,
                [FromServices] ICommandHandler<EImzoMobileAuthCommand, AuthenticationResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .AllowAnonymous()
            .Produces<AuthenticationResponse>()
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/oneid", async (
                [FromBody] OneIdAuthCommand command,
                HttpContext httpContext,
                [FromServices] ICommandHandler<OneIdAuthCommand, AuthenticationResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .AllowAnonymous()
            .Produces<AuthenticationResponse>()
            .Produces(StatusCodes.Status400BadRequest);

    }
}
