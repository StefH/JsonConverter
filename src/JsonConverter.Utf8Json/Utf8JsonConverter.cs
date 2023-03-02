using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Models;
using Stef.Validation;
using Utf8Json;
#if !NET6_0
using Nito.AsyncEx;
#endif

namespace JsonConverter.Utf8Json;

public class Utf8JsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        return JsonSerializer.Deserialize<T>(stream);
    }

    public async Task<T?> DeserializeAsync<T>(Stream stream, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        stream.Seek(0L, SeekOrigin.Begin);
        return await JsonSerializer.DeserializeAsync<T>(stream).WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(text);
    }

    public async Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        return IsValidJson(await stream.ReadAsStringAsync().ConfigureAwait(false));
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
            JsonSerializer.Deserialize<object>(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string Serialize(object value, JsonConverterOptions? options)
    {
        return options?.WriteIndented == true ?
            JsonSerializer.PrettyPrint(JsonSerializer.Serialize(value)) :
            JsonSerializer.ToJsonString(value);
    }

    public Task SerializeAsync(Stream stream, object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanWrite);

        if (options?.WriteIndented != true)
        {
            return JsonSerializer.SerializeAsync(stream, value).WaitAsync(cancellationToken);
        }

        var bytes = JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(value));
        return stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).WaitAsync(cancellationToken);
    }


    public Task<string> SerializeAsync(object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.ToJsonString(value);
        return Task.FromResult(options?.WriteIndented == true ? JsonSerializer.PrettyPrint(json) : json);
    }

    public object? ConvertToDynamicJsonClass(object value, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public object? DeserializeToDynamicJsonClass(string text, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }
}