using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;
using Identity.Application.Authentication.Registration.CheckPhoneNumber;
using Identity.Application.Authentication.Registration.EImzo;
using Identity.Application.Authentication.Registration.EImzoMobile;
using Identity.Application.Authentication.Registration.OneId;
using Identity.Application.Authentication.Registration.PhoneNumber;
using Identity.Application.Authentication.Registration.PhoneNumberConfirm;
using Identity.Domain;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Registration : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("/check-phone-number", async (
			CheckPhoneNumberCommand command,
			ICommandHandler<CheckPhoneNumberCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/phone-number-confirm", async (
			PhoneNumberRegistrationCommand command,
			HttpContext httpContext,
			ICommandHandler<PhoneNumberRegistrationCommand, string> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<string>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/create-password", async (
			PhoneNumberRegistrationConfirmCommand command,
			HttpContext httpContext,
			ICommandHandler<PhoneNumberRegistrationConfirmCommand, AuthenticationResponse> handler,
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

		app.MapPost("/oneid", async (
			OneIdRegistrationCommand command,
			HttpContext httpContext,
			ICommandHandler<OneIdRegistrationCommand, AuthenticationResponse> handler,
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

		app.MapPost("/eimzo", async (
			EImzoRegistrationCommand command,
			HttpContext httpContext,
			ICommandHandler<EImzoRegistrationCommand, AuthenticationResponse> handler,
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
			EImzoMobileRegistrationCommand command,
			HttpContext httpContext,
			ICommandHandler<EImzoMobileRegistrationCommand, AuthenticationResponse> handler,
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
	}
}
