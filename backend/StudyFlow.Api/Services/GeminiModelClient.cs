using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using StudyFlow.Api.Configurations;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Services;

public sealed class GeminiModelClient(HttpClient httpClient, IConfiguration configuration) : IModeloIaClient
{
    public async Task<float[]> GerarEmbeddingAsync(string texto, TipoTarefaEmbedding tipoTarefa, CancellationToken cancellationToken = default)
    {
        var settings = ObterConfiguracao();
        using var resposta = await EnviarEmbeddingAsync(
            $"v1beta/models/{settings.EmbeddingModel}:embedContent",
            new
            {
                model = $"models/{settings.EmbeddingModel}",
                content = new { parts = new[] { new { text = texto } } },
                taskType = tipoTarefa == TipoTarefaEmbedding.Documento ? "RETRIEVAL_DOCUMENT" : "RETRIEVAL_QUERY",
                outputDimensionality = settings.EmbeddingDimensions
            },
            cancellationToken);

        var valores = resposta.RootElement
            .GetProperty("embedding")
            .GetProperty("values")
            .EnumerateArray()
            .Select(x => x.GetSingle())
            .ToArray();

        if (valores.Length != settings.EmbeddingDimensions)
            throw new InvalidOperationException($"O Gemini retornou um embedding com {valores.Length} dimensões; eram esperadas {settings.EmbeddingDimensions}.");

        return valores;
    }

    public async Task<string> GerarJsonAsync(string instrucaoSistema, string prompt, CancellationToken cancellationToken = default)
    {
        var settings = ObterConfiguracao();
        using var resposta = await EnviarAsync(
            $"v1beta/models/{settings.ChatModel}:generateContent",
            new
            {
                systemInstruction = new { parts = new[] { new { text = instrucaoSistema } } },
                contents = new[] { new { role = "user", parts = new[] { new { text = prompt } } } },
                generationConfig = new { responseMimeType = "application/json" }
            },
            cancellationToken);

        var texto = string.Concat(resposta.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")
            .EnumerateArray()
            .Where(x => x.TryGetProperty("text", out _))
            .Select(x => x.GetProperty("text").GetString()));

        return string.IsNullOrWhiteSpace(texto)
            ? throw new InvalidOperationException("O Gemini não retornou conteúdo para a revisão.")
            : texto;
    }

    private async Task<JsonDocument> EnviarAsync(string endpoint, object corpo, CancellationToken cancellationToken)
    {
        return await EnviarComRetentativaAsync(endpoint, corpo, false, cancellationToken);
    }

    private async Task<JsonDocument> EnviarEmbeddingAsync(string endpoint, object corpo, CancellationToken cancellationToken)
    {
        var settings = ObterConfiguracao();
        await Task.Delay(settings.IntervaloMinimoEmbeddingMs, cancellationToken);
        return await EnviarComRetentativaAsync(endpoint, corpo, true, cancellationToken);
    }

    private async Task<JsonDocument> EnviarComRetentativaAsync(string endpoint, object corpo, bool reexecutarAoAtingirCota, CancellationToken cancellationToken)
    {
        const int tentativasMaximas = 5;
        for (var tentativa = 1; tentativa <= tentativasMaximas; tentativa++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = JsonContent.Create(corpo)
            };
            request.Headers.Add("x-goog-api-key", ObterChave(ObterConfiguracao()));

            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            }

            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
            if (reexecutarAoAtingirCota && response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && tentativa < tentativasMaximas)
            {
                await Task.Delay(ObterTempoEspera(erro), cancellationToken);
                continue;
            }

            throw new InvalidOperationException($"O Gemini não concluiu a solicitação ({(int)response.StatusCode}): {erro}");
        }

        throw new InvalidOperationException("O Gemini excedeu o número máximo de tentativas para gerar o embedding.");
    }

    private static TimeSpan ObterTempoEspera(string erro)
    {
        var correspondencia = Regex.Match(erro, @"retry in\s+(?<segundos>[\d.]+)s", RegexOptions.IgnoreCase);
        if (correspondencia.Success && double.TryParse(correspondencia.Groups["segundos"].Value, System.Globalization.CultureInfo.InvariantCulture, out var segundos))
            return TimeSpan.FromSeconds(Math.Ceiling(segundos) + 1);

        return TimeSpan.FromSeconds(45);
    }

    private AiSettings ObterConfiguracao() => configuration.GetSection(AiSettings.SectionName).Get<AiSettings>() ?? new AiSettings();

    private static string ObterChave(AiSettings settings) => settings.GeminiApiKey ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
        ?? throw new InvalidOperationException("Configure Ai:GeminiApiKey ou a variável GEMINI_API_KEY antes de usar recursos de IA.");
}
