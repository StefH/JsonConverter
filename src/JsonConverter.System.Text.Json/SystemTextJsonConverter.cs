using System.Text.Json;
using System.Text.Json.Serialization;
using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Extensions;
using JsonConverter.Abstractions.Models;
using Stef.Validation;

namespace JsonConverter.System.Text.Json;

public class SystemTextJsonConverter : IJsonConverter
{
#if NET8_0_OR_GREATER
    private static readonly JsonSerializerOptions DefaultOptions = JsonSerializerOptions.Default;
#else
    private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions();
#endif

    private readonly JsonSerializerOptions? _jsonSerializerOptions;

    public SystemTextJsonConverter()
    {
    }

    /// <summary>
    /// Ctor with user provided <see cref="JsonSerializerOptions"/>
    /// </summary>
    public SystemTextJsonConverter(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        stream.Seek(0L, SeekOrigin.Begin);
        return JsonSerializer.Deserialize<T>(stream, ConvertOptions(options));
    }

    public async Task<T?> DeserializeAsync<T>(Stream stream, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanSeek);

        stream.Seek(0L, SeekOrigin.Begin);
        return await JsonSerializer.DeserializeAsync<T>(stream, ConvertOptions(options), cancellationToken).ConfigureAwait(false);
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(text, ConvertOptions(options));
    }

    public object? Deserialize(string text, Type type, JsonConverterOptions? options = null)
    {
        return JsonSerializer.Deserialize(text, type, ConvertOptions(options));
    }

    public JsonType GetJsonType(Stream stream)
    {
        return JsonTypeHelper.GetJsonType(stream);
    }

    public JsonType GetJsonType(string value)
    {
        return JsonTypeHelper.GetJsonType(value);
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

    public object? ConvertToDynamicJsonClass(object value, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public object? DeserializeToDynamicJsonClass(string text, DynamicJsonClassOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public string Serialize(object value, JsonConverterOptions? options)
    {
        return JsonSerializer.Serialize(value, ConvertOptions(options));
    }

    public Task SerializeAsync(Stream stream, object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        return JsonSerializer.SerializeAsync(stream, value, ConvertOptions(options), cancellationToken);
    }

    public async Task<string> SerializeAsync(object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, value, ConvertOptions(options), cancellationToken);
        stream.Position = 0L;

        using var reader = new StreamReader(stream);

#if NET8_0_OR_GREATER
        return await reader.ReadToEndAsync(cancellationToken);
#else
        return await reader.ReadToEndAsync();
#endif
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
            JsonElement elementValue => elementValue.Deserialize<T>(ConvertOptions(options))!,
            JsonDocument documentValue => documentValue.RootElement.Deserialize<T>(ConvertOptions(options))!,
            _ => Deserialize<T>(Serialize(value, options), options)!
        };
    }

    public object ToJsonToken(object value, JsonConverterOptions? options = null)
    {
        return value switch
        {
            JsonElement elementValue => elementValue,
            JsonDocument documentValue => documentValue.RootElement.Clone(),
            string stringValue => Deserialize<JsonElement>(stringValue, options),
            _ => JsonSerializer.SerializeToElement(value, ConvertOptions(options))
        };
    }

    private JsonSerializerOptions ConvertOptions(JsonConverterOptions? options)
    {
        // Note: DateParseHandling is currently ignored by JsonConverterOptions in this System.Text.Json implementation.
        // To have custom date parsing behavior configured use user-provided JsonSerializerOptions
        // More on that in https://learn.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support

        return new JsonSerializerOptions(_jsonSerializerOptions ?? DefaultOptions)
        {
            PropertyNameCaseInsensitive = options?.PropertyNameCaseInsensitive ?? false,
            WriteIndented = options?.WriteIndented ?? false,
            DefaultIgnoreCondition = options?.IgnoreNullValues == true
                ? JsonIgnoreCondition.WhenWritingNull
                : JsonIgnoreCondition.Never
        };
    }
}