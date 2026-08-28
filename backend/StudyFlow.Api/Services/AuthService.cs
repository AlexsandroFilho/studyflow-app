using StudyFlow.Api.Domain.Interfaces.Auth;
using StudyFlow.Api.Domain.Interfaces.Usuarios;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;
using FluentValidation;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IValidator<RegistroRequest> _registroValidator;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IValidator<RegistroRequest> registroValidator)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _registroValidator = registroValidator;
        }

        public async Task<AuthResponse> RegisterAsync(RegistroRequest request)
        {
            await _registroValidator.ValidateAndThrowAsync(request);

            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(request.Email.Trim());
            if (usuarioExistente != null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            var senhaHash = _passwordHasher.Hash(request.Senha);
            var usuario = request.ToEntity(senhaHash);

            await _usuarioRepository.CriarAsync(usuario);
            await _usuarioRepository.SalvarAlteracoesAsync();

            return CriarRespostaAutenticada(usuario);
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

            return CriarRespostaAutenticada(usuario);
        }

        private AuthResponse CriarRespostaAutenticada(Usuario usuario)
        {
            var token = _tokenService.GenerateToken(usuario.Id, usuario.Email, usuario.Nome, usuario.Role);
            return usuario.ToResponse(token);
        }
    }
}
