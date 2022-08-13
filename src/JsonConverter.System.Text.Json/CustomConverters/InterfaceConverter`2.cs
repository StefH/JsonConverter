using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConverter.System.Text.Json.CustomConverters;

public class InterfaceConverter<TClass, TInterface> : JsonConverter<TInterface>
    where TClass : class, TInterface
{
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<TClass>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {
    }
}