using System.Text.Json;

namespace StudyFlow.Api.Configurations;

public static class JsonSerializationConfiguration
{
    public static IServiceCollection AddJsonSerializationConfiguration(
        this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });

        return services;
    }

    public static JsonNamingPolicy CamelCase => JsonNamingPolicy.CamelCase;

    public static JsonNamingPolicy SnakeCase => JsonNamingPolicy.SnakeCaseLower;
}