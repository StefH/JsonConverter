﻿using Argon;
using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Models;
using JsonConverter.Argon.Extensions;
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

    // ReSharper disable once ReturnTypeCanBeNotNullable
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

    public object? ConvertToDynamicJsonClass(object value, DynamicJsonClassOptions? options = null)
    {
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

        return ConvertToDynamicJsonClass(result);
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