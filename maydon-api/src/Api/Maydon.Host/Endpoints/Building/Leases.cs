using Building.Application.Leases.Create;
using Building.Application.Leases.Delete;
using Building.Application.Leases.Get;
using Building.Application.Leases.GetById;
using Building.Application.Leases.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Infrastructure.Extensions;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Building;

namespace Maydon.Host.Endpoints.Building;

internal sealed class Leases : IEndpoint
{
    string IEndpoint.GroupName => LeasePermissions.GroupName;
    string IEndpoint.Route => "leases";
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] GetLeasesQuery query,
            IQueryHandler<GetLeasesQuery, PagedList<GetLeasesResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .Produces<PagedList<GetLeasesResponse>>()
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

        app.MapGet("/{id:Guid}", async (
            Guid id,
            IQueryHandler<GetLeaseByIdQuery, GetLeaseByIdResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetLeaseByIdQuery(id), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .Produces<GetLeaseByIdResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

        app.MapPost("/", async (
            CreateLeaseCommand command,
            ICommandHandler<CreateLeaseCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(LeasePermissions.PermissionLeaseCreate.PermissionName)
            .Produces<Guid>()
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/", async (
            UpdateLeaseCommand command,
            ICommandHandler<UpdateLeaseCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(LeasePermissions.PermissionLeaseUpdate.PermissionName)
            .Produces<Guid>()
            .Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteLeaseCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new DeleteLeaseCommand(id), cancellationToken);
            return result.Match(CustomResults.Ok, CustomResults.Problem);
        })
            .HasPermission(LeasePermissions.PermissionLeaseRemove.PermissionName)
            .Produces<Guid>()
            .Produces(StatusCodes.Status400BadRequest);
    }
}
