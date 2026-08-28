using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers
{
    public static class AuthMapper
    {
        public static Usuario ToEntity(this RegistroRequest request, string senhaHash)
        {
            return new Usuario
            {
                Nome = request.Nome.Trim(),
                Email = request.Email.Trim(),
                SenhaHash = senhaHash,
                Role = UserRole.User
            };
        }

        public static AuthResponse ToResponse(this Usuario usuario, string token)
        {
            return new AuthResponse(
                Token: token,
                UserId: usuario.Id,
                Nome: usuario.Nome,
                Email: usuario.Email,
                Role: usuario.Role.ToString());
        }
    }
}