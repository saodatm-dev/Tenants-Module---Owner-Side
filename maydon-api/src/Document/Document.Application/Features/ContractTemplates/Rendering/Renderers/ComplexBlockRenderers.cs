using System.Net;
using System.Text;
using System.Text.Json;

namespace Document.Application.Features.ContractTemplates.Rendering.Renderers;

public sealed class RowBlockRenderer : IBlockRenderer
{
    public string BlockType => "row";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var gap = block.GetPropertyOrDefault("gap", 16);

        var sb = new StringBuilder();
        sb.Append($"<div style=\"display:flex;justify-content:space-between;align-items:center;gap:{gap}px\">");

        if (block.TryGetProperty("columns", out var columns) && columns.ValueKind == JsonValueKind.Array)
        {
            foreach (var column in columns.EnumerateArray())
            {
                sb.Append("<div style=\"flex:1\">");
                sb.Append(context.Factory.RenderBlock(column, context));
                sb.Append("</div>");
            }
        }

        sb.Append("</div>");
        return sb.ToString();
    }
}

public sealed class TableBlockRenderer : IBlockRenderer
{
    public string BlockType => "table";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var sb = new StringBuilder();
        sb.Append("<table>");

        // Headers — support both "columns" (simple string[]) and "headers" (object[] with label/width/align)
        if (block.TryGetProperty("columns", out var columns) && columns.ValueKind == JsonValueKind.Array)
        {
            sb.Append("<thead><tr>");
            foreach (var col in columns.EnumerateArray())
            {
                var label = WebUtility.HtmlEncode(col.GetString() ?? "");
                sb.Append($"<th>{label}</th>");
            }
            sb.Append("</tr></thead>");
        }
        else if (block.TryGetProperty("headers", out var headers) && headers.ValueKind == JsonValueKind.Array)
        {
            sb.Append("<thead><tr>");
            foreach (var header in headers.EnumerateArray())
            {
                var label = WebUtility.HtmlEncode(header.GetPropertyOrDefault("label", ""));
                var width = header.GetPropertyOrDefault("width", "");
                var align = header.GetPropertyOrDefault("align", "left");
                var style = string.IsNullOrEmpty(width) ? "" : $"width:{WebUtility.HtmlEncode(width)};";
                style += $"text-align:{align}";
                sb.Append($"<th style=\"{style}\">{label}</th>");
            }
            sb.Append("</tr></thead>");
        }

        sb.Append("<tbody>");

        // Dynamic table from source data
        if (block.TryGetProperty("source", out var sourceEl))
        {
            var sourceKey = sourceEl.GetString() ?? "";
            if (context.ResolvedValues.TryGetValue(sourceKey, out var sourceData) && sourceData is JsonElement arrayData && arrayData.ValueKind == JsonValueKind.Array)
            {
                var index = 1;
                foreach (var item in arrayData.EnumerateArray())
                {
                    sb.Append("<tr>");
                    if (block.TryGetProperty("row_template", out var rowTemplate) && rowTemplate.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var cell in rowTemplate.EnumerateArray())
                        {
                            var field = cell.GetPropertyOrDefault("field", "");
                            string cellValue;
                            if (field == "#index")
                            {
                                cellValue = index.ToString();
                            }
                            else if (item.TryGetProperty(field, out var fieldValue))
                            {
                                cellValue = fieldValue.ToString();
                            }
                            else
                            {
                                cellValue = "";
                            }
                            sb.Append($"<td>{WebUtility.HtmlEncode(cellValue)}</td>");
                        }
                    }
                    sb.Append("</tr>");
                    index++;
                }
            }
        }
        // Static rows
        else if (block.TryGetProperty("rows", out var rows) && rows.ValueKind == JsonValueKind.Array)
        {
            foreach (var row in rows.EnumerateArray())
            {
                sb.Append("<tr>");
                if (row.ValueKind == JsonValueKind.Array)
                {
                    foreach (var cell in row.EnumerateArray())
                    {
                        sb.Append($"<td>{WebUtility.HtmlEncode(cell.ToString())}</td>");
                    }
                }
                sb.Append("</tr>");
            }
        }

        sb.Append("</tbody></table>");
        return sb.ToString();
    }
}

public sealed class KeyValueBlockRenderer : IBlockRenderer
{
    public string BlockType => "key_value";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var sb = new StringBuilder();
        sb.Append("<div class=\"kv-block\">");

        if (block.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in items.EnumerateArray())
            {
                var label = WebUtility.HtmlEncode(
                    item.GetPropertyOrDefault("label", "") is { Length: > 0 } lbl
                        ? lbl
                        : item.GetPropertyOrDefault("key", ""));
                var value = WebUtility.HtmlEncode(item.GetPropertyOrDefault("value", ""));

                if (string.IsNullOrEmpty(label))
                {
                    sb.Append($"<div>{value}</div>");
                }
                else
                {
                    sb.Append($"<div><strong>{label}:</strong> {value}</div>");
                }
            }
        }

        sb.Append("</div>");
        return sb.ToString();
    }
}

public sealed class SignatureBlockRenderer : IBlockRenderer
{
    public string BlockType => "signature_block";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var sb = new StringBuilder();
        sb.Append("<table class=\"sig-table\" style=\"width:100%\"><tr>");

        if (block.TryGetProperty("parties", out var parties) && parties.ValueKind == JsonValueKind.Array)
        {
            foreach (var party in parties.EnumerateArray())
            {
                var title = WebUtility.HtmlEncode(
                    party.GetPropertyOrDefault("title", "") is { Length: > 0 } t
                        ? t
                        : party.GetPropertyOrDefault("role", ""));
                var signer = WebUtility.HtmlEncode(
                    party.GetPropertyOrDefault("signer", "") is { Length: > 0 } s
                        ? s
                        : party.GetPropertyOrDefault("name", ""));

                sb.Append("<td style=\"width:50%;vertical-align:top;border:none;padding:8px 16px\">");
                sb.Append($"<strong>{title}:</strong><br>");

                if (party.TryGetProperty("fields", out var fields) && fields.ValueKind == JsonValueKind.Array)
                {
                    foreach (var field in fields.EnumerateArray())
                    {
                        var label = WebUtility.HtmlEncode(field.GetPropertyOrDefault("label", ""));
                        var value = WebUtility.HtmlEncode(field.GetPropertyOrDefault("value", ""));

                        if (string.IsNullOrEmpty(label))
                        {
                            sb.Append($"{value}<br>");
                        }
                        else
                        {
                            sb.Append($"{label}: {value}<br>");
                        }
                    }
                }

                sb.Append($"<br><br>_____________ {signer}");
                sb.Append("</td>");
            }
        }

        sb.Append("</tr></table>");
        return sb.ToString();
    }
}

public sealed class IfBlockRenderer : IBlockRenderer
{
    public string BlockType => "if_block";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var conditionKey = block.GetPropertyOrDefault("condition", "");

        // Check if condition is truthy
        var isTruthy = false;
        if (context.ResolvedValues.TryGetValue(conditionKey, out var condValue))
        {
            isTruthy = condValue switch
            {
                null => false,
                bool b => b,
                string s => !string.IsNullOrEmpty(s),
                JsonElement je => je.ValueKind != JsonValueKind.Null && je.ValueKind != JsonValueKind.False &&
                                  !(je.ValueKind == JsonValueKind.String && string.IsNullOrEmpty(je.GetString())) &&
                                  !(je.ValueKind == JsonValueKind.Array && je.GetArrayLength() == 0),
                _ => true
            };
        }

        if (!isTruthy)
            return string.Empty;

        if (block.TryGetProperty("children", out var children) && children.ValueKind == JsonValueKind.Array)
        {
            return context.Factory.RenderBlocks(children, context);
        }

        return string.Empty;
    }
}

public sealed class EachBlockRenderer : IBlockRenderer
{
    public string BlockType => "each_block";

    public string Render(JsonElement block, BlockRenderContext context)
    {
        var sourceKey = block.GetPropertyOrDefault("source", "");

        if (!context.ResolvedValues.TryGetValue(sourceKey, out var sourceData))
            return string.Empty;

        JsonElement arrayData;
        if (sourceData is JsonElement je && je.ValueKind == JsonValueKind.Array)
        {
            arrayData = je;
        }
        else
        {
            return string.Empty;
        }

        if (!block.TryGetProperty("item_template", out var itemTemplate) || itemTemplate.ValueKind != JsonValueKind.Array)
            return string.Empty;

        var sb = new StringBuilder();
        var index = 0;
        foreach (var item in arrayData.EnumerateArray())
        {
            // Create a child context with the item properties overlaid
            var childValues = new Dictionary<string, object?>(context.ResolvedValues)
            {
                ["#index"] = (index + 1).ToString(),
                ["#item"] = item
            };

            // Copy item properties into the child context
            if (item.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in item.EnumerateObject())
                {
                    childValues[prop.Name] = prop.Value;
                }
            }

            var childContext = new BlockRenderContext
            {
                Factory = context.Factory,
                ResolvedValues = childValues
            };

            sb.Append(context.Factory.RenderBlocks(itemTemplate, childContext));
            index++;
        }

        return sb.ToString();
    }
}
