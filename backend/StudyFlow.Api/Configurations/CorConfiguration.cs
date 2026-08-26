namespace StudyFlow.Api.Configurations;

public static class CorsConfiguration
{
    private const string PolicyName = "AllowReactApp";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy.WithOrigins("http://localhost:5173", "http://192.168.222.1:5173")
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