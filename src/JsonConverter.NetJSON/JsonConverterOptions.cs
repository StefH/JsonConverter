using JsonConverter.Abstractions;

namespace JsonConverter.NetJSON;

public class JsonConverterOptions : IJsonConverterOptions
{
    public bool PropertyNameCaseInsensitive { get; set; }

    public bool WriteIndented { get; set; }

    public bool IgnoreNullValues { get; set; }
}