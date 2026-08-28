namespace StudyFlow.Api.Domain.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
