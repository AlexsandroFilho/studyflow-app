using StudyFlow.Api.Data.Repositories;
using StudyFlow.Api.Domain.Interfaces.Auth;
using StudyFlow.Api.Domain.Interfaces.Conexao;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.Domain.Interfaces.Usuarios;
using StudyFlow.Api.Services;
using StudyFlow.Api.Validators;
using FluentValidation;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
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
        services.AddScoped<IFonteAnatomiaRepository, FonteAnatomiaRepository>();
        services.AddScoped<IAnatomiaChunkRepository, AnatomiaChunkRepository>();
        services.AddScoped<IRevisaoNotaRepository, RevisaoNotaRepository>();
        services.AddScoped<IResumoTemaRepository, ResumoTemaRepository>();
        services.AddScoped<IContextoGrafoNotasRepository, ContextoGrafoNotasRepository>();
        services.AddScoped<IContextoGrafoNotasService, ContextoGrafoNotasService>();
        services.AddScoped<IContextoTemaRepository, ContextoTemaRepository>();
        services.AddScoped<IContextoTemaService, ContextoTemaService>();
        services.AddScoped<IEmbeddingService, GeminiEmbeddingService>();
        services.AddScoped<IBuscaContextoAnatomia, BuscaContextoAnatomiaPostgres>();
        services.AddScoped<IRevisorAnatomia, GeminiRevisorAnatomia>();
        services.AddScoped<IResumidorTemaAnatomia, GeminiResumidorTemaAnatomia>();
        services.AddScoped<IRevisaoNotaService, RevisaoNotaService>();
        services.AddScoped<IResumoTemaService, ResumoTemaService>();
        services.AddScoped<IIngestaoAnatomiaService, IngestaoAnatomiaService>();
        services.AddHttpClient<IModeloIaClient, GeminiModelClient>(client =>
        {
            client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
        });
        RegistrarArmazenamentoFonte(services, configuration);
        services.AddValidatorsFromAssemblyContaining<RegistroRequestValidator>();

        return services;
    }

    private static void RegistrarArmazenamentoFonte(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(SupabaseStorageSettings.SectionName).Get<SupabaseStorageSettings>() ?? new SupabaseStorageSettings();
        if (!string.IsNullOrWhiteSpace(settings.Url) && !string.IsNullOrWhiteSpace(settings.ServiceRoleKey))
            services.AddScoped<IArmazenamentoFonteAnatomia, ArmazenamentoFonteSupabase>();
        else
            services.AddScoped<IArmazenamentoFonteAnatomia, ArmazenamentoFonteLocal>();
    }
}
