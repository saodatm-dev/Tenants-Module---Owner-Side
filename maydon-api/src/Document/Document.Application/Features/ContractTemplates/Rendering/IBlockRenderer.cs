using System.Text.Json;

namespace Document.Application.Features.ContractTemplates.Rendering;

/// <summary>
/// Interface for rendering a JSON block to safe HTML.
/// Each block type has its own implementation.
/// </summary>
public interface IBlockRenderer
{
    string BlockType { get; }
    string Render(JsonElement block, BlockRenderContext context);
}

/// <summary>
/// Shared context passed to all block renderers during rendering.
/// </summary>
public sealed class BlockRenderContext
{
    public required BlockRendererFactory Factory { get; init; }
    public required Dictionary<string, object?> ResolvedValues { get; init; }
}
