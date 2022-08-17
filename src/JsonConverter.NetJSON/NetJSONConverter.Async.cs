#if !(NET35 || NET40)
using System.IO;
using System.Text;
using JsonConverter.Abstractions;
using Stef.Validation;

namespace JsonConverter.NetJSON;

public partial class NetJSONConverter
{
    public Task<T?> DeserializeAsync<T>(string text, JsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Deserialize<T>(text, options));
    }

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

#if NETSTANDARD2_1
        await stream.WriteAsync(Encoding.UTF8.GetBytes(json), cancellationToken).ConfigureAwait(false);
#else
        var bytes = Encoding.UTF8.GetBytes(json);
        await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
#endif
    }

    public async Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        return IsValidJson(await stream.ReadAsStringAsync().ConfigureAwait(false));
    }
}
#endif