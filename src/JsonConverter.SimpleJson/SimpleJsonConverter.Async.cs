#if !(NET35 || NET40)
using System.Text;
using JsonConverter.Abstractions;
using Stef.Validation;

namespace JsonConverter.SimpleJson;

public partial class SimpleJsonConverter
{
    public Task<T?> DeserializeAsync<T>(Stream stream, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Deserialize<T>(stream, options));
    }

    public Task<string> SerializeAsync(object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Serialize(value, options));
    }

    public async Task SerializeAsync(Stream stream, object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        var json = Serialize(value, options);

        cancellationToken.ThrowIfCancellationRequested();

        await stream.WriteAsync(Encoding.UTF8.GetBytes(json), cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        return IsValidJson(await stream.ReadAsStringAsync().ConfigureAwait(false));
    }
}
#endif