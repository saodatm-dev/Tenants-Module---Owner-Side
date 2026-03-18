using System.Net;
using System.Text.Json;

namespace Document.Application.Features.ContractTemplates.Rendering.Renderers;

public sealed class TitleBlockRenderer : IBlockRenderer
{
    public string BlockType => "title";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var value = WebUtility.HtmlEncode(block.GetPropertyOrDefault("value", ""));
        var align = block.GetPropertyOrDefault("align", "center");
        var level = block.GetPropertyOrDefault("level", 1);

        level = Math.Clamp(level, 1, 3);
        var tag = $"h{level}";

        return $"<{tag} style=\"text-align:{align}\">{value}</{tag}>";
    }
}

public sealed class SubtitleBlockRenderer : IBlockRenderer
{
    public string BlockType => "subtitle";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var value = WebUtility.HtmlEncode(block.GetPropertyOrDefault("value", ""));
        var align = block.GetPropertyOrDefault("align", "left");

        return $"<h2 style=\"text-align:{align}\">{value}</h2>";
    }
}

public sealed class ParagraphBlockRenderer : IBlockRenderer
{
    public string BlockType => "paragraph";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var value = WebUtility.HtmlEncode(block.GetPropertyOrDefault("value", ""));
        var align = block.GetPropertyOrDefault("align", "justify");
        var indent = block.GetPropertyOrDefault("indent", false);

        var indentStyle = indent ? "text-indent:1.5em;" : "";
        return $"<p style=\"{indentStyle}text-align:{align}\">{value}</p>";
    }
}

public sealed class ClauseBlockRenderer : IBlockRenderer
{
    public string BlockType => "clause";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var value = WebUtility.HtmlEncode(block.GetPropertyOrDefault("value", ""));
        var number = WebUtility.HtmlEncode(block.GetPropertyOrDefault("number", ""));

        return $"<p class=\"clause\"><strong>{number}.</strong> {value}</p>";
    }
}

public sealed class TextBlockRenderer : IBlockRenderer
{
    public string BlockType => "text";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var value = WebUtility.HtmlEncode(block.GetPropertyOrDefault("value", ""));
        var bold = block.GetPropertyOrDefault("bold", false);
        var italic = block.GetPropertyOrDefault("italic", false);
        var align = block.GetPropertyOrDefault("align", "left");
        var fontSize = Math.Clamp(block.GetPropertyOrDefault("font_size", 12), 8, 24);

        var styles = $"font-size:{fontSize}pt;text-align:{align};";
        if (bold) styles += "font-weight:bold;";
        if (italic) styles += "font-style:italic;";

        return $"<span style=\"{styles}\">{value}</span>";
    }
}

public sealed class SpacerBlockRenderer : IBlockRenderer
{
    public string BlockType => "spacer";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var height = Math.Clamp(block.GetPropertyOrDefault("height", 16), 4, 100);
        return $"<div style=\"height:{height}px\"></div>";
    }
}

public sealed class DividerBlockRenderer : IBlockRenderer
{
    public string BlockType => "divider";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var style = block.GetPropertyOrDefault("style", "solid");
        var color = block.GetPropertyOrDefault("color", "#000000");

        var allowedStyles = new HashSet<string> { "solid", "dashed", "dotted" };
        if (!allowedStyles.Contains(style)) style = "solid";

        return $"<hr style=\"border-style:{style};border-color:{WebUtility.HtmlEncode(color)}\">";
    }
}

public sealed class PageBreakBlockRenderer : IBlockRenderer
{
    public string BlockType => "page_break";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        return "<div style=\"page-break-after:always\"></div>";
    }
}

public sealed class ImageBlockRenderer : IBlockRenderer
{
    public string BlockType => "image";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var src = WebUtility.HtmlEncode(block.GetPropertyOrDefault("src", ""));
        var width = Math.Clamp(block.GetPropertyOrDefault("width", 100), 20, 800);
        var align = block.GetPropertyOrDefault("align", "left");

        return $"<div style=\"text-align:{align}\"><img src=\"{src}\" style=\"width:{width}px\" alt=\"\"></div>";
    }
}
