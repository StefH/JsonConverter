using System.Text.Json;

namespace JsonConverter.System.Text.Json.Extensions;

public static class JsonElementExtensions
{
    public static T? ToObject<T>(this JsonElement element, JsonSerializerOptions options)
    {
        var rawText = element.GetRawText();
        return JsonSerializer.Deserialize<T>(rawText, options);
    }
}