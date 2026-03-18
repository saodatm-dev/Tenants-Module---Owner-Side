using System.Text.Json;
using System.Text.RegularExpressions;

namespace Document.Application.Features.ContractTemplates.Rendering;

/// <summary>
/// Walks the entire block tree recursively and replaces {{key}} tokens
/// with resolved values from the value map.
/// </summary>
public sealed partial class PlaceholderResolver
{
    private static readonly Regex PlaceholderPattern = PlaceholderRegex();

    [GeneratedRegex(@"\{\{(\w+)\}\}", RegexOptions.Compiled)]
    private static partial Regex PlaceholderRegex();

    /// <summary>
    /// Resolves all {{key}} placeholders in the JSON block tree.
    /// Returns a new JsonElement with all placeholders replaced.
    /// </summary>
    public static JsonElement Resolve(JsonElement root, Dictionary<string, object?> values, bool replaceUnresolved = false)
    {
        var jsonString = root.GetRawText();
        var resolved = ResolveString(jsonString, values, replaceUnresolved);
        using var doc = JsonDocument.Parse(resolved);
        return doc.RootElement.Clone();
    }

    /// <summary>
    /// Resolves all {{key}} tokens in a single string.
    /// When <paramref name="replaceUnresolved"/> is true, unresolved keys become [key] for preview.
    /// </summary>
    public static string ResolveString(string input, Dictionary<string, object?> values, bool replaceUnresolved = false)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return PlaceholderPattern.Replace(input, match =>
        {
            var key = match.Groups[1].Value;
            if (values.TryGetValue(key, out var value))
            {
                return value switch
                {
                    null => "",
                    string s => EscapeJsonString(s),
                    JsonElement je => je.ValueKind == JsonValueKind.String
                        ? EscapeJsonString(je.GetString() ?? "")
                        : je.GetRawText(),
                    _ => EscapeJsonString(value.ToString() ?? "")
                };
            }
            // In preview mode, show readable label; otherwise keep raw mustache for editor
            return replaceUnresolved ? $"[{key}]" : match.Value;
        });
    }

    /// <summary>
    /// Escapes a string for safe embedding inside a JSON string value.
    /// </summary>
    private static string EscapeJsonString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
