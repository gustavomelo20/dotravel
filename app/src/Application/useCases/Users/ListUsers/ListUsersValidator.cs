using Application.Dtos.Users;
using FluentValidation;

namespace Application.useCases.Users.ListUsers;

public class ListUsersValidator : AbstractValidator<ListUsersDto>
{
    public ListUsersValidator()
    {
        RuleFor(q => q.Page)
            .GreaterThan(0).WithMessage("Page deve ser maior que 0.")
            .When(q => q.Page.HasValue);

        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("PageSize deve ser maior que 0.")
            .LessThanOrEqualTo(200).WithMessage("PageSize máximo é 200.")
            .When(q => q.PageSize.HasValue);

        RuleFor(q => q.Search)
            .MaximumLength(200).WithMessage("Search deve ter até 200 caracteres.")
            .When(q => !string.IsNullOrWhiteSpace(q.Search));
    }
}
