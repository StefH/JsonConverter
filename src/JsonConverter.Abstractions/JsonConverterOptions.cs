namespace JsonConverter.Abstractions;

public class JsonConverterOptions
{
    public bool PropertyNameCaseInsensitive { get; set; }

    public bool WriteIndented { get; set; }

    public bool IgnoreNullValues { get; set; }

    /// <summary>
    /// Controls how date formatted strings are parsed during deserialization.
    /// - 0 (None): Date formatted strings are not parsed to a date type and are read as strings.
    /// - 1 (DateTime): Date formatted strings, e.g. "/Date(1198908717056)/" and "2012-03-21T05:40Z", are parsed to DateTime.
    /// - 2 (DateTimeOffset): Date formatted strings, e.g. "/Date(1198908717056)/" and "2012-03-21T05:40Z", are parsed to DateTimeOffset.
    /// </summary>
    public int DateParseHandling { get; set; } = 1; // Default to DateTime
}