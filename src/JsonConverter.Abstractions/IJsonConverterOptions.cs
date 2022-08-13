namespace JsonConverter.Abstractions;

public interface IJsonConverterOptions
{
    bool PropertyNameCaseInsensitive { get; set; }

    bool WriteIndented { get; set; }

    bool IgnoreNullValues { get; set; }
}
