using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Document.Contract.ContractTemplates.Commands;
using Document.Contract.ContractTemplates.Enums;
using Document.Contract.ContractTemplates.Queries;
using Document.Contract.ContractTemplates.Responses;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Document;
using Microsoft.AspNetCore.Mvc;

namespace Maydon.Administration.Host.Endpoints.Document;

internal sealed class ContractTemplates : IEndpoint
{
    string IEndpoint.GroupName => ContractTemplatePermissions.GroupName;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] GetContractTemplatesQuery query,
            IQueryHandler<GetContractTemplatesQuery, PagedList<ContractTemplateListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(ContractTemplatePermissions.PermissionContractTemplateList.PermissionName)
            .WithName("Admin_GetContractTemplates")
            .WithSummary("List contract templates with optional filters")
            .Produces<PagedList<ContractTemplateListResponse>>()
            .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetContractTemplateByIdQuery, ContractTemplateResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetContractTemplateByIdQuery(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(ContractTemplatePermissions.PermissionContractTemplateGetById.PermissionName)
            .WithName("Admin_GetContractTemplateById")
            .WithSummary("Get a contract template by ID")
            .Produces<ContractTemplateResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/", async (
            [FromBody] CreateContractTemplateCommand command,
            ICommandHandler<CreateContractTemplateCommand, ContractTemplateResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                value => Results.Created($"/api/v1/contract-templates/{value.Id}", value),
                CustomResults.Problem);
        })
            .HasPermission(ContractTemplatePermissions.PermissionContractTemplateCreate.PermissionName)
            .WithName("Admin_CreateContractTemplate")
            .WithSummary("Create a new contract template")
            .Produces<ContractTemplateResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateContractTemplateRequest request,
            ICommandHandler<UpdateContractTemplateCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateContractTemplateCommand
            {
                Id = id,
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Page = request.Page,
                Theme = request.Theme,
                Header = request.Header,
                Footer = request.Footer,
                Bodies = request.Bodies,
                ManualFields = request.ManualFields,
                Scope = request.Scope,
                Category = request.Category,
                IsActive = request.IsActive
            };

            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .HasPermission(ContractTemplatePermissions.PermissionContractTemplateUpdate.PermissionName)
            .WithName("Admin_UpdateContractTemplate")
            .WithSummary("Update a contract template (bumps version)")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut("/{id:guid}/bodies/{lang}", async (
            Guid id,
            string lang,
            [FromBody] UpdateContractTemplateBodyRequest request,
            ICommandHandler<UpdateContractTemplateBodyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateContractTemplateBodyCommand { Id = id, Language = lang, Blocks = request.Blocks };
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .HasPermission(ContractTemplatePermissions.PermissionContractTemplateUpdateBody.PermissionName)
            .WithName("Admin_UpdateContractTemplateBody")
            .WithSummary("Update a single language body")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteContractTemplateCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new DeleteContractTemplateCommand(id), cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .HasPermission(ContractTemplatePermissions.PermissionContractTemplateDelete.PermissionName)
            .WithName("Admin_DeleteContractTemplate")
            .WithSummary("Soft-delete a contract template")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/{id:guid}/preview", async (
            Guid id,
            PreviewPdfRequest? request,
            IQueryHandler<PreviewContractTemplatePdfQuery, byte[]> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new PreviewContractTemplatePdfQuery(
                id,
                request?.Language ?? "ru",
                request?.ManualValues);

            var result = await handler.Handle(query, cancellationToken);
            return result.Match(
                value => Results.File(value, "application/pdf", "preview.pdf"),
                CustomResults.Problem);
        })
            .HasPermission(ContractTemplatePermissions.PermissionContractTemplatePreviewPdf.PermissionName)
            .WithName("Admin_PreviewContractTemplatePdf")
            .WithSummary("Generate a PDF preview without saving")
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/placeholders", async (
            IQueryHandler<GetPlaceholderCatalogQuery, PlaceholderCatalogResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetPlaceholderCatalogQuery(), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(ContractTemplatePermissions.PermissionContractTemplateList.PermissionName)
            .WithName("Admin_GetPlaceholderCatalog")
            .WithSummary("Get the placeholder catalog for the block editor")
            .Produces<PlaceholderCatalogResponse>();
    }
}

internal sealed record UpdateContractTemplateRequest(
    string? Code = null,
    JsonDocument? Name = null,
    JsonDocument? Description = null,
    JsonDocument? Page = null,
    JsonDocument? Theme = null,
    JsonDocument? Header = null,
    JsonDocument? Footer = null,
    JsonDocument? Bodies = null,
    JsonDocument? ManualFields = null,
    ContractTemplateScope? Scope = null,
    ContractTemplateCategory? Category = null,
    bool? IsActive = null);

internal sealed record UpdateContractTemplateBodyRequest
{
    public required JsonDocument Blocks { get; init; }
}

internal sealed record PreviewPdfRequest
{
    public string? Language { get; init; }
    public string? ManualValues { get; init; }
}
