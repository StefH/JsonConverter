using JsonConverter.Abstractions;
using Stef.Validation;

namespace JsonConverter.SimpleJson;

public class JsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanRead);

        var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();

        return SimpleJson.DeserializeObject<T?>(text);
    }

    public T? Deserialize<T>(string text, IJsonConverterOptions? options = null)
    {
        return SimpleJson.DeserializeObject<T?>(text);
    }

    public Task<T?> DeserializeAsync<T>(string text, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Deserialize<T>(text, options));
    }

    public Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Deserialize<T>(stream, options));
    }

    public string Serialize<T>(T source, IJsonConverterOptions? options = null)
    {
        return SimpleJson.SerializeObject(source!) ?? string.Empty;
    }

    public Task<string> SerializeAsync<T>(T source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Serialize(source, options));
    }

    public async Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        return IsValidJson(await stream.ReadAsStringAsync().ConfigureAwait(false));
    }

    public Task<bool> IsValidJsonAsync(string input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(IsValidJson(input));
    }

    public bool IsValidJson(Stream stream)
    {
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

        return SimpleJson.TryDeserializeObject(input, out _);
    }
}