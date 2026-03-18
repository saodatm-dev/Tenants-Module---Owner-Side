using Core.Domain.Extensions;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Contracts.DidoxAccounts.Commands;
using Didox.Application.Contracts.DidoxAccounts.Queries;
using Didox.Application.Contracts.DidoxAccounts.Responses;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Didox;

public record UpdateDidoxAccountRequest(string? Login = null, string? Password = null);

internal sealed class DidoxAccountEndpoint : IEndpoint
{
    string IEndpoint.GroupName => "didox";
    string IEndpoint.Route => "didox-accounts";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] GetDidoxAccountsQuery query,
            IQueryHandler<GetDidoxAccountsQuery, PagedList<DidoxAccountResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithName("GetDidoxAccounts")
            .WithSummary("Get all Didox accounts")
            .Produces<PagedList<DidoxAccountResponse>>()
            .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetDidoxAccountByIdQuery, DidoxAccountResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetDidoxAccountByIdQuery(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithName("GetDidoxAccountById")
            .WithSummary("Get Didox account by ID")
            .Produces<DidoxAccountResponse>()
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/", async (
            CreateDidoxAccountCommand command,
            ICommandHandler<CreateDidoxAccountCommand, DidoxAccountResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                value => Results.Created($"/api/v1/didox-accounts/{value.Id}", value),
                CustomResults.Problem);
        })
            .WithName("CreateDidoxAccount")
            .WithSummary("Create a new Didox account")
            .Produces<DidoxAccountResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/{id:guid}", async (
            Guid id,
            UpdateDidoxAccountRequest request,
            ICommandHandler<UpdateDidoxAccountCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateDidoxAccountCommand(id, request.Login, request.Password);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .WithName("UpdateDidoxAccount")
            .WithSummary("Update a Didox account")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteDidoxAccountCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new DeleteDidoxAccountCommand(id), cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .WithName("DeleteDidoxAccount")
            .WithSummary("Delete a Didox account")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
