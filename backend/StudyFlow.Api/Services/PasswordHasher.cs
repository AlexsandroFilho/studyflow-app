using BCrypt.Net;
using StudyFlow.Api.Domain.Interfaces.Auth;

namespace StudyFlow.Api.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Senha não pode ser vazia.", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(passwordHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
