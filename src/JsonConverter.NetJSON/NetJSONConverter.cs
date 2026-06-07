using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Extensions;
using JsonConverter.Abstractions.Models;
using NetJSON;
using Stef.Validation;
using NetJson = NetJSON.NetJSON;

namespace JsonConverter.NetJSON;

public partial class NetJSONConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanRead);

        var streamReader = new StreamReader(stream);
       
        return options == null
            ? NetJson.Deserialize<T>(streamReader)
            : NetJson.Deserialize<T>(streamReader, ConvertOptions(options));
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return options == null
            ? NetJson.Deserialize<T>(text)
            : NetJson.Deserialize<T>(text, ConvertOptions(options));
    }

    public object? Deserialize(string text, Type type, JsonConverterOptions? options = null)
    {
        return options == null
            ? NetJson.Deserialize(type, text)
            : NetJson.Deserialize(type, text, ConvertOptions(options));
    }

    public string Serialize(object value, JsonConverterOptions? options = null)
    {
        return options != null ?
            NetJson.Serialize(value, ConvertOptions(options)) :
            NetJson.Serialize(value);
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
        Guard.NotNull(stream);

        return IsValidJson(stream.ReadAsString());
    }

    public bool IsValidJson(string input)
    {
        if (input.IsNullOrWhiteSpaceInternal())
        {
            return false;
        }

        input = input.Trim();
        if ((!input.StartsWith("{") || !input.EndsWith("}")) && (!input.StartsWith("[") || !input.EndsWith("]")))
        {
            return false;
        }

        try
        {
            _ = NetJson.Deserialize<object>(input);
            return true;
        }
        catch
        {
            return false;
        }
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
            string stringValue => Deserialize<object>(stringValue, options)!,
            _ => Deserialize<object>(Serialize(value, options), options)!
        };
    }

    private static NetJSONSettings ConvertOptions(JsonConverterOptions options)
    {
        return new NetJSONSettings
        {
            Format = options.WriteIndented ? NetJSONFormat.Prettify : NetJSONFormat.Default
        };
    }
}