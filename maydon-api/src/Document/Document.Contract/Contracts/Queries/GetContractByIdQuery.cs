using Core.Application.Abstractions.Messaging;
using Document.Contract.Contracts.Responses;

namespace Document.Contract.Contracts.Queries;

/// <summary>
/// Gets a contract by ID with financial items.
/// </summary>
public sealed record GetContractByIdQuery(Guid Id) : IQuery<ContractResponse>;
