using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Models;
using LitJson;
using Stef.Validation;

namespace JsonConverter.LitJson;

public partial class LitJsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);

        return JsonMapper.ToObject<T>(stream.ReadAsString());
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return JsonMapper.ToObject<T>(text);
    }

    public string Serialize(object value, JsonConverterOptions? options = null)
    {
        return JsonMapper.ToJson(value);
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
            _ = new JsonData(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public object ConvertToDynamicJsonClass(object value, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public object DeserializeToDynamicJsonClass(string text, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }
}