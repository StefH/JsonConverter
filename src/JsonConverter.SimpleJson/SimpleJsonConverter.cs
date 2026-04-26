using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Models;
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

    public object? Deserialize(string text, Type type, JsonConverterOptions? options = null)
    {
        return SimpleJson.DeserializeObject(text, type);
    }

    public string Serialize(object value, JsonConverterOptions? options = null)
    {
        return SimpleJson.SerializeObject(value!) ?? string.Empty;
    }

    public JsonType GetJsonType(Stream stream)
    {
        return JsonTypeHelper.GetJsonType(stream);
    }

    public JsonType GetJsonType(string value)
    {
        return JsonTypeHelper.GetJsonType(value);
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

    public object? ConvertToDynamicJsonClass(object value, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public object? DeserializeToDynamicJsonClass(string text, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public T ParseJsonToken<T>(object? value, JsonConverterOptions? options = null)
    {
        if (value == null)
        {
            return default!;
        }

        if (value.GetType() == typeof(T))
        {
            return (T)value;
        }

        return value switch
        {
            string stringValue => Deserialize<T>(stringValue, options)!,
            _ => Deserialize<T>(Serialize(value, options), options)!
        };
    }

    public object ToJsonToken(object value, JsonConverterOptions? options = null)
    {
        return value switch
        {
            JsonObject jsonObject => jsonObject,
            JsonArray jsonArray => jsonArray,
            string stringValue => Deserialize<object>(stringValue, options)!,
            _ => Deserialize<object>(Serialize(value, options), options)!
        };
    }
}