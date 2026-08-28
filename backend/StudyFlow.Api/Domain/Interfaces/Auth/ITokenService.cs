using System.Security.Claims;
using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.Domain.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateToken(ClaimsPrincipal principal);
        string GenerateToken(Guid userId, string email, string name, UserRole role);
    }
}
