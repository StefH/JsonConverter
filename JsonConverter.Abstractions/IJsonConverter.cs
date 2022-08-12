namespace JsonConverter.Abstractions;

public interface IJsonConverter
{
    Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null);

    Task<string> SerializeAsync<T>(T source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    string Serialize<T>(T source, IJsonConverterOptions? options = null);

    Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default);

    Task<bool> IsValidJsonAsync(string input, CancellationToken cancellationToken = default);
}