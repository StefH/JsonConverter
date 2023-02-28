namespace JsonConverter.Abstractions.Models;

public class DynamicJsonClassOptions
{
    public JsonConverterOptions? JsonConverterOptions { get; set; }

    public IntegerBehavior IntegerConvertBehavior { get; set; } = IntegerBehavior.UseLong;

    public FloatBehavior FloatConvertBehavior { get; set; } = FloatBehavior.UseDouble;
}