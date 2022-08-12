using JsonConverter.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stef.Validation;

namespace JsonConverter.System.Text.Json.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSystemTextJsonConverterScoped(this IServiceCollection services)
    {
        Guard.NotNull(services);

        services.TryAddScoped<IJsonConverter, JsonConverter>();
        services.TryAddScoped<IJsonConverterOptions, JsonConverterOptions>();
        return services;
    }

    public static IServiceCollection AddSystemTextJsonConverter(this IServiceCollection services)
    {
        Guard.NotNull(services);

        services.TryAddSingleton<IJsonConverter, JsonConverter>();
        services.TryAddSingleton<IJsonConverterOptions, JsonConverterOptions>();
        return services;
    }
}