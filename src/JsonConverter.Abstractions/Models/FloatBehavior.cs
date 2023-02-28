namespace JsonConverter.Abstractions.Models;

/// <summary>
/// Enum to define how to convert an Float in the Json Object.
/// </summary>
public enum FloatBehavior
{
    /// <summary>
    /// Convert all Float types in the Json Object to a double. (default)
    /// </summary>
    UseDouble = 0,

    /// <summary>
    /// Convert all Float types in the Json Object to a float (unless overflow).
    /// </summary>
    UseFloat = 1,

    /// <summary>
    /// Convert all Float types in the Json Object to a decimal (unless overflow).
    /// </summary>
    UseDecimal = 2
}