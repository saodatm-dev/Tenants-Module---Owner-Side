using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Document.Contract.ContractTemplates.Enums;
using Document.Contract.ContractTemplates.Responses;

namespace Document.Contract.ContractTemplates.Commands;

public sealed record CreateContractTemplateCommand : ICommand<ContractTemplateResponse>
{
    public required string Code { get; init; }
    public required JsonDocument Name { get; init; }
    public JsonDocument? Description { get; init; }
    public required JsonDocument Page { get; init; }
    public required JsonDocument Theme { get; init; }
    public JsonDocument? Header { get; init; }
    public JsonDocument? Footer { get; init; }
    public required JsonDocument Bodies { get; init; }
    public JsonDocument? ManualFields { get; init; }
    public required ContractTemplateScope Scope { get; init; }
    public required ContractTemplateCategory Category { get; init; }
}
