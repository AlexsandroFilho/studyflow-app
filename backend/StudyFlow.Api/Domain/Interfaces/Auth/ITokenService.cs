using System.Security.Claims;

namespace StudyFlow.Api.Domain.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateToken(ClaimsPrincipal principal);
        string GenerateToken(Guid userId, string email, string name);
    }
}
