namespace StudyFlow.Api.Configurations;

public sealed class AiSettings
{
    public const string SectionName = "Ai";
    public string? GeminiApiKey { get; init; }
    public string ChatModel { get; init; } = "gemini-3.5-flash-lite";
    public string EmbeddingModel { get; init; } = "gemini-embedding-001";
    public int EmbeddingDimensions { get; init; } = 1536;
    public int IntervaloMinimoEmbeddingMs { get; init; } = 700;
    public int TamanhoLoteIngestao { get; init; } = 20;
    public int ContextoQuantidadeChunks { get; init; } = 6;
}
