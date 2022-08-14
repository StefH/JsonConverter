namespace JsonConverter.Abstractions;

public partial interface IJsonConverter
{
    T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null);

    T? Deserialize<T>(string text, IJsonConverterOptions? options = null);

    string Serialize<T>(T source, IJsonConverterOptions? options = null);


    bool IsValidJson(Stream stream);

    bool IsValidJson(string input);
}