using Argon;
using JsonConverter.Abstractions;
using Stef.Validation;

namespace JsonConverter.Argon;

public partial class ArgonConverter : IJsonConverter
{
    // ReSharper disable once ReturnTypeCanBeNotNullable
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
        }

        return jsonSerializer.Deserialize<T>(jsonTextReader);
    }

    // ReSharper disable once ReturnTypeCanBeNotNullable
    public T? Deserialize<T>(string text, JsonConverterOptions? options = null)
    {
        return options == null
            ? JsonConvert.DeserializeObject<T>(text)
            : JsonConvert.DeserializeObject<T>(text, ConvertOptions(options));
    }

    public string Serialize(object value, JsonConverterOptions? options = null)
    {
        return options != null ?
            JsonConvert.SerializeObject(value, ConvertOptions(options)) :
            JsonConvert.SerializeObject(value);
    }

    public bool IsValidJson(Stream stream)
    {
        Guard.NotNull(stream);

        return IsValidJson(stream.ReadAsString());
    }

    public bool IsValidJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
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

    private static JsonSerializerSettings ConvertOptions(JsonConverterOptions options)
    {
        return new JsonSerializerSettings
        {
            Formatting = options.WriteIndented ? Formatting.Indented : Formatting.None,
            NullValueHandling = options.IgnoreNullValues ? NullValueHandling.Include : NullValueHandling.Ignore
        };
    }
}