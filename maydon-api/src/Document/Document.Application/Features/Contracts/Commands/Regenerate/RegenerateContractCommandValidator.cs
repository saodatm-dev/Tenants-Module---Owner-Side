using System.Text.Json;
using Document.Contract.Contracts.Commands;
using FluentValidation;

namespace Document.Application.Features.Contracts.Commands.Regenerate;

public sealed class RegenerateContractCommandValidator : AbstractValidator<RegenerateContractCommand>
{
    public RegenerateContractCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("ContractId is required");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Contract body is required")
            .Must(BeValidJson)
            .WithMessage("Contract body must be a valid JSON array or object");
    }

    private static bool BeValidJson(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return false;

        try
        {
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.ValueKind is JsonValueKind.Array or JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
