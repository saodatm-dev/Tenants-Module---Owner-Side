using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;

namespace Didox.Application.Contracts.DidoxDocuments.Queries;

public record GetDocumentJsonQuery(string DocumentId) : IQuery<JsonDocument>;

