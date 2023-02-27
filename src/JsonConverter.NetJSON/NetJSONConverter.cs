using JsonConverter.Abstractions;
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

    public string Serialize(object value, JsonConverterOptions? options = null)
    {
        return options != null ?
            NetJson.Serialize(value, ConvertOptions(options)) :
            NetJson.Serialize(value);
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

    public object? ConvertToDynamicJsonClass(object value)
    {
        throw new NotImplementedException();
    }

    public object? DeserializeToDynamicJsonClass(string text, JsonConverterOptions? options = null)
    {
        throw new NotImplementedException();
    }

    private static NetJSONSettings ConvertOptions(JsonConverterOptions options)
    {
        return new NetJSONSettings
        {
            Format = options.WriteIndented ? NetJSONFormat.Prettify : NetJSONFormat.Default
        };
    }
}