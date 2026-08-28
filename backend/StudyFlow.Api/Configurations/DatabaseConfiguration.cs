using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;

namespace StudyFlow.Api.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não foi configurada.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        return services;
    }
}