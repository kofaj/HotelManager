using System.Text.Json.Serialization;
using System.Text.Json;

namespace HotelManager.Shared.Services.JsonConverters;

internal class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string serializationFormat = "yyyyMMdd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateOnly.ParseExact(value!, serializationFormat);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(serializationFormat));
}
