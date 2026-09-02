using Microsoft.AspNetCore.HttpOverrides;

namespace StudyFlow.Api.Configurations;

public static class ForwardedHeadersConfiguration
{
    public static IServiceCollection AddForwardedHeadersConfiguration(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services;
    }

    public static WebApplication UseForwardedHeadersConfiguration(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseForwardedHeaders();
        }

        return app;
    }
}
