namespace StudyFlow.Api.DTOs;

public sealed record AtualizarPreferenciaGuiaRequest(bool MostrarGuiaInicial);

public sealed record UsuarioPreferenciasResponse(bool MostrarGuiaInicial);
