using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Pdf;
using Core.Application.Resources;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.ContractTemplates.Rendering;
using Document.Application.Features.ContractTemplates.Rendering.Renderers;
using Document.Contract.Contracts.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Document.Application.Features.Contracts.Queries.GeneratePdf;

/// <summary>
/// Handles <see cref="GenerateContractPdfQuery"/>.
/// Loads the contract body (already-resolved blocks) and template styling,
/// renders to HTML, then converts to PDF via Gotenberg.
/// </summary>
public sealed class GenerateContractPdfQueryHandler(
    IDocumentDbContext db,
    IPdfRenderer pdfRenderer,
    BlockRendererFactory blockRendererFactory,
    ISharedViewLocalizer sharedViewLocalizer,
    ILogger<GenerateContractPdfQueryHandler> logger)
    : IQueryHandler<GenerateContractPdfQuery, byte[]>
{
    public async Task<Result<byte[]>> Handle(
        GenerateContractPdfQuery query,
        CancellationToken cancellationToken)
    {
        // 1. Load contract
        var contract = await db.Contracts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure<byte[]>(sharedViewLocalizer.ContractNotFound(nameof(query.ContractId)));
        }

        // 2. Load the template for page/theme/header/footer styling
        var template = await db.ContractTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == contract.TemplateId, cancellationToken);

        if (template is null)
        {
            return Result.Failure<byte[]>(sharedViewLocalizer.TemplateNotFound(nameof(contract.TemplateId)));
        }

        // 3. Parse contract body (resolved JSON blocks) and template styling
        using var bodyDoc = JsonDocument.Parse(contract.Body);
        using var pageDoc = JsonDocument.Parse(template.Page);
        using var themeDoc = JsonDocument.Parse(template.Theme);

        var blocks = bodyDoc.RootElement;
        var page = pageDoc.RootElement;
        var theme = themeDoc.RootElement;

        // 4. Render blocks → HTML (body is already resolved, no placeholder substitution needed)
        var context = new BlockRenderContext
        {
            Factory = blockRendererFactory,
            ResolvedValues = new Dictionary<string, object?>()
        };
        var bodyHtml = blockRendererFactory.RenderBlocks(blocks, context);

        // 5. Compose full HTML page (body only — header/footer handled by Gotenberg natively)
        var html = HtmlPageComposer.Compose(page, theme, null, null, bodyHtml);

        // Build native Gotenberg header/footer HTML (renders on every page with page numbers)
        string? nativeHeaderHtml = null;
        string? nativeFooterHtml = null;

        if (template.Header is not null)
        {
            using var headerDoc = JsonDocument.Parse(template.Header);
            nativeHeaderHtml = BuildGotenbergHeaderFooterHtml(headerDoc.RootElement, theme);
        }

        if (template.Footer is not null)
        {
            using var footerDoc = JsonDocument.Parse(template.Footer);
            nativeFooterHtml = BuildGotenbergHeaderFooterHtml(footerDoc.RootElement, theme);
        }

        // 6. Convert to PDF via Gotenberg
        var orientation = page.GetPropertyOrDefault("orientation", "portrait");
        var options = new PdfRenderOptions
        {
            PaperWidth = "8.27",
            PaperHeight = "11.69",
            Landscape = orientation == "landscape",
            PreferCssPageSize = true,
            PrintBackground = true,
            HeaderHtml = nativeHeaderHtml,
            FooterHtml = nativeFooterHtml,
            MarginTop = nativeHeaderHtml is not null ? "0.6" : "0",
            MarginBottom = nativeFooterHtml is not null ? "0.6" : "0"
        };

        var pdfResult = await pdfRenderer.ConvertHtmlToPdfAsync(html, options, cancellationToken);

        if (pdfResult.IsFailure)
        {
            logger.LogError("PDF rendering failed for contract {ContractId}: {Error}",
                contract.Id, pdfResult.Error.Description);
            return Result.Failure<byte[]>(sharedViewLocalizer.ContractPdfRenderFailed(nameof(query.ContractId)));
        }

        return pdfResult;
    }

    private static string BuildGotenbergHeaderFooterHtml(JsonElement element, JsonElement theme)
    {
        var text = element.GetPropertyOrDefault("text", "");
        var align = element.GetPropertyOrDefault("align", "center");

        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var fontFamily = theme.GetPropertyOrDefault("font_family", "Times New Roman");
        var fontSize = Math.Max(theme.GetPropertyOrDefault("font_size", 12) - 2, 8);

        var rendered = System.Net.WebUtility.HtmlEncode(text)
            .Replace(System.Net.WebUtility.HtmlEncode("{page}"), "<span class=\"pageNumber\"></span>")
            .Replace(System.Net.WebUtility.HtmlEncode("{pages}"), "<span class=\"totalPages\"></span>");

        return $$"""
            <!DOCTYPE html>
            <html><head>
            <style>
                body { margin: 0; padding: 0 10mm; width: 100%; font-family: '{{System.Net.WebUtility.HtmlEncode(fontFamily)}}', serif; font-size: {{fontSize}}pt; color: #333; }
                .content { text-align: {{align}}; width: 100%; }
            </style>
            </head><body>
            <div class="content">{{rendered}}</div>
            </body></html>
            """;
    }
}
