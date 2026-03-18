using Core.Application.Abstractions.Messaging;
using Document.Contract.ContractTemplates.Responses;

namespace Document.Contract.ContractTemplates.Queries;

public sealed record GetContractTemplateByIdQuery(Guid Id) : IQuery<ContractTemplateResponse>;
