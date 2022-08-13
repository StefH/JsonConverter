using System.Text.Json;

namespace JsonConverter.System.Text.Json.Extensions;

public static class JsonDocumentExtensions
{
    public static T? ToObject<T>(this JsonDocument document, JsonSerializerOptions options)
    {
        var rawText = document.RootElement.GetRawText();
        return JsonSerializer.Deserialize<T>(rawText, options);
    }
}