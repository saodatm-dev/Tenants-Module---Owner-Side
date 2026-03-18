using Core.Application.Abstractions.Messaging;

namespace Document.Contract.Contracts.Queries;

/// <summary>
/// Generates a PDF from the contract's resolved body.
/// Returns raw PDF bytes for download.
/// </summary>
public sealed record GenerateContractPdfQuery(Guid ContractId) : IQuery<byte[]>;
