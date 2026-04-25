using System.Collections;
using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Models;
using JsonConverter.Newtonsoft.Json.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stef.Validation;

namespace JsonConverter.Newtonsoft.Json;

public partial class NewtonsoftJsonConverter : IJsonConverter
{
    public T? Deserialize<T>(Stream stream, JsonConverterOptions? options = null)
    {
        Guard.NotNull(stream);
        Guard.Condition(stream, s => s.CanRead);

        using var streamReader = new StreamReader(stream);
        using var jsonTextReader = new JsonTextReader(streamReader);
        var jsonSerializer = new JsonSerializer();

        if (options != null)
        {
            var serializerSettings = ConvertOptions(options);
            jsonSerializer.Formatting = serializerSettings.Formatting;
            jsonSerializer.NullValueHandling = serializerSettings.NullValueHandling;
            jsonSerializer.DateParseHandling = serializerSettings.DateParseHandling;
            jsonTextReader.DateParseHandling = serializerSettings.DateParseHandling;
        }

        return jsonSerializer.Deserialize<T>(jsonTextReader);
    }

    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return options == null
            ? JsonConvert.DeserializeObject<T>(text)
            : JsonConvert.DeserializeObject<T>(text, ConvertOptions(options));
    }

    public object? Deserialize(string text, Type type, JsonConverterOptions? options = null)
    {
        return options == null
            ? JsonConvert.DeserializeObject(text, type)
            : JsonConvert.DeserializeObject(text, type, ConvertOptions(options));
    }

    public string Serialize(object value, JsonConverterOptions? options = null)
    {
        return options != null ?
            JsonConvert.SerializeObject(value, ConvertOptions(options)) :
            JsonConvert.SerializeObject(value);
    }

    public JsonType GetJsonType(Stream stream)
    {
        return JsonTypeHelper.GetJsonType(stream);
    }

    public JsonType GetJsonType(string value)
    {
        return JsonTypeHelper.GetJsonType(value);
    }

    public bool IsValidJson(Stream stream)
    {
        Guard.NotNull(stream);

        return IsValidJson(stream.ReadAsString());
    }

    public bool IsValidJson(string input)
    {
        if (input.IsNullOrWhiteSpace())
        {
            return false;
        }

        input = input.Trim();
        if ((!input.StartsWith("{") || !input.EndsWith("}")) && (!input.StartsWith("[") || !input.EndsWith("]")))
        {
            return false;
        }

        try
        {
            JToken.Parse(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public object? ConvertToDynamicJsonClass(object value, DynamicJsonClassOptions? options = null)
    {
        Guard.NotNull(value);

        if (value is JObject jObject)
        {
            return jObject.ToDynamicJsonClass(options);
        }

        if (value is JArray jArray)
        {
            return jArray.ToDynamicJsonClassArray(options);
        }

        if (value is JValue jValue)
        {
            return jValue.ToDynamicJsonClass(options);
        }

        if (value is JToken jToken)
        {
            return jToken.ToDynamicJsonClass(options);
        }

        return value;
    }

    public object? DeserializeToDynamicJsonClass(string text, DynamicJsonClassOptions? options = null)
    {
        Guard.NotNullOrEmpty(text);

        var result = options?.JsonConverterOptions == null ? 
            JsonConvert.DeserializeObject(text) : 
            JsonConvert.DeserializeObject(text, ConvertOptions(options.JsonConverterOptions));

        return result != null ? ConvertToDynamicJsonClass(result) : null;
    }

    public T ParseJsonTokenToObject<T>(object? value, JsonConverterOptions? options = null)
    {
        if (value != null && value.GetType() == typeof(T))
        {
            return (T)value;
        }

        return value switch
        {
            JToken tokenValue => tokenValue.ToObject<T>()!,

            _ => throw new NotSupportedException($"Unable to convert value to {typeof(T)}.")
        };
    }

    public object ConvertValueToJsonToken(object value, JsonConverterOptions? options = null)
    {
        // Check if JToken, string, IEnumerable or object
        return value switch
        {
            JToken tokenValue => tokenValue,
            string stringValue => Deserialize<JToken>(stringValue, options)!,
            IEnumerable enumerableValue => JArray.FromObject(enumerableValue),
            _ => JObject.FromObject(value),
        };
    }

    private static JsonSerializerSettings ConvertOptions(JsonConverterOptions options)
    {
        var dateParseHandling = options.DateParseHandling switch
        {
            0 => DateParseHandling.None,
            2 => DateParseHandling.DateTimeOffset,
            _ => DateParseHandling.DateTime
        };

        return new JsonSerializerSettings
        {
            Formatting = options.WriteIndented ? Formatting.Indented : Formatting.None,
            NullValueHandling = options.IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include,
            DateParseHandling = dateParseHandling
        };
    }
}