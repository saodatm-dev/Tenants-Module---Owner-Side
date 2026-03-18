using Core.Application.Abstractions.Messaging;
using Document.Contract.Contracts.Commands;
using Document.Contract.Contracts.Enums;
using Document.Contract.Contracts.Queries;
using Document.Contract.Contracts.Responses;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Document;
using Microsoft.AspNetCore.Mvc;

namespace Maydon.Administration.Host.Endpoints.Document;

internal sealed class Contracts : IEndpoint
{
    string IEndpoint.GroupName => ContractPermissions.GroupName;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? status,
            [FromQuery] Guid? leaseId,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate,
            IQueryHandler<ListContractsQuery, PagedContractResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new ListContractsQuery(page, pageSize, status, leaseId, FromDate: fromDate, ToDate: toDate);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractList.PermissionName)
            .WithName("Admin_ListContracts")
            .WithSummary("Get paginated contract list with filters")
            .Produces<PagedContractResponse>()
            .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetContractByIdQuery, ContractResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetContractByIdQuery(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractGetById.PermissionName)
            .WithName("Admin_GetContractById")
            .WithSummary("Get a contract by ID")
            .Produces<ContractResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/", async (
            [FromBody] CreateContractRequest request,
            ICommandHandler<CreateContractCommand, ContractResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateContractCommand(
                request.TemplateId,
                request.LeaseId,
                request.Language,
                request.Body);

            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                value => Results.Created($"/api/v1/contracts/{value.Id}", value),
                CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractCreate.PermissionName)
            .WithName("Admin_CreateContract")
            .WithSummary("Create a new contract (Draft)")
            .Produces<ContractResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/{id:guid}/body", async (
            Guid id,
            [FromBody] UpdateContractBodyRequest request,
            ICommandHandler<UpdateContractBodyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateContractBodyCommand(id, request.Body);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractUpdateBody.PermissionName)
            .WithName("Admin_UpdateContractBody")
            .WithSummary("Edit contract body (Draft only)")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/{id:guid}/regenerate", async (
            Guid id,
            [FromBody] RegenerateContractRequest request,
            ICommandHandler<RegenerateContractCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegenerateContractCommand(id, request.Body);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractRegenerate.PermissionName)
            .WithName("Admin_RegenerateContract")
            .WithSummary("Regenerate contract body (Draft only)")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/{id:guid}/reject", async (
            Guid id,
            [FromBody] RejectContractRequest request,
            ICommandHandler<RejectContractCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RejectContractCommand(id, request.Party, request.Reason);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractReject.PermissionName)
            .WithName("Admin_RejectContract")
            .WithSummary("Reject a contract")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/{id:guid}/export-didox", async (
            Guid id,
            ICommandHandler<ExportContractToDidoxCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ExportContractToDidoxCommand(id);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.Accepted($"/api/v1/contracts/{id}",
                    new { contractId = id, message = "Contract exported to Didox. Editing is now locked." }),
                CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractExportDidox.PermissionName)
            .WithName("Admin_ExportContractToDidox")
            .WithSummary("Export contract to Didox (locks editing)")
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/{id:guid}/sync-from-didox", async (
            Guid id,
            ICommandHandler<SyncContractFromDidoxCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SyncContractFromDidoxCommand(id);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractSyncDidox.PermissionName)
            .WithName("Admin_SyncContractFromDidox")
            .WithSummary("Sync contract status from Didox")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/{id:guid}/pdf", async (
            Guid id,
            IQueryHandler<GenerateContractPdfQuery, byte[]> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GenerateContractPdfQuery(id), cancellationToken);
            return result.Match(
                value => Results.File(value, "application/pdf", $"contract-{id}.pdf"),
                CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractGetById.PermissionName)
            .WithName("Admin_GetContractPdf")
            .WithSummary("Generate and download a PDF for a contract")
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/{id:guid}/attachments", async (
            Guid id,
            IFormFile file,
            [FromQuery] AttachmentDocumentType? documentType,
            ICommandHandler<UploadContractAttachmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UploadContractAttachmentCommand(
                id,
                file.FileName,
                file.ContentType,
                file.Length,
                documentType ?? AttachmentDocumentType.Other,
                file.OpenReadStream());
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(
                () => Results.Created($"/api/v1/contracts/{id}/attachments",
                    new { contractId = id, fileName = file.FileName }),
                CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractUploadAttachment.PermissionName)
            .WithName("Admin_UploadContractAttachment")
            .WithSummary("Upload an attachment to a contract")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();

        app.MapGet("/{id:guid}/attachments", async (
            Guid id,
            IQueryHandler<ListContractAttachmentsQuery, IReadOnlyList<ContractAttachmentResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new ListContractAttachmentsQuery(id);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractGetById.PermissionName)
            .WithName("Admin_ListContractAttachments")
            .WithSummary("List contract attachments")
            .Produces<IReadOnlyList<ContractAttachmentResponse>>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/prefill", async (
            [FromQuery] Guid templateId,
            [FromQuery] Guid leaseId,
            [FromQuery] string lang,
            IQueryHandler<PrefillContractQuery, PrefillContractResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new PrefillContractQuery(templateId, leaseId, lang);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .HasPermission(ContractPermissions.PermissionContractCreate.PermissionName)
            .WithName("Admin_PrefillContract")
            .WithSummary("Pre-fill contract template with lease and company data")
            .Produces<PrefillContractResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }
}

internal sealed record CreateContractRequest(
    Guid TemplateId,
    Guid LeaseId,
    string Language,
    string Body);

internal sealed record UpdateContractBodyRequest(
    string Body);

internal sealed record RegenerateContractRequest(
    string Body);

internal sealed record RejectContractRequest(
    string Party,
    string? Reason = null);
