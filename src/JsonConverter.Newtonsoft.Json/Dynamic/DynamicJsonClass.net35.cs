#if NET35
namespace JsonConverter.Newtonsoft.Json.Dynamic;

/// <summary>
/// Provides a base class for dynamic objects for Net 3.5
/// </summary>
internal abstract class DynamicJsonClass
{
    /// <summary>
    /// Gets the dynamic property by name.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>T</returns>
    public T? GetDynamicPropertyValue<T>(string propertyName)
    {
        var type = GetType();
        var propInfo = type.GetProperty(propertyName);

        return (T?)propInfo?.GetValue(this, null);
    }

    /// <summary>
    /// Gets the dynamic property value by name.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>value</returns>
    public object? GetDynamicPropertyValue(string propertyName)
    {
        return GetDynamicPropertyValue<object>(propertyName);
    }

    /// <summary>
    /// Sets the dynamic property value by name.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public void SetDynamicPropertyValue<T>(string propertyName, T value)
    {
        var type = GetType();
        var propInfo = type.GetProperty(propertyName);

        propInfo?.SetValue(this, value, null);
    }

    /// <summary>
    /// Sets the dynamic property value by name.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public void SetDynamicPropertyValue(string propertyName, object value)
    {
        SetDynamicPropertyValue<object>(propertyName, value);
    }
}
#endif