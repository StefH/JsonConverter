using FluentAssertions;
using JsonConverter.Abstractions;
using JsonConverter.System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConverter.System.Text.Json.Tests;

public class DateTestModel
{
    [JsonPropertyName("dateString")]
    public string DateString { get; set; }
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
}