using System.Text.Json;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Pdf;
using Core.Application.Resources;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.ContractTemplates.Rendering;
using Document.Application.Features.ContractTemplates.Rendering.Renderers;
using Document.Contract.ContractTemplates.Queries;
using Document.Contract.Gateways;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Document.Application.Features.ContractTemplates.Queries.PreviewPdf;

public sealed class PreviewContractTemplatePdfQueryHandler(
    IDocumentDbContext db,
    IExecutionContextProvider executionContext,
    IOwnerPlaceholderResolver ownerPlaceholderResolver,
    IPdfRenderer pdfRenderer,
    BlockRendererFactory blockRendererFactory,
    ISharedViewLocalizer sharedViewLocalizer,
    ILogger<PreviewContractTemplatePdfQueryHandler> logger)
    : IQueryHandler<PreviewContractTemplatePdfQuery, byte[]>
{
    public async Task<Result<byte[]>> Handle(
        PreviewContractTemplatePdfQuery query,
        CancellationToken cancellationToken)
    {
        var template = await db.ContractTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == query.TemplateId &&
                                      t.TenantId == executionContext.TenantId,
                cancellationToken);

        if (template is null)
        {
            return Result.Failure<byte[]>(sharedViewLocalizer.TemplateNotFound(nameof(query.TemplateId)));
        }

        // Parse template JSON fields
        using var bodiesDoc = JsonDocument.Parse(template.Bodies);
        using var pageDoc = JsonDocument.Parse(template.Page);
        using var themeDoc = JsonDocument.Parse(template.Theme);

        var bodies = bodiesDoc.RootElement;
        var page = pageDoc.RootElement;
        var theme = themeDoc.RootElement;

        // Get the blocks for the requested language
        if (!bodies.TryGetProperty(query.Language, out var blocks) || blocks.ValueKind != JsonValueKind.Array)
        {
            return Result.Failure<byte[]>(
                sharedViewLocalizer.ContractTemplateLanguageNotFound(nameof(query.Language), query.Language));
        }

        // Auto-resolve owner/company/bank placeholders from backend
        var values = await ownerPlaceholderResolver.ResolveOwnerValuesAsync(
            executionContext.TenantId, cancellationToken);

        // Merge manual values if provided
        if (!string.IsNullOrWhiteSpace(query.ManualValues))
        {
            JsonDocument manualDoc;
            try
            {
                manualDoc = JsonDocument.Parse(query.ManualValues);
            }
            catch (JsonException)
            {
                return Result.Failure<byte[]>(
                    sharedViewLocalizer.ContractTemplateInvalidManualValues(nameof(query.ManualValues)));
            }

            using (manualDoc)
            {
                if (manualDoc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in manualDoc.RootElement.EnumerateObject())
                    {
                        values[prop.Name] = prop.Value.Clone();
                    }
                }
            }
        }

        // Resolve placeholders in the block tree (preview mode: show [key] for unresolved)
        var resolvedBlocks = PlaceholderResolver.Resolve(blocks, values, replaceUnresolved: true);

        // Render blocks to HTML
        var context = new BlockRenderContext
        {
            Factory = blockRendererFactory,
            ResolvedValues = values
        };
        var bodyHtml = blockRendererFactory.RenderBlocks(resolvedBlocks, context);

        // Compose full HTML page (body only — header/footer handled by Gotenberg natively)
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

        // Convert to PDF using Gotenberg
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
            logger.LogError("PDF rendering failed for template {TemplateId}: {Error}",
                template.Id, pdfResult.Error.Description);
            return Result.Failure<byte[]>(sharedViewLocalizer.ContractTemplatePdfRenderFailed(nameof(query.TemplateId)));
        }

        return pdfResult;
    }
    /// <summary>
    /// Builds a standalone HTML document for Gotenberg's native header/footer rendering.
    /// Chromium uses special CSS classes: "pageNumber" and "totalPages" for auto page numbering.
    /// Tokens {page} and {pages} in template text are replaced with the appropriate spans.
    /// </summary>
    private static string BuildGotenbergHeaderFooterHtml(JsonElement element, JsonElement theme)
    {
        var text = element.GetPropertyOrDefault("text", "");
        var align = element.GetPropertyOrDefault("align", "center");

        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var fontFamily = theme.GetPropertyOrDefault("font_family", "Times New Roman");
        var fontSize = Math.Max(theme.GetPropertyOrDefault("font_size", 12) - 2, 8);

        // Replace {page} and {pages} tokens with Gotenberg/Chromium magic spans
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
