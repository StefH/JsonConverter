using System.Globalization;
using System.Text;
using CultureAwareTesting.xUnit;
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
    public void Deserialize_DateParseHandlingNone_KeepsDateAsString()
    {
        // Arrange
        var json = """{"dateString":"2021-11-10T13:39:13.705"}""";
        var options = new JsonConverterOptions
        {
            DateParseHandling = (int)DateParseHandling.None // None - keep as string
        };

        // Act
        var result = _sut.Deserialize<JObject>(json, options);

        // Assert
        result.Should().NotBeNull();
        var dateValue = result!["dateString"];
        dateValue.Should().NotBeNull();
        dateValue!.Type.Should().Be(JTokenType.String);
        dateValue.Value<string>().Should().Be("2021-11-10T13:39:13.705");
    }

    [Fact]
    public void Deserialize_Stream_DateParseHandlingNone_KeepsDateAsString()
    {
        // Arrange
        var json = """{"dateString":"2021-11-10T13:39:13.705"}""";
        var options = new JsonConverterOptions
        {
            DateParseHandling = 0 // None - keep as string
        };
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = _sut.Deserialize<JObject>(stream, options);

        // Assert
        result.Should().NotBeNull();
        var dateValue = result!["dateString"];
        dateValue.Should().NotBeNull();
        dateValue!.Type.Should().Be(JTokenType.String);
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
        var dateValue = result!["dateString"];
        dateValue.Should().NotBeNull();
        dateValue!.Type.Should().Be(JTokenType.Date);
        ((DateTime)dateValue).Should().Be(new DateTime(2021, 11, 10, 13, 39, 13, 705));
    }

    [CulturedFact("en-US")]
    public void Deserialize_DateParseHandlingDateTimeOffset_ParsesDateAsDateTimeOffset()
    {
        // Arrange
        var offset = new DateTimeOffset(2021, 11, 10, 13, 39, 13, 705, TimeSpan.Zero);
        var json = $"{{\"dateString\":\"{offset.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz", CultureInfo.InvariantCulture)}\"}}";
        var options = new JsonConverterOptions
        {
            DateParseHandling = 2 // DateTimeOffset
        };

        // Act
        var result = _sut.Deserialize<JObject>(json, options);

        // Assert
        result.Should().NotBeNull();
        var dateValue = result!["dateString"];
        dateValue.Should().NotBeNull();
        dateValue!.Type.Should().Be(JTokenType.Date);
        ((DateTimeOffset)dateValue).Should().Be(offset);
    }

    [Theory]
    [InlineData("{\"a\":1}", JsonType.Object)]
    [InlineData("[1,2,3]", JsonType.Array)]
    [InlineData("\"value\"", JsonType.String)]
    [InlineData("123", JsonType.Number)]
    [InlineData("-1.5", JsonType.Number)]
    [InlineData("true", JsonType.True)]
    [InlineData("false", JsonType.False)]
    [InlineData("null", JsonType.Null)]
    [InlineData("", JsonType.Undefined)]
    [InlineData("   ", JsonType.Undefined)]
    [InlineData("abc", JsonType.Undefined)]
    public void GetJsonType_String_ReturnsExpectedType(string input, JsonType expected)
    {
        var result = _sut.GetJsonType(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void GetJsonType_Stream_ReturnsExpectedType()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(" [1,2,3] "));

        var result = _sut.GetJsonType(stream);

        result.Should().Be(JsonType.Array);
    }

    [Fact]
    public void ParseJsonToken_FromJToken_ReturnsTypedObject()
    {
        // Arrange
        var token = JToken.Parse("""{"name":"test","value":123}""");

        // Act
        var result = _sut.ParseJsonToken<TestModel>(token);

        // Assert
        result.Name.Should().Be("test");
        result.Value.Should().Be(123);
    }

    [Fact]
    public void ToJsonToken_FromJsonString_ReturnsJToken()
    {
        // Act
        var result = _sut.ToJsonToken("""{"name":"test","value":123}""");

        // Assert
        result.Should().BeAssignableTo<JToken>();
        var token = (JToken)result;
        token["name"]!.Value<string>().Should().Be("test");
        token["value"]!.Value<int>().Should().Be(123);
    }

    [Fact]
    public void ToJsonToken_FromObject_ReturnsJObject()
    {
        // Act
        var result = _sut.ToJsonToken(new TestModel { Name = "test", Value = 123 });

        // Assert
        result.Should().BeOfType<JObject>();
        var token = (JObject)result;
        token["Name"]!.Value<string>().Should().Be("test");
        token["Value"]!.Value<int>().Should().Be(123);
    }

    [Fact]
    public void ParseJsonToken_WhenValueIsNull_ReturnsDefault()
    {
        // Act
        var result = _sut.ParseJsonToken<TestModel>(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseJsonToken_WhenValueIsPlainObject_UsesFallbackSerialization()
    {
        // Arrange
        var source = new
        {
            Name = "fallback",
            Value = 99
        };

        // Act
        var result = _sut.ParseJsonToken<TestModel>(source);

        // Assert
        result.Name.Should().Be("fallback");
        result.Value.Should().Be(99);
    }

    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;

        public int Value { get; set; }
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
