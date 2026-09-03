namespace StudyFlow.Api.DTOs
{
    public record RegistroRequest(
        string Nome,
        string Email,
        string Senha,
        string ConfirmacaoSenha);

    public record LoginRequest(
        string Email,
        string Senha);

    public record AuthResponse(
        string Token,
        Guid UserId,
        string Nome,
        string Email,
        string Role,
        bool MostrarGuiaInicial);
}
