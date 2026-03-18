using System.Text.Json.Serialization;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.MeterReadings.Upsert;

public sealed record UpsertMeterReadingCommand(
	[property: JsonPropertyName("meterId")] Guid MeterId,
	[property: JsonPropertyName("readingDate")] DateOnly? ReadingDate,
	[property: JsonPropertyName("previousValue")] decimal? PreviousValue,
	[property: JsonPropertyName("value")] decimal Value,
	[property: JsonPropertyName("isManual")] bool IsManual,
	[property: JsonPropertyName("note")] string? Note = null) : ICommand<Guid>;
