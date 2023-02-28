using FluentAssertions;
using JsonConverter.Abstractions.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonConverter.Newtonsoft.Json.Tests;

public class UnitTest1
{
    private readonly NewtonsoftJsonConverter _sut;

    public UnitTest1()
    {
        _sut = new NewtonsoftJsonConverter();
    }

    [Fact]
    public void ConvertToDynamicJsonClass()
    {
        // Arrange
        var jObject = GetJObject();
        var options = new DynamicJsonClassOptions
        {
            IntegerConvertBehavior = IntegerBehavior.UseInt
        };

        // Act
        var instance = _sut.ConvertToDynamicJsonClass(jObject, options);

        // Assert
        instance.Should().BeAssignableTo<DynamicJsonClass>();
    }

    [Fact]
    public void DeserializeToDynamicJsonClass()
    {
        // Arrange
        var jObject = GetJObject();
        var json = jObject.ToString(Formatting.Indented);
        var options = new DynamicJsonClassOptions
        {
            IntegerConvertBehavior = IntegerBehavior.UseInt,
            FloatConvertBehavior = FloatBehavior.UseFloat
        };

        // Act
        var instance = _sut.DeserializeToDynamicJsonClass(json, options);

        // Assert
        instance.Should().BeAssignableTo<DynamicJsonClass>();
    }

    private static JObject GetJObject()
    {
        var jObject = new JObject
        {
            { "U", new JValue(new Uri("http://localhost:80/abc?a=5")) },
            { "N", new JValue((object?)null) },
            { "G", new JValue(Guid.NewGuid()) },
            { "Flt", new JValue(10.0f) },
            { "Dbl", new JValue(float.MaxValue * 2.0) },
            { "Check", new JValue(true) },
            {
                "Child", new JObject
                {
                    { "ChildId", new JValue(4) },
                    { "ChildDateTime", new JValue(new DateTime(2018, 2, 17)) },
                    { "TS", new JValue(TimeSpan.FromMilliseconds(999)) }
                }
            },
            { "I", new JValue(9) },
            { "L", new JValue(long.MaxValue) },
            { "Name", new JValue("Test") },
            { "Array", new JArray("stef", "test", "other") }
        };
        return jObject;
    }
}