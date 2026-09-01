using System.Net.Http.Headers;
using StudyFlow.Api.Configurations;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Services;

public sealed class ArmazenamentoFonteLocal(IHostEnvironment environment) : IArmazenamentoFonteAnatomia
{
    public async Task<string> ArmazenarAsync(string caminhoArquivo, string hashConteudo, CancellationToken cancellationToken = default)
    {
        var diretorio = Path.Combine(environment.ContentRootPath, "App_Data", "fontes-anatomia");
        Directory.CreateDirectory(diretorio);
        var destino = Path.Combine(diretorio, $"{hashConteudo}.pdf");
        if (!File.Exists(destino))
            await using (var origem = File.OpenRead(caminhoArquivo))
            await using (var destinoStream = File.Create(destino))
                await origem.CopyToAsync(destinoStream, cancellationToken);
        return destino;
    }
}

public sealed class ArmazenamentoFonteSupabase(HttpClient httpClient, IConfiguration configuration) : IArmazenamentoFonteAnatomia
{
    public async Task<string> ArmazenarAsync(string caminhoArquivo, string hashConteudo, CancellationToken cancellationToken = default)
    {
        var settings = configuration.GetSection(SupabaseStorageSettings.SectionName).Get<SupabaseStorageSettings>()!;
        if (string.IsNullOrWhiteSpace(settings.Url) || string.IsNullOrWhiteSpace(settings.ServiceRoleKey))
            throw new InvalidOperationException("Supabase Storage não está configurado.");

        var chave = $"fontes/{hashConteudo}.pdf";
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{settings.Url.TrimEnd('/')}/storage/v1/object/{settings.Bucket}/{chave}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ServiceRoleKey);
        request.Headers.Add("apikey", settings.ServiceRoleKey);
        request.Headers.Add("x-upsert", "true");
        request.Content = new StreamContent(File.OpenRead(caminhoArquivo));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return chave;
    }
}
