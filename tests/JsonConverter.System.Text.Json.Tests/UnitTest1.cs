using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Models;

namespace JsonConverter.System.Text.Json.Tests;

public class DateTestModel
{
    [JsonPropertyName("dateString")]
    public string DateString { get; set; } = string.Empty;
}

public class UnitTest1
{
    private readonly SystemTextJsonConverter _sut;

    public UnitTest1()
    {
        _sut = new SystemTextJsonConverter();
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
    public void Deserialize_DateParseHandlingNone_KeepsDateAsString()
    {
        // Arrange
        const string expectedDateString = "2021-11-10T13:39:13.705";
        var json = """{"dateString":"2021-11-10T13:39:13.705"}""";
        var options = new JsonConverterOptions
        {
            DateParseHandling = 0 // None - keep as string
        };

        // Act
        var result = _sut.Deserialize<DateTestModel>(json, options);

        // Assert
        result.Should().NotBeNull();
        result!.DateString.Should().NotBeNull();
        result.DateString.Should().BeOfType<string>();
        result.DateString.Should().Be(expectedDateString);
    }

    [Fact]
    public void Deserialize_DateParseHandlingDateTime_KeepsDateAsStringByDefault()
    {
        // Arrange
        // Note: System.Text.Json by default doesn't parse ISO 8601 date strings to DateTime
        // unless the property type is DateTime. Since our model has a string property,
        // the date string is kept as a string regardless of DateParseHandling setting.
        const string dateString = "2021-11-10T13:39:13.705";
        var json = """{"dateString":"2021-11-10T13:39:13.705"}""";
        var options = new JsonConverterOptions
        {
            DateParseHandling = 1 // DateTime
        };

        // Act
        var result = _sut.Deserialize<DateTestModel>(json, options);

        // Assert
        result.Should().NotBeNull();
        result!.DateString.Should().NotBeNull();
        result.DateString.Should().Be(dateString);
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
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(" { \"a\": 1 } "));

        var result = _sut.GetJsonType(stream);

        result.Should().Be(JsonType.Object);
    }

    [Fact]
    public void ParseJsonToken_FromJsonElement_ReturnsTypedObject()
    {
        var element = JsonSerializer.Deserialize<JsonElement>("""{"Name":"test","Value":123}""");

        var result = _sut.ParseJsonToken<TestModel>(element);

        result.Name.Should().Be("test");
        result.Value.Should().Be(123);
    }

    [Fact]
    public void ToJsonToken_FromJsonString_ReturnsJsonElement()
    {
        var result = _sut.ToJsonToken("""{"name":"test","value":123}""");

        result.Should().BeOfType<JsonElement>();
        var element = (JsonElement)result;
        element.GetProperty("name").GetString().Should().Be("test");
        element.GetProperty("value").GetInt32().Should().Be(123);
    }

    [Fact]
    public void ToJsonToken_FromObject_ReturnsJsonElement()
    {
        var result = _sut.ToJsonToken(new TestModel { Name = "test", Value = 123 });

        result.Should().BeOfType<JsonElement>();
        var element = (JsonElement)result;
        element.GetProperty("Name").GetString().Should().Be("test");
        element.GetProperty("Value").GetInt32().Should().Be(123);
    }

    [Fact]
    public void ParseJsonToken_WhenValueIsNull_ReturnsDefault()
    {
        var result = _sut.ParseJsonToken<TestModel>(null);

        result.Should().BeNull();
    }

    [Fact]
    public void ParseJsonToken_WhenValueIsPlainObject_UsesFallbackSerialization()
    {
        var source = new
        {
            Name = "fallback",
            Value = 99
        };

        var result = _sut.ParseJsonToken<TestModel>(source);

        result.Name.Should().Be("fallback");
        result.Value.Should().Be(99);
    }

    [Fact]
    public void Serialize_UserProvidedSettings_Success()
    {
        // Arrange
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        var sut = new SystemTextJsonConverter(jsonSerializerOptions);

        var classWithEnum = new ClassWithEnum
        {
            Type = (ClassWithEnumType)Random.Shared.Next(3)
        };

        // Act
        var result = sut.Serialize(classWithEnum, options: null);

        // Assert
        result.Should().Contain(classWithEnum.Type.ToString().ToLower());
    }

    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;

        public int Value { get; set; }
    }

    private sealed class ClassWithEnum
    {
        public ClassWithEnumType Type { get; set; }
    }

    private enum ClassWithEnumType
    {
        Default = 0,
        First = 1,
        Second = 2
    }
}