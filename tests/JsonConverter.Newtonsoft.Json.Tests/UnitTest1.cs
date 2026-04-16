using FluentAssertions;
using JsonConverter.Abstractions;
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
    public void Serialize_IgnoreNullValuesFalse()
    {
        // Arrange
        var options = new JsonConverterOptions
        {
            IgnoreNullValues = false,
            WriteIndented = false
        };
        var value = new
        {
            NullableString = (string?)null,
            NullableBoolean = (bool?)null
        };

        // Act
        var json = _sut.Serialize(value, options);

        // Assert
        json.Should().Be("""{"NullableString":null,"NullableBoolean":null}""");
    }

    [Fact]
    public void Serialize_IgnoreNullValuesTrue()
    {
        // Arrange
        var options = new JsonConverterOptions
        {
            IgnoreNullValues = true,
            WriteIndented = false
        };
        var value = new
        {
            NullableString = (string?)null,
            NullableBoolean = (bool?)null
        };

        // Act
        var json = _sut.Serialize(value, options);

        // Assert
        json.Should().Be("{}");
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

    [Fact]
    public void Deserialize_DateParseHandlingString_KeepsDateAsString()
    {
        // Arrange
        var json = """{"dateString":"2021-11-10T13:39:13.705"}""";
        var options = new JsonConverterOptions
        {
            DateParseHandling = 0 // None - keep as string
        };

        // Act
        var result = _sut.Deserialize<JObject>(json, options);

        // Assert
        result.Should().NotBeNull();
        var dateValue = result["dateString"];
        dateValue.Should().NotBeNull();
        dateValue.Type.Should().Be(JTokenType.String);
        dateValue.Value<string>().Should().Be("2021-11-10T13:39:13.705");
    }

    [Fact]
    public void Deserialize_DateParseHandlingDateTime_ParsesDateAsDateTime()
    {
        // Arrange
        var json = """{"dateString":"2021-11-10T13:39:13.705"}""";
        var options = new JsonConverterOptions
        {
            DateParseHandling = 1 // DateTime
        };

        // Act
        var result = _sut.Deserialize<JObject>(json, options);

        // Assert
        result.Should().NotBeNull();
        var dateValue = result["dateString"];
        dateValue.Should().NotBeNull();
        dateValue.Type.Should().Be(JTokenType.Date);
        ((DateTime)dateValue).Should().Be(new DateTime(2021, 11, 10, 13, 39, 13, 705));
    }

    private static JObject GetJObject()
    {
        return new JObject
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
    }
}