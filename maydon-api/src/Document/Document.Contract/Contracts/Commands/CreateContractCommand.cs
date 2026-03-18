using Core.Application.Abstractions.Messaging;
using Document.Contract.Contracts.Responses;

namespace Document.Contract.Contracts.Commands;

/// <summary>
/// Creates a new contract in Draft status.
/// The body is the pre-resolved JSONB from the prefill step, potentially edited by the user.
/// </summary>
public sealed record CreateContractCommand(Guid TemplateId, Guid LeaseId, string Language, string Body) : ICommand<ContractResponse>;
