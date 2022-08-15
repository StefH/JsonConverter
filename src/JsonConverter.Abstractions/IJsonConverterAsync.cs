#if !(NET35 || NET40)
namespace JsonConverter.Abstractions;

public partial interface IJsonConverter
{
    Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task<T?> DeserializeAsync<T>(string text, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task<string> SerializeAsync(object source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default);
}
#endif