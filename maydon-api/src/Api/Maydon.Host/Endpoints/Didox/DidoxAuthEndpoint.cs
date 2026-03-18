using Microsoft.AspNetCore.Mvc;
using Core.Domain.Extensions;
using Didox.Application.Services;
using Didox.Application.Services.Auth;
using Didox.Application.Contracts.DidoxClient.Contracts.Registration;
using Didox.Application.Contracts.DidoxAccounts.Responses;
using Maydon.Host.Abstractions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Didox;

internal sealed class DidoxAuthEndpoint : IEndpoint
{
    string IEndpoint.GroupName => "didox";
    string IEndpoint.Route => "didox-auth";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (
            RegistrationRequest request,
            [FromServices] IDidoxAuthService didoxAuthService,
            CancellationToken cancellationToken) =>
        {
            var result = await didoxAuthService.RegisterAsync(request, cancellationToken);
            return result.Match(
                value => Results.Created($"/api/v1/didox-accounts/{value.Id}", value),
                CustomResults.Problem);
        })
            .WithName("DidoxRegistration")
            .WithSummary("Registration into Didox system")
            .Produces<DidoxAccountResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
