using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;

namespace Didox.Application.Contracts.DidoxDocuments.Queries;

public record GetDocumentPdfQuery(string DocumentId) : IQuery<byte[]>;

