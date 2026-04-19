namespace JsonConverter.Abstractions.Models;

public enum JsonType : byte
{
    /// <summary>
    ///   Indicates that there is no value (as distinct from <see cref="Null"/>).
    /// </summary>
    Undefined = 0,

    /// <summary>
    ///   Indicates that a value is a JSON object.
    /// </summary>
    Object = 1,

    /// <summary>
    ///   Indicates that a value is a JSON array.
    /// </summary>
    Array = 2,

    /// <summary>
    ///   Indicates that a value is a JSON string.
    /// </summary>
    String = 3,

    /// <summary>
    ///   Indicates that a value is a JSON number.
    /// </summary>
    Number = 4,

    /// <summary>
    ///   Indicates that a value is the JSON value <c>true</c>.
    /// </summary>
    True = 5,

    /// <summary>
    ///   Indicates that a value is the JSON value <c>false</c>.
    /// </summary>
    False = 6,

    /// <summary>
    ///   Indicates that a value is the JSON value <c>null</c>.
    /// </summary>
    Null = 7
}