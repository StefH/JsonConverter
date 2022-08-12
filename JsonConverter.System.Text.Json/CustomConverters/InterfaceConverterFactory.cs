using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConverter.System.Text.Json.CustomConverters;

public class InterfaceConverterFactory : JsonConverterFactory
{
    public InterfaceConverterFactory(Type concrete, Type interfaceType)
    {
        ConcreteType = concrete;
        InterfaceType = interfaceType;
    }

    public Type ConcreteType { get; }

    public Type InterfaceType { get; }

    public override bool CanConvert(Type typeToConvert) => typeToConvert == InterfaceType;

    public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return (global::System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(typeof(InterfaceConverter<,>).MakeGenericType(ConcreteType, InterfaceType));
    }
}