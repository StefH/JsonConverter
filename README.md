# JsonConverter

Common interface + implementation for Json Converters:
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json)
- [SimpleJson](https://github.com/facebook-csharp-sdk/simple-json)
- [NetJSON](https://github.com/rpgmaker/NetJSON)

## NuGets

| Name | Version |
| - | - |
| **JsonConverter.Abstractions** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.Abstractions)](https://www.nuget.org/packages/JsonConverter.Abstractions)
| **JsonConverter.Newtonsoft.Json** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.Newtonsoft.Json)](https://www.nuget.org/packages/JsonConverter.Newtonsoft.Json)
| **JsonConverter.System.Text.Json** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.System.Text.Json)](https://www.nuget.org/packages/JsonConverter.System.Text.Json)
| **JsonConverter.SimpleJson** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.SimpleJson)](https://www.nuget.org/packages/JsonConverter.SimpleJson)
| **JsonConverter.NetJSON** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.NetJSON)](https://www.nuget.org/packages/JsonConverter.NetJSON)



## Interfaces

### IJsonConverter

``` csharp
public interface IJsonConverter
{
    Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    Task<T?> DeserializeAsync<T>(string text, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null);

    T? Deserialize<T>(string text, IJsonConverterOptions? options = null);

    Task<string> SerializeAsync(object source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    string Serialize(object source, IJsonConverterOptions? options = null);

    Task<bool> IsValidJsonAsync(Stream stream, CancellationToken cancellationToken = default);

    Task<bool> IsValidJsonAsync(string input, CancellationToken cancellationToken = default);

    bool IsValidJson(Stream stream);

    bool IsValidJson(string input);
}
```


### JsonConverterOptions

``` csharp
public class IJsonConverterOptions
{
    public bool PropertyNameCaseInsensitive { get; set; }

    public bool WriteIndented { get; set; }

    public bool IgnoreNullValues { get; set; }
}
```