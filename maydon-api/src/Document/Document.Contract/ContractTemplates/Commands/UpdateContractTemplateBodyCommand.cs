using System.Text.Json;
using Core.Application.Abstractions.Messaging;

namespace Document.Contract.ContractTemplates.Commands;

/// <summary>
/// Updates a single language body without touching other languages.
/// </summary>
public sealed record UpdateContractTemplateBodyCommand : ICommand
{
    public required Guid Id { get; init; }
    public required string Language { get; init; }
    public required JsonDocument Blocks { get; init; }
}
