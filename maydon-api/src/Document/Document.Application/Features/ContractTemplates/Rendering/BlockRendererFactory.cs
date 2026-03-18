using System.Text.Json;

namespace Document.Application.Features.ContractTemplates.Rendering;

/// <summary>
/// Factory that maps block type string to its renderer implementation.
/// </summary>
public sealed class BlockRendererFactory
{
    private readonly Dictionary<string, IBlockRenderer> _renderers;

    public BlockRendererFactory(IEnumerable<IBlockRenderer> renderers)
    {
        _renderers = renderers.ToDictionary(r => r.BlockType, r => r, StringComparer.OrdinalIgnoreCase);
    }

    public IBlockRenderer GetRenderer(string blockType)
    {
        return _renderers.TryGetValue(blockType, out var renderer)
            ? renderer
            : throw new InvalidOperationException($"Unknown block type: '{blockType}'");
    }

    public bool HasRenderer(string blockType) =>
        _renderers.ContainsKey(blockType);

    /// <summary>
    /// Renders a single block using the appropriate renderer.
    /// Gracefully skips blocks with missing or unknown types.
    /// </summary>
    public string RenderBlock(JsonElement block, BlockRenderContext context)
    {
        var type = block.TryGetProperty("type", out var typeEl)
            ? typeEl.GetString() ?? string.Empty
            : string.Empty;

        if (string.IsNullOrWhiteSpace(type) || !_renderers.TryGetValue(type, out var renderer))
            return string.Empty;

        return renderer.Render(block, context);
    }

    /// <summary>
    /// Renders an array of blocks sequentially.
    /// </summary>
    public string RenderBlocks(JsonElement blocks, BlockRenderContext context)
    {
        if (blocks.ValueKind != JsonValueKind.Array)
            return string.Empty;

        var sb = new System.Text.StringBuilder();
        foreach (var block in blocks.EnumerateArray())
        {
            sb.AppendLine(RenderBlock(block, context));
        }
        return sb.ToString();
    }
}
