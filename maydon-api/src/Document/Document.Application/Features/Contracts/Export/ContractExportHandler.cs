using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Pdf;
using Document.Application.Abstractions.Data;
using Document.Application.Features.ContractTemplates.Rendering;
using Document.Contract.Gateways;
using Document.Contract.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Document.Application.Features.Contracts.Export;

/// <summary>
/// Handles <see cref="InitiateDocumentExport"/> integration events for contracts.
/// Loads the contract body + template styling, renders to PDF via Gotenberg,
/// then publishes <see cref="DocumentExportRequested"/> for the Didox worker.
/// </summary>
public sealed class ContractExportHandler(
    IDocumentDbContext db,
    IPdfRenderer pdfRenderer,
    BlockRendererFactory blockRendererFactory,
    IUserLookupService userLookupService,
    IIntegrationEventPublisher publisher,
    ILogger<ContractExportHandler> logger)
    : IntegrationEventHandlerBase<InitiateDocumentExport>(logger)
{
    protected override async Task HandleCoreAsync(InitiateDocumentExport @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("ContractExportHandler: Processing export for Document {DocumentId}, Provider {Provider}",
            @event.DocumentId, @event.TargetProvider);

        // 1. Load contract
        var contract = await db.Contracts
            .AsNoTracking()
            .Include(c => c.IntegrationStates)
            .FirstOrDefaultAsync(c => c.Id == @event.DocumentId, cancellationToken);

        if (contract is null)
        {
            logger.LogWarning("Contract {ContractId} not found, skipping export", @event.DocumentId);
            return;
        }

        // 2. Load the template for page/theme styling
        var template = await db.ContractTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == contract.TemplateId, cancellationToken);

        if (template is null)
        {
            logger.LogWarning("Template {TemplateId} not found for contract {ContractId}, skipping export", contract.TemplateId, contract.Id);
            return;
        }

        // 3. Render contract body (resolved JSON blocks) → HTML → PDF
        string pdfBase64;
        try
        {
            pdfBase64 = await RenderContractToPdfBase64Async(contract.Body, template, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to render PDF for contract {ContractId}", contract.Id);
            return;
        }

        // 4. Resolve owner name from Identity (by PINFL or INN)
        var ownerName = string.Empty;
        var ownerIdentifier = contract.OwnerPinfl ?? contract.OwnerInn;
        if (!string.IsNullOrEmpty(ownerIdentifier))
        {
            var ownerSnapshot = await userLookupService.GetByTinOrPinflAsync(ownerIdentifier, cancellationToken);
            if (ownerSnapshot is not null)
                ownerName = ownerSnapshot.FullName;
        }

        // 5. Resolve client name from Identity (by PINFL or INN)
        var clientName = string.Empty;
        var clientIdentifier = contract.ClientPinfl ?? contract.ClientInn;
        if (!string.IsNullOrEmpty(clientIdentifier))
        {
            var clientSnapshot = await userLookupService.GetByTinOrPinflAsync(clientIdentifier, cancellationToken);
            if (clientSnapshot is not null)
                clientName = clientSnapshot.FullName;
        }

        // 6. Build export payload and publish DocumentExportRequested
        var payload = new DocumentExportPayload(
            Document: new ExportDocumentDto(
                Id: contract.Id,
                DocumentNumber: contract.ContractNumber,
                ContractDate: contract.ContractDate,
                SignerTin: contract.ClientInn,
                SignerPinfl: contract.ClientPinfl),
            Owner: new OwnerDto(
                Id: contract.OwnerCompanyId,
                Name: ownerName,
                Tin: contract.OwnerInn ?? string.Empty,
                Pinfl: contract.OwnerPinfl ?? string.Empty),
            Signer: new OwnerDto(
                Id: contract.ClientCompanyId ?? Guid.Empty,
                Name: clientName,
                Tin: contract.ClientInn ?? string.Empty,
                Pinfl: contract.ClientPinfl ?? string.Empty),
            TargetProvider: @event.TargetProvider,
            PdfBase64: pdfBase64);

        var exportEvent = new DocumentExportRequested(
            DocumentId: contract.Id,
            TargetProvider: @event.TargetProvider,
            Payload: payload,
            InitiatedBy: @event.InitiatedBy);

        await publisher.PublishAsync(exportEvent, cancellationToken);

        logger.LogInformation("ContractExportHandler: Published DocumentExportRequested for contract {ContractId}", contract.Id);
    }

    private async Task<string> RenderContractToPdfBase64Async(
        string contractBody,
        Domain.ContractTemplates.ContractTemplate template,
        CancellationToken ct)
    {
        // Parse contract body (resolved JSON blocks)
        using var bodyDoc = JsonDocument.Parse(contractBody);
        using var pageDoc = JsonDocument.Parse(template.Page);
        using var themeDoc = JsonDocument.Parse(template.Theme);

        var blocks = bodyDoc.RootElement;
        var page = pageDoc.RootElement;
        var theme = themeDoc.RootElement;

        // Render blocks to HTML
        var context = new BlockRenderContext
        {
            Factory = blockRendererFactory,
            ResolvedValues = new Dictionary<string, object?>()
        };
        var bodyHtml = blockRendererFactory.RenderBlocks(blocks, context);

        // Render header and footer if present
        string? headerHtml = null;
        string? footerHtml = null;

        if (template.Header is not null)
        {
            using var headerDoc = JsonDocument.Parse(template.Header);
            headerHtml = blockRendererFactory.RenderBlock(headerDoc.RootElement, context);
        }

        if (template.Footer is not null)
        {
            using var footerDoc = JsonDocument.Parse(template.Footer);
            footerHtml = blockRendererFactory.RenderBlock(footerDoc.RootElement, context);
        }

        // Compose full HTML page
        var html = HtmlPageComposer.Compose(page, theme, headerHtml, footerHtml, bodyHtml);

        // Convert to PDF base64
        var orientation = page.GetPropertyOrDefault("orientation", "portrait");
        var options = new PdfRenderOptions
        {
            PaperWidth = "8.27",
            PaperHeight = "11.69",
            Landscape = orientation == "landscape",
            PreferCssPageSize = true,
            PrintBackground = true
        };

        var result = await pdfRenderer.ConvertHtmlToPdfBase64Async(html, options, ct);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"PDF rendering failed: {result.Error.Description}");
        }

        return result.Value;
    }
}
