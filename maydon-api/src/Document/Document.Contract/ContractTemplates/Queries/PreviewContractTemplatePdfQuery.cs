using Core.Application.Abstractions.Messaging;

namespace Document.Contract.ContractTemplates.Queries;

/// <summary>
/// Preview PDF without saving. Returns PDF bytes.
/// </summary>
public sealed record PreviewContractTemplatePdfQuery(
    Guid TemplateId,
    string Language,
    string? ManualValues = null
) : IQuery<byte[]>;
