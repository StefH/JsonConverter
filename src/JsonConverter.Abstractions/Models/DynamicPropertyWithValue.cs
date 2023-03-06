namespace JsonConverter.Newtonsoft.Json.Dynamic;

public struct DynamicPropertyWithValue
{
    public string Name { get; } = null!;

    public object? Value { get; }

    public Type Type { get; } = null!;

    public DynamicPropertyWithValue(string name, object? value)
    {
        Name = name;
        Value = value;
        Type = value?.GetType() ?? typeof(object);
    }
}