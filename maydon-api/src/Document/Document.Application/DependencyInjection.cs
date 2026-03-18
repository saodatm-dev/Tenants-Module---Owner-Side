using Core.Application.Abstractions.Messaging;
using Document.Application.Features.Contracts.Export;
using Document.Application.Features.ContractTemplates.Rendering;
using Document.Application.Features.ContractTemplates.Rendering.Renderers;
using Document.Contract.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Document.Application;

/// <summary>
/// Dependency injection configuration for Document Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Document Application services.
    /// CQRS handlers are auto-registered by AddCoreApplication assembly scanning.
    /// </summary>
    public static IServiceCollection AddDocumentApplication(this IServiceCollection services)
    {


        // Contract template block rendering engine
        services.AddSingleton<IBlockRenderer, TitleBlockRenderer>();
        services.AddSingleton<IBlockRenderer, SubtitleBlockRenderer>();
        services.AddSingleton<IBlockRenderer, ParagraphBlockRenderer>();
        services.AddSingleton<IBlockRenderer, ClauseBlockRenderer>();
        services.AddSingleton<IBlockRenderer, TextBlockRenderer>();
        services.AddSingleton<IBlockRenderer, SpacerBlockRenderer>();
        services.AddSingleton<IBlockRenderer, DividerBlockRenderer>();
        services.AddSingleton<IBlockRenderer, PageBreakBlockRenderer>();
        services.AddSingleton<IBlockRenderer, ImageBlockRenderer>();
        services.AddSingleton<IBlockRenderer, RowBlockRenderer>();
        services.AddSingleton<IBlockRenderer, TableBlockRenderer>();
        services.AddSingleton<IBlockRenderer, KeyValueBlockRenderer>();
        services.AddSingleton<IBlockRenderer, SignatureBlockRenderer>();
        services.AddSingleton<IBlockRenderer, IfBlockRenderer>();
        services.AddSingleton<IBlockRenderer, EachBlockRenderer>();
        services.AddSingleton<BlockRendererFactory>();

        // Contract export handler (InitiateDocumentExport → PDF → DocumentExportRequested)
        services.AddScoped<IIntegrationEventHandler<InitiateDocumentExport>, ContractExportHandler>();

        return services;
    }
}
