﻿#if !(NET35 || NET40)
using JsonConverter.Abstractions;
using Stef.Validation;

namespace JsonConverter.Newtonsoft.Json;

public partial class JsonConverter
{
    public Task<T?> DeserializeAsync<T>(string text, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Deserialize<T>(text, options));
    }

    public Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Deserialize<T>(stream, options));
    }

    public Task<string> SerializeAsync(object source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Serialize(source, options));
    }

    public async Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(stream);

        return IsValidJson(await stream.ReadAsStringAsync().ConfigureAwait(false));
    }
}
#endif