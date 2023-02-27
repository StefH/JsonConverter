using JsonConverter.Abstractions;
using Stef.Validation;

namespace JsonConverter.SimpleJson;

public partial class SimpleJsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanRead);

        var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();

        return SimpleJson.DeserializeObject<T?>(text);
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return SimpleJson.DeserializeObject<T?>(text);
    }

    public string Serialize(object value, JsonConverterOptions? options = null)
    {
        return SimpleJson.SerializeObject(value!) ?? string.Empty;
    }

    public bool IsValidJson(Stream stream)
    {
        return IsValidJson(stream.ReadAsString());
    }

    public bool IsValidJson(string input)
    {
        if (input.IsNullOrWhiteSpace())
        {
            return false;
        }

        input = input.Trim();
        if ((!input.StartsWith("{") || !input.EndsWith("}")) && (!input.StartsWith("[") || !input.EndsWith("]")))
        {
            return false;
        }

        return SimpleJson.TryDeserializeObject(input, out _);
    }

    public object? ConvertToDynamicJsonClass(object value)
    {
        throw new NotImplementedException();
    }

    public object? DeserializeToDynamicJsonClass(string text, JsonConverterOptions? options = null)
    {
        throw new NotImplementedException();
    }
}