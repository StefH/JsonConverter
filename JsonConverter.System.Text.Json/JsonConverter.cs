using System.Text.Json;
using System.Text.Json.Serialization;
using JsonConverter.Abstractions;
using Stef.Validation;

namespace JsonConverter.System.Text.Json;

public class JsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        stream.Seek(0L, SeekOrigin.Begin);
        return JsonSerializer.Deserialize<T>(stream.ToString(), options == null ? null : ConvertOptions(options));
    }

    public async Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        stream.Seek(0L, SeekOrigin.Begin);
        return await JsonSerializer.DeserializeAsync<T>(stream, options == null ? null : ConvertOptions(options), cancellationToken).ConfigureAwait(false);
    }

    public T? Deserialize<T>(string text, IJsonConverterOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(text, ConvertOptions(options));
    }

    public async Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var text = await new StreamReader(stream).ReadToEndAsync().ConfigureAwait(false);
        return IsValidJson(text);
    }

    public Task<bool> IsValidJsonAsync(string input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(IsValidJson(input));
    } 

    private static bool IsValidJson(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        input = input!.Trim();

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

    public string Serialize<T>(T source, IJsonConverterOptions? options)
    {
        return JsonSerializer.Serialize(source, ConvertOptions(options));
    }

    public async Task<string> SerializeAsync<T>(T source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, source, options == null ? null : ConvertOptions(options), cancellationToken);
        stream.Position = 0L;

        using var reader = new StreamReader(stream);
        var endAsync = await reader.ReadToEndAsync();
        return endAsync;
    }

    private static JsonSerializerOptions? ConvertOptions(IJsonConverterOptions? options)
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