using System.Text;
using JsonConverter.Abstractions;
using ServiceStack.Text;
using Stef.Validation;
using ServiceStackJsonSerializer = ServiceStack.Text.JsonSerializer;
#if !NET6_0
using Nito.AsyncEx;
#endif

namespace JsonConverter.ServiceStack.Text;

public class ServiceStackJsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        return ServiceStackJsonSerializer.DeserializeFromStream<T>(stream);
    }

    public async Task<T?> DeserializeAsync<T>(Stream stream, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        return await ServiceStackJsonSerializer.DeserializeFromStreamAsync<T>(stream).WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return ServiceStackJsonSerializer.DeserializeFromString<T>(text);
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
            JsonObject.Parse(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string Serialize(object value, JsonConverterOptions? options)
    {
        var json = ServiceStackJsonSerializer.SerializeToString(value);
        return options?.WriteIndented == true ? json.IndentJson() : json;
    }

    public Task SerializeAsync(Stream stream, object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        if (options?.WriteIndented != true)
        {
            ServiceStackJsonSerializer.SerializeToStream(value, stream);
        }

        var json = Serialize(value, options);
        var bytes = Encoding.UTF8.GetBytes(json);
        return stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).WaitAsync(cancellationToken);
    }

    public Task<string> SerializeAsync(object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Serialize(value, options));
    }
}