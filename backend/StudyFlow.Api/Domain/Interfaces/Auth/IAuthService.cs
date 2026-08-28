using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegistroRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
    }
}
