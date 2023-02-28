﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonConverter.Abstractions.Models;
using JsonConverter.Newtonsoft.Json.Dynamic;
#if NEWTON
using Newtonsoft.Json.Linq;

namespace JsonConverter.Newtonsoft.Json.Extensions;
#else
using Argon;

namespace JsonConverter.Argon.Extensions;
#endif

/// <summary>
/// Based on https://github.com/fluffynuts/PeanutButter/blob/master/source/Utils/PeanutButter.JObjectExtensions/JObjectExtensions.cs
/// </summary>
internal static class JObjectExtensions
{
    private static readonly JTokenResolvers Resolvers = new()
    {
        { JTokenType.Array, ConvertJTokenArray },
        { JTokenType.Boolean, (jToken, _) => jToken.Value<bool>() },
        { JTokenType.Bytes, (jToken, _) => jToken.Value<byte[]>() },
        { JTokenType.Date, (jToken, _) => jToken.Value<DateTime>() },
        { JTokenType.Float, ConvertJTokenToDoubleOrFloat },
        { JTokenType.Guid, (jToken, _) => jToken.Value<Guid>() },
        { JTokenType.Integer, ConvertJTokenToLongOrInt },
        { JTokenType.None, (_, _) => null },
        { JTokenType.Null, (_, _) => null },
        { JTokenType.Object, TryConvertObject },
        { JTokenType.Property, ConvertJTokenProperty },
        { JTokenType.String, (jToken, _) => jToken.Value<string>() },
        { JTokenType.TimeSpan, (jToken, _) => jToken.Value<TimeSpan>() },
        { JTokenType.Undefined, (_, _) => null },
        { JTokenType.Uri, (o, _) => o.Value<Uri>() },
    };

    public static object? ToDynamicJsonClass(this JValue src)
    {
        return src.Value;
    }

    public static DynamicJsonClass? ToDynamicJsonClass(this JObject? src, DynamicJsonClassOptions? options = null)
    {
        if (src == null)
        {
            return null;
        }

        var dynamicPropertyWithValues = new List<DynamicPropertyWithValue>();

        foreach (var prop in src.Properties())
        {
            var value = Resolvers[prop.Type](prop.Value, options);
            if (value != null)
            {
                dynamicPropertyWithValues.Add(new DynamicPropertyWithValue(prop.Name, value));
            }
        }

        return DynamicJsonClassFactory.CreateInstance(dynamicPropertyWithValues);
    }

    public static IEnumerable ToDynamicJsonClassArray(this JArray? src, DynamicJsonClassOptions? options = null)
    {
        if (src == null)
        {
            return new object?[0];
        }

        return ConvertJTokenArray(src);
    }

    public static object? ToDynamicJsonClass(this JToken? src, DynamicJsonClassOptions? options = null)
    {
        if (src == null)
        {
            return null;
        }

        return GetResolverFor(src)(src, options);
    }

    private static object? TryConvertObject(JToken arg, DynamicJsonClassOptions? options = null)
    {
        if (arg is JObject asJObject)
        {
            return asJObject.ToDynamicJsonClass(options);
        }

        return GetResolverFor(arg)(arg, options);
    }

    private static object PassThrough(JToken arg, DynamicJsonClassOptions? options)
    {
        return arg;
    }

    private static Func<JToken, DynamicJsonClassOptions?, object?> GetResolverFor(JToken arg)
    {
        return Resolvers.TryGetValue(arg.Type, out var result) ? result : PassThrough;
    }

    private static object ConvertJTokenToDoubleOrFloat(JToken arg, DynamicJsonClassOptions? options = null)
    {
        if (arg.Type != JTokenType.Float)
        {
            throw new InvalidOperationException($"Unable to convert {nameof(JToken)} of type: {arg.Type} to double or float.");
        }

        if (options?.FloatConvertBehavior == FloatBehavior.UseFloat)
        {
            try
            {
                return arg.Value<float>();
            }
            catch
            {
                return arg.Value<double>();
            }
        }

        if (options?.FloatConvertBehavior == FloatBehavior.UseDecimal)
        {
            try
            {
                return arg.Value<decimal>();
            }
            catch
            {
                return arg.Value<double>();
            }
        }
        

        return arg.Value<double>();
    }

    private static object ConvertJTokenToLongOrInt(JToken arg, DynamicJsonClassOptions? options = null)
    {
        if (arg.Type != JTokenType.Integer)
        {
            throw new InvalidOperationException($"Unable to convert {nameof(JToken)} of type: {arg.Type} to long or int.");
        }

        var longValue = arg.Value<long>();

        if (options is null || options.IntegerConvertBehavior == IntegerBehavior.UseInt)
        {
            if (longValue is >= int.MinValue and <= int.MaxValue)
            {
                return Convert.ToInt32(longValue);
            }
        }

        return longValue;
    }

    private static object? ConvertJTokenProperty(JToken arg, DynamicJsonClassOptions? options = null)
    {
        var resolver = GetResolverFor(arg);
        if (resolver is null)
        {
            throw new InvalidOperationException($"Unable to handle {nameof(JToken)} of type: {arg.Type}.");
        }

        return resolver(arg, options);
    }

    private static IEnumerable ConvertJTokenArray(JToken arg, DynamicJsonClassOptions? options = null)
    {
        if (arg is not JArray array)
        {
            throw new InvalidOperationException($"Unable to convert {nameof(JToken)} of type: {arg.Type} to {nameof(JArray)}.");
        }

        var result = new List<object?>();
        foreach (var item in array)
        {
            result.Add(TryConvertObject(item));
        }

        var distinctType = FindSameTypeOf(result);
        return distinctType == null ? result.ToArray() : ConvertToTypedArray(result, distinctType);
    }

    private static Type? FindSameTypeOf(IEnumerable<object?> src)
    {
        var types = src.Select(o => o?.GetType()).Distinct().OfType<Type>().ToArray();
        return types.Length == 1 ? types[0] : null;
    }

    private static IEnumerable ConvertToTypedArray(IEnumerable<object?> src, Type newType)
    {
        var method = ConvertToTypedArrayGenericMethod.MakeGenericMethod(newType);
        return (IEnumerable)method.Invoke(null, new object[] { src })!;
    }

    private static readonly MethodInfo ConvertToTypedArrayGenericMethod = typeof(JObjectExtensions).GetMethod(nameof(ConvertToTypedArrayGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static T[] ConvertToTypedArrayGeneric<T>(IEnumerable<object> src)
    {
        return src.Cast<T>().ToArray();
    }

    private class JTokenResolvers : Dictionary<JTokenType, Func<JToken, DynamicJsonClassOptions?, object?>>
    {
    }
}