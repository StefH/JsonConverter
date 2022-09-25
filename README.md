# JsonConverter

Common interface + implementation for Json Converters:
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json)
- [SimpleJson](https://github.com/facebook-csharp-sdk/simple-json)
- [NetJSON](https://github.com/rpgmaker/NetJSON)
- [Utf8Json](https://github.com/neuecc/Utf8Json)
- [XUtf8Json](https://github.com/geeking/Utf8Json)
- [ServiceStack.Text](https://docs.servicestack.net/json-format)

## NuGets

| Name | Version |
| - | - |
| **JsonConverter.Abstractions** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.Abstractions)](https://www.nuget.org/packages/JsonConverter.Abstractions)
| **JsonConverter.Newtonsoft.Json** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.Newtonsoft.Json)](https://www.nuget.org/packages/JsonConverter.Newtonsoft.Json)
| **JsonConverter.System.Text.Json** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.System.Text.Json)](https://www.nuget.org/packages/JsonConverter.System.Text.Json)
| **JsonConverter.SimpleJson** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.SimpleJson)](https://www.nuget.org/packages/JsonConverter.SimpleJson)
| **JsonConverter.NetJSON** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.NetJSON)](https://www.nuget.org/packages/JsonConverter.NetJSON)
| **JsonConverter.Utf8Json** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.Utf8Json)](https://www.nuget.org/packages/JsonConverter.Utf8Json)
| **JsonConverter.XUtf8Json** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.XUtf8Json)](https://www.nuget.org/packages/JsonConverter.XUtf8Json)
| **JsonConverter.ServiceStack.Text** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.ServiceStack.Text)](https://www.nuget.org/packages/JsonConverter.ServiceStack.Text)
| **JsonConverter.Argon** | [![NuGet Badge](https://buildstats.info/nuget/JsonConverter.Argon)](https://www.nuget.org/packages/JsonConverter.Argon)


## Interfaces

### IJsonConverter

``` csharp
public interface IJsonConverter
{
    Task<T?> DeserializeAsync<T>(Stream stream, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    T? Deserialize<T>(Stream stream, IJsonConverterOptions? options = null);

    T? Deserialize<T>(string text, IJsonConverterOptions? options = null);

    Task<string> SerializeAsync(object source, IJsonConverterOptions? options = null, CancellationToken cancellationToken = default);

    string Serialize(object source, IJsonConverterOptions? options = null);

    Task SerializeAsync(Stream stream, object value, JsonConverterOptions? options = null, CancellationToken cancellationToken = default);

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