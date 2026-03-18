using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Domain.ValueObjects;

namespace Core.Infrastructure.Serialization;

/// <summary>
/// Serializes Money as a plain decimal number (so'm).
/// Reads: plain number → so'm.
/// Writes: plain decimal number (e.g., 15000000.00).
/// Tiyin is NOT exposed to API clients.
/// </summary>
public sealed class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var som = reader.GetDecimal();
            return Money.FromSom(som);
        }

        throw new JsonException("Money must be a number (so'm).");
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Amount);
    }
}
