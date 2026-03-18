using Didox.Application.Contracts.DidoxAccounts.Responses;
using Didox.Application.Contracts.DidoxClient.Contracts.Registration;
using Didox.Application.Services;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Didox;
using Microsoft.AspNetCore.Mvc;

namespace Maydon.Administration.Host.Endpoints.Didox;

internal sealed class DidoxAuth : IEndpoint
{
    string IEndpoint.GroupName => DidoxAuthPermissions.GroupName;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (
            RegistrationRequest request,
            [FromServices] IDidoxAuthService didoxAuthService,
            CancellationToken cancellationToken) =>
        {
            var result = await didoxAuthService.RegisterAsync(request, cancellationToken);
            return result.Match(
                value => Results.Created($"/api/v1/didoxaccounts/{value.Id}", value),
                CustomResults.Problem);
        })
            .HasPermission(DidoxAuthPermissions.PermissionDidoxAuthRegister.PermissionName)
            .WithName("Admin_DidoxRegistration")
            .WithSummary("Registration into Didox system")
            .Produces<DidoxAccountResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
