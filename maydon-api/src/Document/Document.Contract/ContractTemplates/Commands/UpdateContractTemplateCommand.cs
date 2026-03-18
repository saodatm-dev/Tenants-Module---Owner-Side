using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Document.Contract.ContractTemplates.Enums;

namespace Document.Contract.ContractTemplates.Commands;

public sealed record UpdateContractTemplateCommand : ICommand
{
    public Guid Id { get; init; }
    public string? Code { get; init; }
    public JsonDocument? Name { get; init; }
    public JsonDocument? Description { get; init; }
    public JsonDocument? Page { get; init; }
    public JsonDocument? Theme { get; init; }
    public JsonDocument? Header { get; init; }
    public JsonDocument? Footer { get; init; }
    public JsonDocument? Bodies { get; init; }
    public JsonDocument? ManualFields { get; init; }
    public ContractTemplateScope? Scope { get; init; }
    public ContractTemplateCategory? Category { get; init; }
    public bool? IsActive { get; init; }
}
