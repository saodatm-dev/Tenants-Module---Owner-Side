using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;

namespace Didox.Application.Contracts.DidoxDocuments.Queries;

public record GetDocumentHtmlQuery(string DocumentId) : IQuery<string>;
