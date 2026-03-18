using Core.Application.Abstractions.Messaging;
using Core.Domain.Extensions;
using Document.Contract.Contracts.Commands;
using Document.Contract.Contracts.Enums;
using Document.Contract.Contracts.Queries;
using Document.Contract.Contracts.Responses;
using Maydon.Host.Abstractions;
using Maydon.Host.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Maydon.Host.Endpoints.Document;

internal sealed class ContractEndpoint : IEndpoint
{
    string IEndpoint.GroupName => "documents";
    string IEndpoint.Route => "contracts";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetContractByIdQuery, ContractResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetContractByIdQuery(id), ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithName("GetContractById")
            .WithSummary("Get a contract by ID")
            .Produces<ContractResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/", async (
            [FromBody] CreateContractRequest request,
            ICommandHandler<CreateContractCommand, ContractResponse> handler,
            CancellationToken ct) =>
        {
            var command = new CreateContractCommand(
                request.TemplateId,
                request.LeaseId,
                request.Language,
                request.Body);

            var result = await handler.Handle(command, ct);
            return result.Match(
                value => Results.Created($"/api/v1/contracts/{value.Id}", value),
                CustomResults.Problem);
        })
            .WithName("CreateContract")
            .WithSummary("Create a new contract (Draft)")
            .WithDescription("Creates a contract in Draft status from a template and lease data. " +
                             "Auto-generates contract number and default financial items from the lease.")
            .Produces<ContractResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);


        app.MapPut("/{id:guid}/body", async (
            Guid id,
            [FromBody] UpdateContractBodyRequest request,
            ICommandHandler<UpdateContractBodyCommand> handler,
            CancellationToken ct) =>
        {
            var command = new UpdateContractBodyCommand(id, request.Body);
            var result = await handler.Handle(command, ct);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .WithName("UpdateContractBody")
            .WithSummary("Edit contract body (Draft only)")
            .WithDescription("Updates the JSONB body of a contract. Only allowed while the contract is in Draft status.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/{id:guid}/export-didox", async (
            Guid id,
            ICommandHandler<ExportContractToDidoxCommand> handler,
            CancellationToken ct) =>
        {
            var command = new ExportContractToDidoxCommand(id);
            var result = await handler.Handle(command, ct);
            return result.Match(
                () => Results.Accepted($"/api/v1/contracts/{id}",
                    new { contractId = id, message = "Contract exported to Didox. Editing is now locked." }),
                CustomResults.Problem);
        })
            .WithName("ExportContractToDidox")
            .WithSummary("Export contract to Didox (locks editing)")
            .WithDescription("Transitions the contract to PendingSignature status and locks body edits. " +
                             "After this point, changes happen via the Didox integration only.")
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/prefill", async (
            [FromQuery] Guid templateId,
            [FromQuery] Guid leaseId,
            [FromQuery] string lang,
            IQueryHandler<PrefillContractQuery, PrefillContractResponse> handler,
            CancellationToken ct) =>
        {
            var query = new PrefillContractQuery(templateId, leaseId, lang);
            var result = await handler.Handle(query, ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithName("PrefillContract")
            .WithSummary("Pre-fill contract template with lease and company data")
            .WithDescription("Loads the template body, resolves placeholders from lease, property, owner, and client data, " +
                             "and returns the pre-filled body for the UI to review and edit.")
            .Produces<PrefillContractResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? status,
            [FromQuery] Guid? leaseId,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate,
            IQueryHandler<ListContractsQuery, PagedContractResponse> handler,
            CancellationToken ct) =>
        {
            var query = new ListContractsQuery(page, pageSize, status, leaseId, FromDate: fromDate, ToDate: toDate);
            var result = await handler.Handle(query, ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithName("ListContracts")
            .WithSummary("Get paginated contract list with filters")
            .WithDescription("Returns a paginated list of contracts filtered by status, lease, and date range.")
            .Produces<PagedContractResponse>()
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/{id:guid}/regenerate", async (
            Guid id,
            [FromBody] RegenerateContractRequest request,
            ICommandHandler<RegenerateContractCommand> handler,
            CancellationToken ct) =>
        {
            var command = new RegenerateContractCommand(id, request.Body);
            var result = await handler.Handle(command, ct);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .WithName("RegenerateContract")
            .WithSummary("Regenerate contract body (Draft only)")
            .WithDescription("Regenerates the contract body and increments the version. Only allowed while in Draft status.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);



        app.MapPost("/{id:guid}/reject", async (
            Guid id,
            [FromBody] RejectContractRequest request,
            ICommandHandler<RejectContractCommand> handler,
            CancellationToken ct) =>
        {
            var command = new RejectContractCommand(id, request.Party, request.Reason);
            var result = await handler.Handle(command, ct);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .WithName("RejectContract")
            .WithSummary("Reject a contract")
            .WithDescription("Rejects a contract by the specified party (owner or client). " +
                             "Sets the rejection reason and transitions to RejectedByOwner or RejectedByClient.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/{id:guid}/sync-from-didox", async (
            Guid id,
            ICommandHandler<SyncContractFromDidoxCommand> handler,
            CancellationToken ct) =>
        {
            var command = new SyncContractFromDidoxCommand(id);
            var result = await handler.Handle(command, ct);
            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
            .WithName("SyncContractFromDidox")
            .WithSummary("Sync contract status from Didox (Admin)")
            .WithDescription("Polls the Didox API for the current envelope status and reconciles with local contract state.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/{id:guid}/pdf", async (
            Guid id,
            IQueryHandler<GenerateContractPdfQuery, byte[]> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GenerateContractPdfQuery(id), ct);
            return result.Match(
                value => Results.File(value, "application/pdf", $"contract-{id}.pdf"),
                CustomResults.Problem);
        })
            .WithName("GetContractPdf")
            .WithSummary("Generate and download a PDF for a contract")
            .WithDescription("Renders the contract's resolved body to HTML using the template styling, " +
                             "then converts to PDF via Gotenberg. Returns the PDF file for download.")
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/{id:guid}/attachments", async (
            Guid id,
            IFormFile file,
            [FromQuery] AttachmentDocumentType? documentType,
            ICommandHandler<UploadContractAttachmentCommand> handler,
            CancellationToken ct) =>
        {
            var command = new UploadContractAttachmentCommand(
                id,
                file.FileName,
                file.ContentType,
                file.Length,
                documentType ?? AttachmentDocumentType.Other,
                file.OpenReadStream());
            var result = await handler.Handle(command, ct);
            return result.Match(
                () => Results.Created($"/api/v1/contracts/{id}/attachments", new { contractId = id, fileName = file.FileName }),
                CustomResults.Problem);
        })
            .WithName("UploadContractAttachment")
            .WithSummary("Upload an attachment to a contract")
            .WithDescription("Uploads a file attachment (≤10MB, PDF/PNG/JPG/DOCX) to the specified contract. " +
                             "Maximum 20 attachments per contract.")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .DisableAntiforgery();

        app.MapGet("/{id:guid}/attachments", async (
            Guid id,
            IQueryHandler<ListContractAttachmentsQuery, IReadOnlyList<ContractAttachmentResponse>> handler,
            CancellationToken ct) =>
        {
            var query = new ListContractAttachmentsQuery(id);
            var result = await handler.Handle(query, ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithName("ListContractAttachments")
            .WithSummary("List contract attachments")
            .WithDescription("Returns metadata for all attachments on the specified contract.")
            .Produces<IReadOnlyList<ContractAttachmentResponse>>()
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
