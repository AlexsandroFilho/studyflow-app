using FluentValidation;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Validators;

public class RegistroRequestValidator : AbstractValidator<RegistroRequest>
{
    public RegistroRequestValidator()
    {
        RuleFor(request => request.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres.");

        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail deve possuir um formato válido.");

        RuleFor(request => request.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("Senha deve ter pelo menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("Senha deve conter pelo menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Senha deve conter pelo menos um caractere especial.");

        RuleFor(request => request.ConfirmacaoSenha)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória.")
            .Equal(request => request.Senha).WithMessage("A confirmação de senha deve ser igual à senha.");
    }
}