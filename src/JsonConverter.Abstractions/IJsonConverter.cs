namespace JsonConverter.Abstractions;

public partial interface IJsonConverter
{
    T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null);

    T? Deserialize<T>(string text, JsonConverterOptions? options = null);

    string Serialize(object source, JsonConverterOptions? options = null);

    bool IsValidJson(Stream stream);

    bool IsValidJson(string input);
}