using Core.Application.Abstractions.Messaging;
using Document.Contract.ContractTemplates.Responses;

namespace Document.Contract.ContractTemplates.Queries;

/// <summary>
/// Returns the full placeholder catalog grouped by category for frontend sidebar.
/// </summary>
public sealed record GetPlaceholderCatalogQuery : IQuery<PlaceholderCatalogResponse>;
