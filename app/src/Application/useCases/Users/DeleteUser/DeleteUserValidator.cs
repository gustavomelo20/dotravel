using Application.Dtos.Users;
using FluentValidation;

namespace Application.useCases.Users.DeleteUser;

public class DeleteUserValidator : AbstractValidator<DeleteUserDto>
{
    public DeleteUserValidator()
    {
        RuleFor(u => u.Id)
            .NotEmpty().WithMessage("Id é obrigatório.");
    }
}
