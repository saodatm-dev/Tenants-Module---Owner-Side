using Core.Application.Abstractions.Messaging;
using Document.Contract.Contracts.Responses;

namespace Document.Contract.Contracts.Queries;

/// <summary>
/// Lists all attachments for a given contract.
/// </summary>
public sealed record ListContractAttachmentsQuery(Guid ContractId) : IQuery<IReadOnlyList<ContractAttachmentResponse>>;
