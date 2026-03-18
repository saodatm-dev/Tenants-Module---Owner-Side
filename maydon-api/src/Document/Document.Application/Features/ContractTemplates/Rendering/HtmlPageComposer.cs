using System.Text;
using System.Text.Json;

namespace Document.Application.Features.ContractTemplates.Rendering;

/// <summary>
/// Assembles the final HTML document from theme, page config, header, footer, and body fragments.
/// </summary>
public static class HtmlPageComposer
{
    /// <summary>
    /// Composes a full HTML page from the template configuration and rendered body blocks.
    /// </summary>
    public static string Compose(
        JsonElement page,
        JsonElement theme,
        string? headerHtml,
        string? footerHtml,
        string bodyHtml)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"UTF-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("<style>");
        sb.AppendLine(GenerateCss(page, theme));
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        if (!string.IsNullOrWhiteSpace(headerHtml))
        {
            sb.AppendLine("<header class=\"page-header\">");
            sb.AppendLine(headerHtml);
            sb.AppendLine("</header>");
        }

        sb.AppendLine("<main class=\"page-body\">");
        sb.AppendLine(bodyHtml);
        sb.AppendLine("</main>");

        if (!string.IsNullOrWhiteSpace(footerHtml))
        {
            sb.AppendLine("<footer class=\"page-footer\">");
            sb.AppendLine(footerHtml);
            sb.AppendLine("</footer>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    /// <summary>
    /// Composes a bilingual (two-column) HTML page for dual-language PDF output.
    /// </summary>
    public static string ComposeBilingual(
        JsonElement page,
        JsonElement theme,
        string? headerHtml,
        string? footerHtml,
        string leftBodyHtml,
        string rightBodyHtml)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"UTF-8\">");
        sb.AppendLine("<style>");
        sb.AppendLine(GenerateCss(page, theme));
        sb.AppendLine(".bilingual-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }");
        sb.AppendLine(".bilingual-col { }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        if (!string.IsNullOrWhiteSpace(headerHtml))
        {
            sb.AppendLine("<header class=\"page-header\">");
            sb.AppendLine(headerHtml);
            sb.AppendLine("</header>");
        }

        sb.AppendLine("<main class=\"page-body bilingual-grid\">");
        sb.AppendLine("<div class=\"bilingual-col\">");
        sb.AppendLine(leftBodyHtml);
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"bilingual-col\">");
        sb.AppendLine(rightBodyHtml);
        sb.AppendLine("</div>");
        sb.AppendLine("</main>");

        if (!string.IsNullOrWhiteSpace(footerHtml))
        {
            sb.AppendLine("<footer class=\"page-footer\">");
            sb.AppendLine(footerHtml);
            sb.AppendLine("</footer>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private static string GenerateCss(JsonElement page, JsonElement theme)
    {
        var sb = new StringBuilder();

        // Page margins
        var marginTop = page.GetPropertyOrDefault("margin_top", 20);
        var marginBottom = page.GetPropertyOrDefault("margin_bottom", 20);
        var marginLeft = page.GetPropertyOrDefault("margin_left", 25);
        var marginRight = page.GetPropertyOrDefault("margin_right", 15);

        // Theme properties with safe defaults
        var fontFamily = theme.GetPropertyOrDefault("font_family", "Times New Roman");
        var fontSize = Math.Clamp(theme.GetPropertyOrDefault("font_size", 12), 8, 24);
        var lineHeight = Math.Clamp(theme.GetPropertyOrDefault("line_height", 15) / 10.0, 1.0, 3.0);
        var textColor = theme.GetPropertyOrDefault("color", "#000000");

        sb.AppendLine($@"
@page {{
    margin: {marginTop}mm {marginRight}mm {marginBottom}mm {marginLeft}mm;
}}

* {{
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}}

body {{
    font-family: '{System.Net.WebUtility.HtmlEncode(fontFamily)}', serif;
    font-size: {fontSize}pt;
    line-height: {lineHeight:F1};
    color: {System.Net.WebUtility.HtmlEncode(textColor)};
}}

h1, h2, h3 {{
    margin-bottom: 8pt;
    line-height: 1.3;
}}

p {{
    margin-bottom: 6pt;
}}

.clause {{
    margin-bottom: 6pt;
}}

table {{
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 8pt;
}}

th, td {{
    border: 1px solid #333;
    padding: 4pt 6pt;
    font-size: {fontSize}pt;
}}

th {{
    background-color: #f5f5f5;
    font-weight: bold;
    text-align: left;
}}

.sig-table td {{
    border: none;
}}

.kv-block div {{
    margin-bottom: 4pt;
}}

.page-header {{
    margin-bottom: 12pt;
}}

.page-footer {{
    margin-top: 12pt;
    font-size: {Math.Max(fontSize - 2, 8)}pt;
    color: #666;
}}
");

        return sb.ToString();
    }
}
