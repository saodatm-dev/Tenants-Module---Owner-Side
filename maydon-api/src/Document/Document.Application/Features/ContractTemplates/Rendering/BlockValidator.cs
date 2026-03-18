using System.Text.Json;

namespace Document.Application.Features.ContractTemplates.Rendering;

/// <summary>
/// Validates JSON blocks before rendering.
/// Rejects unknown types, missing required fields, excessive nesting, and disallowed values.
/// </summary>
public sealed class BlockValidator
{
    private static readonly HashSet<string> AllowedBlockTypes =
    [
        "title", "subtitle", "paragraph", "clause", "text",
        "row", "spacer", "divider", "page_break", "table",
        "key_value", "signature_block", "image",
        "if_block", "each_block"
    ];

    private static readonly HashSet<string> AllowedAlignValues =
        ["left", "center", "right", "justify"];

    private static readonly HashSet<string> AllowedFontFamilies =
        ["Times New Roman", "Arial", "Helvetica", "Georgia", "Roboto", "Inter", "Noto Sans"];

    private const int MaxBlockCount = 200;
    private const int MaxNestingDepth = 5;

    public List<string> Errors { get; } = [];

    /// <summary>
    /// Validates the entire body blocks array.
    /// Returns true if valid, false with errors populated.
    /// </summary>
    public bool ValidateBody(JsonElement blocks)
    {
        Errors.Clear();

        if (blocks.ValueKind != JsonValueKind.Array)
        {
            Errors.Add("Body must be an array of blocks.");
            return false;
        }

        var blockCount = 0;
        foreach (var block in blocks.EnumerateArray())
        {
            ValidateBlock(block, depth: 0, ref blockCount);
        }

        if (blockCount > MaxBlockCount)
        {
            Errors.Add($"Block count ({blockCount}) exceeds maximum ({MaxBlockCount}).");
        }

        return Errors.Count == 0;
    }

    /// <summary>
    /// Validates the theme configuration.
    /// </summary>
    public bool ValidateTheme(JsonElement theme)
    {
        Errors.Clear();

        if (theme.ValueKind != JsonValueKind.Object)
        {
            Errors.Add("Theme must be a JSON object.");
            return false;
        }

        var fontFamily = theme.GetPropertyOrDefault("font_family", "");
        if (!string.IsNullOrEmpty(fontFamily) && !AllowedFontFamilies.Contains(fontFamily))
        {
            Errors.Add($"Font family '{fontFamily}' is not allowed. Allowed: {string.Join(", ", AllowedFontFamilies)}");
        }

        return Errors.Count == 0;
    }

    private void ValidateBlock(JsonElement block, int depth, ref int blockCount)
    {
        blockCount++;

        if (depth > MaxNestingDepth)
        {
            Errors.Add($"Block nesting depth ({depth}) exceeds maximum ({MaxNestingDepth}).");
            return;
        }

        if (block.ValueKind != JsonValueKind.Object)
        {
            Errors.Add("Each block must be a JSON object.");
            return;
        }

        if (!block.TryGetProperty("type", out var typeEl) || typeEl.ValueKind != JsonValueKind.String)
        {
            Errors.Add("Block is missing required 'type' property.");
            return;
        }

        var type = typeEl.GetString()!;
        if (!AllowedBlockTypes.Contains(type))
        {
            Errors.Add($"Unknown block type: '{type}'.");
            return;
        }

        // Validate align property if present
        if (block.TryGetProperty("align", out var alignEl) && alignEl.ValueKind == JsonValueKind.String)
        {
            var align = alignEl.GetString() ?? "";
            if (!AllowedAlignValues.Contains(align))
            {
                Errors.Add($"Invalid align value '{align}' in {type} block. Allowed: {string.Join(", ", AllowedAlignValues)}");
            }
        }

        // Validate type-specific requirements
        switch (type)
        {
            case "title":
            case "subtitle":
            case "paragraph":
            case "text":
                if (!block.TryGetProperty("value", out _))
                    Errors.Add($"Block type '{type}' requires a 'value' property.");
                break;

            case "clause":
                if (!block.TryGetProperty("number", out _))
                    Errors.Add("Block type 'clause' requires a 'number' property.");
                if (!block.TryGetProperty("value", out _))
                    Errors.Add("Block type 'clause' requires a 'value' property.");
                break;

            case "row":
                if (block.TryGetProperty("columns", out var cols) && cols.ValueKind == JsonValueKind.Array)
                {
                    foreach (var col in cols.EnumerateArray())
                        ValidateBlock(col, depth + 1, ref blockCount);
                }
                break;

            case "if_block":
                if (!block.TryGetProperty("condition", out _))
                    Errors.Add("Block type 'if_block' requires a 'condition' property.");
                if (block.TryGetProperty("children", out var children) && children.ValueKind == JsonValueKind.Array)
                {
                    foreach (var child in children.EnumerateArray())
                        ValidateBlock(child, depth + 1, ref blockCount);
                }
                break;

            case "each_block":
                if (!block.TryGetProperty("source", out _))
                    Errors.Add("Block type 'each_block' requires a 'source' property.");
                if (block.TryGetProperty("item_template", out var template) && template.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in template.EnumerateArray())
                        ValidateBlock(item, depth + 1, ref blockCount);
                }
                break;

            case "table":
                if (!block.TryGetProperty("headers", out _) && !block.TryGetProperty("columns", out _))
                    Errors.Add("Block type 'table' requires a 'headers' or 'columns' property.");
                break;

            case "signature_block":
                if (!block.TryGetProperty("parties", out _))
                    Errors.Add("Block type 'signature_block' requires a 'parties' property.");
                break;
        }
    }
}
