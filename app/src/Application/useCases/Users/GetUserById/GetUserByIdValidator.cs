using Application.Dtos.Users;
using FluentValidation;

namespace Application.useCases.Users.GetUserById;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdDto>
{
    public GetUserByIdValidator()
    {
        RuleFor(u => u.Id)
            .NotEmpty().WithMessage("Id é obrigatório.");
    }
}
