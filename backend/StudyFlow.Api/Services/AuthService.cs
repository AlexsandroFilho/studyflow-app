using StudyFlow.Api.Domain.Interfaces.Auth;
using StudyFlow.Api.Domain.Interfaces.Usuarios;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new InvalidOperationException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new InvalidOperationException("E-mail é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Senha))
                throw new InvalidOperationException("Senha é obrigatória.");

            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(request.Email.Trim());
            if (usuarioExistente != null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            var senhaHash = _passwordHasher.Hash(request.Senha);
            var usuario = request.ToEntity(senhaHash);

            await _usuarioRepository.CriarAsync(usuario);
            await _usuarioRepository.SalvarAlteracoesAsync();

            var token = _tokenService.GenerateToken(usuario.Id, usuario.Email, usuario.Nome);

            return usuario.ToResponse(token);
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                return null;

            var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email.Trim());
            if (usuario == null)
                return null;

            var senhaValida = _passwordHasher.Verify(request.Senha, usuario.SenhaHash);
            if (!senhaValida)
                return null;

            var token = _tokenService.GenerateToken(usuario.Id, usuario.Email, usuario.Nome);

            return usuario.ToResponse(token);
        }
    }
}
