using Application.Dtos.Users;
using Application.Interfaces.Users;
using Domain.Entities.Users;
using Domain.Repositories.Users;
using FluentValidation;

namespace Application.useCases.Users.ListUsers;

public class ListUsersUseCase : IListUsers
{
    private readonly IUserReadOnlyRepository _repositoryRead;
    private readonly IValidator<ListUsersDto> _validator;
    
    public ListUsersUseCase(
        IUserReadOnlyRepository repositoryRead,
        IValidator<ListUsersDto> validator
    )
    {
        _repositoryRead = repositoryRead;
        _validator = validator;
    }
    
    public async Task<IList<User>> ExecuteAsync(ListUsersDto input, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);
        // TODO: Query via _repositoryRead
        return new List<User>();
    }
}
