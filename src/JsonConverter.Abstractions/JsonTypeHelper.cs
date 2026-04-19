using JsonConverter.Abstractions.Models;

namespace JsonConverter.Abstractions;

public static class JsonTypeHelper
{
    public static JsonType GetJsonType(string value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return JsonType.Undefined;
        }

        var trimmed = value.Trim();
        var firstChar = trimmed[0];

        return firstChar switch
        {
            '{' => JsonType.Object,
            '[' => JsonType.Array,
            '"' => JsonType.String,
            't' when string.Equals(trimmed, "true", StringComparison.Ordinal) => JsonType.True,
            'f' when string.Equals(trimmed, "false", StringComparison.Ordinal) => JsonType.False,
            'n' when string.Equals(trimmed, "null", StringComparison.Ordinal) => JsonType.Null,
            '-' or >= '0' and <= '9' => JsonType.Number,
            _ => JsonType.Undefined
        };
    }
}