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

    public async Task<string> ArmazenarTemporarioAsync(Stream arquivo, string nomeArquivo, Guid ingestaoId, CancellationToken cancellationToken = default)
    {
        var diretorio = Path.Combine(environment.ContentRootPath, "App_Data", "ingestoes-fontes");
        Directory.CreateDirectory(diretorio);
        var chave = Path.Combine(diretorio, $"{ingestaoId:N}.pdf");
        await using var destino = File.Create(chave);
        await arquivo.CopyToAsync(destino, cancellationToken);
        return chave;
    }

    public Task<string> BaixarParaArquivoTemporarioAsync(string chave, CancellationToken cancellationToken = default) => Task.FromResult(chave);

    public Task RemoverAsync(string chave, CancellationToken cancellationToken = default)
    {
        if (File.Exists(chave)) File.Delete(chave);
        return Task.CompletedTask;
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

    public async Task<string> ArmazenarTemporarioAsync(Stream arquivo, string nomeArquivo, Guid ingestaoId, CancellationToken cancellationToken = default)
    {
        var chave = $"ingestoes/{ingestaoId:N}.pdf";
        await EnviarAsync(chave, arquivo, cancellationToken);
        return chave;
    }

    public async Task<string> BaixarParaArquivoTemporarioAsync(string chave, CancellationToken cancellationToken = default)
    {
        var settings = ObterSettings();
        using var request = CriarRequest(HttpMethod.Get, $"{settings.Url!.TrimEnd('/')}/storage/v1/object/{settings.Bucket}/{chave}");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var diretorio = Path.Combine(Path.GetTempPath(), "studyflow", "ingestoes");
        Directory.CreateDirectory(diretorio);
        var caminho = Path.Combine(diretorio, $"{Guid.NewGuid():N}.pdf");
        await using var destino = File.Create(caminho);
        await response.Content.CopyToAsync(destino, cancellationToken);
        return caminho;
    }

    public async Task RemoverAsync(string chave, CancellationToken cancellationToken = default)
    {
        var settings = ObterSettings();
        using var request = CriarRequest(HttpMethod.Delete, $"{settings.Url!.TrimEnd('/')}/storage/v1/object/{settings.Bucket}/{chave}");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task EnviarAsync(string chave, Stream arquivo, CancellationToken cancellationToken)
    {
        var settings = ObterSettings();
        using var request = CriarRequest(HttpMethod.Post, $"{settings.Url!.TrimEnd('/')}/storage/v1/object/{settings.Bucket}/{chave}");
        request.Headers.Add("x-upsert", "true");
        request.Content = new StreamContent(arquivo);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private HttpRequestMessage CriarRequest(HttpMethod method, string url)
    {
        var settings = ObterSettings();
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ServiceRoleKey);
        request.Headers.Add("apikey", settings.ServiceRoleKey);
        return request;
    }

    private SupabaseStorageSettings ObterSettings()
    {
        var settings = configuration.GetSection(SupabaseStorageSettings.SectionName).Get<SupabaseStorageSettings>()!;
        if (string.IsNullOrWhiteSpace(settings.Url) || string.IsNullOrWhiteSpace(settings.ServiceRoleKey))
            throw new InvalidOperationException("Supabase Storage não está configurado.");
        return settings;
    }
}
