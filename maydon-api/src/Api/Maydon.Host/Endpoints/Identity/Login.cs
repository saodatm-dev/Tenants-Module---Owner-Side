using System.Net.Http.Headers;
using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;
using Identity.Application.Authentication.Login.EImzo;
using Identity.Application.Authentication.Login.EImzoMobile;
using Identity.Application.Authentication.Login.OneId;
using Identity.Application.Authentication.Login.PhoneNumber;
using Identity.Domain;
using Maydon.Host.Abstractions;
using Maydon.Host.Endpoints.Identity.TestsBox;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Login : IEndpoint
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

		app.MapPost("/oneid", async (
		   [FromBody] OneIdLoginCommand command,
		   HttpContext httpContext,
		   [FromServices] ICommandHandler<OneIdLoginCommand, AuthenticationResponse> handler,
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
		   [FromBody] EImzoLoginCommand command,
		   HttpContext httpContext,
		   [FromServices] ICommandHandler<EImzoLoginCommand, AuthenticationResponse> handler,
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
		   [FromBody] EImzoMobileLoginCommand command,
		   HttpContext httpContext,
		   [FromServices] ICommandHandler<EImzoMobileLoginCommand, AuthenticationResponse> handler,
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

		// TODO :ONLY for Testing Purpose remove it as quick as it possible
		app.MapPost("/my-id/test", async (
				IHttpClientFactory httpClientFactory,
				IOptions<MyIdOptions> myIdOptions,
				CancellationToken cancellationToken) =>
			{
				try
				{
					var client = httpClientFactory.CreateClient("MyId");
					var options = myIdOptions.Value;
					client.BaseAddress = new Uri(options.BaseUrl);

					// Step 1 Get token
					var requestBody = new MyIdTokenRequest { ClientId = options.ClientId, ClientSecret = options.ClientSecret };
					var response = await client.PostAsJsonAsync("api/v1/auth/clients/access-token", requestBody, cancellationToken);
					if (!response.IsSuccessStatusCode)
					{
						var error = await response.Content.ReadAsStringAsync(cancellationToken);
						return Results.Problem(error, statusCode: (int)response.StatusCode);
					}

					var result = await response.Content.ReadFromJsonAsync<MyIdTokenResponse>(cancellationToken: cancellationToken);

					// Step 2: Set Bearer auth and create session
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result?.AccessToken);

					var sessionResponse = await client.PostAsJsonAsync("api/v2/sdk/sessions", new { }, cancellationToken);

					if (!sessionResponse.IsSuccessStatusCode)
					{
						var error = await sessionResponse.Content.ReadAsStringAsync(cancellationToken);
						return Results.Problem(error, statusCode: (int)sessionResponse.StatusCode);
					}

					var session = await sessionResponse.Content.ReadFromJsonAsync<MyIdSessionResponse>(cancellationToken);
					return Results.Ok(session);
				}
				catch (Exception e)
				{
					return Results.InternalServerError(e.Message);
				}
			})
			.AllowAnonymous()
			.Produces<MyIdSessionResponse>()
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status500InternalServerError);
	}
}
