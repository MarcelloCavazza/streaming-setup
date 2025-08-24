using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models.Extensions;

public class GuidStringConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var guidString = reader.GetString();
        return guidString != null ? Guid.Parse(guidString) : Guid.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
