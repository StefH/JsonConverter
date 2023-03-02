using JsonConverter.Abstractions.Models;

namespace JsonConverter.Abstractions;

public partial interface IJsonConverter
{
    T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null);

    T? Deserialize<T>(string text, JsonConverterOptions? options = null);

    string Serialize(object value, JsonConverterOptions? options = null);

    bool IsValidJson(Stream stream);

    bool IsValidJson(string input);

    /// <summary>
    /// Convert an object to a DynamicJsonClass or DynamicJsonClass-array. 
    /// </summary>.
    /// <param name="value">The object (e.g. JObject in case of Newtonsoft.Json).</param>
    /// <param name="options">The <see cref="DynamicJsonClassOptions"/> (optional).</param>
    /// <returns>object, DynamicJsonClass or DynamicJsonClass-array</returns>
    object? ConvertToDynamicJsonClass(object value, DynamicJsonClassOptions? options = null);

    /// <summary>
    /// Convert Json Text to a DynamicJsonClass or DynamicJsonClass-array. 
    /// </summary>.
    /// <param name="text">The Json Text.</param>
    /// <param name="options">The <see cref="DynamicJsonClassOptions"/> (optional).</param>
    /// <returns>object, DynamicJsonClass or DynamicJsonClass-array</returns>
    object? DeserializeToDynamicJsonClass(string text, DynamicJsonClassOptions? options = null);
}