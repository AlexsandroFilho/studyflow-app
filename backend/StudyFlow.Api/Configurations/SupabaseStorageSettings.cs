namespace StudyFlow.Api.Configurations;

public sealed class SupabaseStorageSettings
{
    public const string SectionName = "SupabaseStorage";
    public string? Url { get; init; }
    public string? ServiceRoleKey { get; init; }
    public string Bucket { get; init; } = "fontes-anatomia";
}
