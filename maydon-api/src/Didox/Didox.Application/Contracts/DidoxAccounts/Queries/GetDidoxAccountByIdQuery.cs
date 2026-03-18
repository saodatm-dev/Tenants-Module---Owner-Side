using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Didox.Application.Contracts.DidoxAccounts.Responses;

namespace Didox.Application.Contracts.DidoxAccounts.Queries;

public record GetDidoxAccountByIdQuery(Guid Id) : IQuery<DidoxAccountResponse>;

