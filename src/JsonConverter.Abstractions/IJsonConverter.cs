namespace JsonConverter.Abstractions;

public partial interface IJsonConverter
{
    T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null);

    T? Deserialize<T>(string text, JsonConverterOptions? options = null);

    string Serialize(object value, JsonConverterOptions? options = null);

    bool IsValidJson(Stream stream);

    bool IsValidJson(string input);

    object? ToDynamicJsonClass(object value);
}