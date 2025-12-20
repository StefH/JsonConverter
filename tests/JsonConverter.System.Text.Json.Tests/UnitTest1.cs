using FluentAssertions;
using JsonConverter.Abstractions;
using JsonConverter.System.Text.Json;

namespace JsonConverter.Newtonsoft.Json.Tests;

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
}