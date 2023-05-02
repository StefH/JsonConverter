using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Models;
using LitJson;
using Stef.Validation;

namespace JsonConverter.LitJson;

public class LitJsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return JsonMapper.ToObject<T>(text);
    }

    public string Serialize(object value, JsonConverterOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public bool IsValidJson(Stream stream)
    {
        Guard.NotNull(stream);

        return IsValidJson(stream.ReadAsString());
    }

    public bool IsValidJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
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

    public object? ConvertToDynamicJsonClass(object value, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public object? DeserializeToDynamicJsonClass(string text, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<T?> DeserializeAsync<T>(Stream stream, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SerializeAsync(Stream stream, object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> SerializeAsync(object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}