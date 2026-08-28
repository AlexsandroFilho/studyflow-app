using StudyFlow.Api.Data.Repositories;
using StudyFlow.Api.Domain.Interfaces.Auth;
using StudyFlow.Api.Domain.Interfaces.Conexao;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.Domain.Interfaces.Usuarios;
using StudyFlow.Api.Services;

namespace StudyFlow.Api.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITemaRepository, TemaRepository>();
        services.AddScoped<ITemaService, TemaService>();
        services.AddScoped<INotaRepository, NotaRepository>();
        services.AddScoped<INotaService, NotaService>();
        services.AddScoped<IConexaoNotaRepository, ConexaoNotaRepository>();
        services.AddScoped<IConexaoNotaService, ConexaoNotaService>();

        return services;
    }
}