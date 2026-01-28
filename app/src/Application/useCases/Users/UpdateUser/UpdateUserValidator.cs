using Application.Dtos.Users;
using FluentValidation;

namespace Application.useCases.Users.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(u => u.Id)
            .NotEmpty().WithMessage("Id é obrigatório.");

        RuleFor(u => u.Name)
            .MaximumLength(200).WithMessage("Nome deve ter até 200 caracteres.")
            .When(u => !string.IsNullOrWhiteSpace(u.Name));

        RuleFor(u => u.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .When(u => !string.IsNullOrWhiteSpace(u.Email));

        RuleFor(u => u.Password)
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres.")
            .When(u => !string.IsNullOrWhiteSpace(u.Password));
    }
}
