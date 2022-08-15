using JsonConverter.Abstractions;
using NetJSON;
using Stef.Validation;
using NetJson = NetJSON.NetJSON;

namespace JsonConverter.NetJSON;

public partial class JsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanRead);

        var streamReader = new StreamReader(stream);
       
        return options == null
            ? NetJson.Deserialize<T>(streamReader)
            : NetJson.Deserialize<T>(streamReader, ConvertOptions(options));
    }

    public T? Deserialize<T>(string text, IJsonConverterOptions? options = null)
    {
        return options == null
            ? NetJson.Deserialize<T>(text)
            : NetJson.Deserialize<T>(text, ConvertOptions(options));
    }

    public string Serialize(object source, IJsonConverterOptions? options = null)
    {
        return options != null ?
            NetJson.Serialize(source, ConvertOptions(options)) :
            NetJson.Serialize(source);
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

    private static NetJSONSettings ConvertOptions(IJsonConverterOptions options)
    {
        return new NetJSONSettings
        {
            Format = options.WriteIndented ? NetJSONFormat.Prettify : NetJSONFormat.Default
        };
    }
}