using Core.Application.Abstractions.Messaging;
using Document.Contract.Contracts.Responses;

namespace Document.Contract.Contracts.Queries;

/// <summary>
/// Pre-fills a template with resolved placeholder values from lease and company data.
/// Returns the resolved body and all placeholder values for UI editing.
/// </summary>
public sealed record PrefillContractQuery(Guid TemplateId, Guid LeaseId, string Language) : IQuery<PrefillContractResponse>;
