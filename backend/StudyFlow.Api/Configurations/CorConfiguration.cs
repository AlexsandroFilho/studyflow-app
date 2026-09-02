namespace StudyFlow.Api.Configurations;

public static class CorsConfiguration
{
    private const string PolicyName = "AllowReactApp";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var originsConfiguradas = configuration.GetSection("Cors").GetValue<string>("AllowedOrigins")
            ?? "http://localhost:5173";
        var origins = originsConfiguradas.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (origins.Length == 0)
            throw new InvalidOperationException("Configure Cors:AllowedOrigins com ao menos uma URL do frontend.");

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    public static WebApplication UseCorsConfiguration(this WebApplication app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}
