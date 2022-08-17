#if !(NET35 || NET40)
namespace JsonConverter.Abstractions;

public partial interface IJsonConverter
{
    Task<T?> DeserializeAsync<T>(Stream stream, JsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task<T?> DeserializeAsync<T>(string text, JsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task SerializeAsync(Stream stream, object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task<string> SerializeAsync(object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default);
}
#endif