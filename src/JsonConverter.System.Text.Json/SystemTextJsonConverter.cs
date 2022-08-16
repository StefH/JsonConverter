using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonConverter.Abstractions;
using Stef.Validation;

namespace JsonConverter.System.Text.Json;

public class SystemTextJsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        stream.Seek(0L, SeekOrigin.Begin);
        return JsonSerializer.Deserialize<T>(stream, options == null ? null : ConvertOptions(options));
    }

    public async Task<T?> DeserializeAsync<T>(Stream stream, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        stream.Seek(0L, SeekOrigin.Begin);
        return await JsonSerializer.DeserializeAsync<T>(stream, options == null ? null : ConvertOptions(options), cancellationToken).ConfigureAwait(false);
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(text, ConvertOptions(options));
    }

    public async Task<T?> DeserializeAsync<T>(string text, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await JsonSerializer.DeserializeAsync<T>(new MemoryStream(Encoding.UTF8.GetBytes(text)), ConvertOptions(options), cancellationToken);
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
            JsonDocument.Parse(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string Serialize(object source, JsonConverterOptions? options)
    {
        return JsonSerializer.Serialize(source, ConvertOptions(options));
    }

    public async Task<string> SerializeAsync(object source, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, source, options == null ? null : ConvertOptions(options), cancellationToken);
        stream.Position = 0L;

        using var reader = new StreamReader(stream);
        var endAsync = await reader.ReadToEndAsync();
        return endAsync;
    }

    private static JsonSerializerOptions? ConvertOptions(JsonConverterOptions? options)
    {
        if (options == null)
        {
            return null;
        }

        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive,
            WriteIndented = options.WriteIndented,
            DefaultIgnoreCondition = options.IgnoreNullValues ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never
        };
    }
}