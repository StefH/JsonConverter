using JsonConverter.Abstractions;

namespace JsonConverter.System.Text.Json.Extensions;

public static class StringExtensions
{
    public static async Task<string> ToJsonAsync<T>(this T obj, IJsonConverter? converter, IJsonConverterOptions? options, CancellationToken cancellationToken)
    {
        return converter == null ? string.Empty : await converter.SerializeAsync(obj, options, cancellationToken).ConfigureAwait(false);
    }

    public static string ToJson<T>(this T obj, IJsonConverter? converter = null, IJsonConverterOptions? options = null)
    {
        return converter == null ? string.Empty : converter.Serialize(obj, options);
    }
}