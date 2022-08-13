using JsonConverter.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stef.Validation;

namespace JsonConverter.Newtonsoft.Json;

public class JsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanRead);

        using var streamReader = new StreamReader(stream);
        using var jsonTextReader = new JsonTextReader(streamReader);
        var jsonSerializer = new JsonSerializer();

        if (options != null)
        {
            var serializerSettings = ConvertOptions(options);
            jsonSerializer.Formatting = serializerSettings.Formatting;
            jsonSerializer.NullValueHandling = serializerSettings.NullValueHandling;
        }

        return jsonSerializer.Deserialize<T>(jsonTextReader);
    }

    public T? Deserialize<T>(string text, IJsonConverterOptions? options = null)
    {
        return options == null
            ? JsonConvert.DeserializeObject<T>(text)
            : JsonConvert.DeserializeObject<T>(text, ConvertOptions(options));
    }

    public Task<T?> DeserializeAsync<T>(string text, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Deserialize<T>(text, options));
    }

    public Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Deserialize<T>(stream, options));
    }

    public async Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        var text = await new StreamReader(stream).ReadToEndAsync().ConfigureAwait(false);
        return IsValidJson(text);
    }

    public Task<bool> IsValidJsonAsync(string input, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(input);

        return Task.FromResult(IsValidJson(input));
    }

    public string Serialize<T>(T source, IJsonConverterOptions? options = null)
    {
        return options != null ?
            JsonConvert.SerializeObject(source, ConvertOptions(options)) :
            JsonConvert.SerializeObject(source);
    }

    public Task<string> SerializeAsync<T>(T source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Serialize(source, options));
    }

    private static JsonSerializerSettings ConvertOptions(IJsonConverterOptions options)
    {
        return new JsonSerializerSettings
        {
            Formatting = options.WriteIndented ? Formatting.Indented : Formatting.None,
            NullValueHandling = options.IgnoreNullValues ? NullValueHandling.Include : NullValueHandling.Ignore
        };
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
            JToken.Parse(input);
            return true;
        }
        catch
        {
            return false;
        }
    }
}