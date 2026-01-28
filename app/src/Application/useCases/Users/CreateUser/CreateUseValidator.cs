using Application.Dtos.Users;
using FluentValidation;

namespace Application.useCases.Users.CreateUser;

public class CreateUserValidator: AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter até 200 caracteres.");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres.");
    }
}