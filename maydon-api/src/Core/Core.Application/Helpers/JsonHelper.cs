using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Application.Helpers;

public sealed class JsonDateOnlyConverter : JsonConverter<DateOnly>
{
	private readonly string format = "yyyy-MM-dd";

	public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		DateOnly.ParseExact(reader.GetString()!, format, CultureInfo.InvariantCulture);

	public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
		writer.WriteStringValue(value.ToString(format, CultureInfo.InvariantCulture));
}
