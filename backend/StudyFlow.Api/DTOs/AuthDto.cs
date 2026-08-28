namespace StudyFlow.Api.DTOs
{
    public record RegisterRequest(
        string Nome,
        string Email,
        string Senha);

    public record LoginRequest(
        string Email,
        string Senha);

    public record AuthResponse(
        string Token,
        Guid UserId,
        string Nome,
        string Email);
}
