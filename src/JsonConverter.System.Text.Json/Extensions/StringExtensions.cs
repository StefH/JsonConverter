using JsonConverter.Abstractions;

namespace JsonConverter.System.Text.Json.Extensions;

public static class StringExtensions
{
    public static string ToJson<T>(this T obj, IJsonConverter? converter = null, JsonConverterOptions? options = null)
    {
        return converter == null ? string.Empty : converter.Serialize(obj, options);
    }

    public static async Task<string> ToJsonAsync<T>(this T obj, IJsonConverter? converter, JsonConverterOptions? options, CancellationToken cancellationToken)
    {
        return converter == null ? string.Empty : await converter.SerializeAsync(obj, options, cancellationToken).ConfigureAwait(false);
    }
}