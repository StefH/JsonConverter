using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConverter.System.Text.Json.CustomConverters;

public class IListInterfaceConverterFactory : JsonConverterFactory
{
    public IListInterfaceConverterFactory(Type interfaceType) => InterfaceType = interfaceType;

    public Type InterfaceType { get; }

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof (IList<>).MakeGenericType(InterfaceType) && typeToConvert.GenericTypeArguments[0] == InterfaceType;

    public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return (global::System.Text.Json.Serialization.JsonConverter) Activator.CreateInstance(typeof (ListConverter<>).MakeGenericType(InterfaceType));
    }
}