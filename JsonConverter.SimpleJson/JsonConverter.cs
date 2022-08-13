using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        return SimpleJson.SerializeObject(source!) ?? string.Empty;
    }

    public Task<string> SerializeAsync<T>(T source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Serialize(source, options));
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

        return SimpleJson.TryDeserializeObject(input, out _);
    }
}