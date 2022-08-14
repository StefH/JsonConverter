namespace JsonConverter.Abstractions;

public interface IJsonConverter
{
    Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task<T?> DeserializeAsync<T>(string text, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null);

    T? Deserialize<T>(string text, IJsonConverterOptions? options = null);

    Task<string> SerializeAsync<T>(T source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    string Serialize<T>(T source, IJsonConverterOptions? options = null);

    Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default);

    bool IsValidJson(Stream stream);

    bool IsValidJson(string input);
}