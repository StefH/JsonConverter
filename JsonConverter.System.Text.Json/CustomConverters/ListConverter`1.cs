using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConverter.System.Text.Json.CustomConverters;

public class ListConverter<T> : JsonConverter<IList<T>>
{
    public override IList<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<List<T>>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, IList<T> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}