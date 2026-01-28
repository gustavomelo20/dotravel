using Application.Dtos.Users;
using Application.Interfaces.Users;
using Domain.Entities.Users;
using Domain.Repositories.Users;
using FluentValidation;

namespace Application.useCases.Users.GetUserById;

public class GetUserByIdUseCase : IGetUserById
{
    private readonly IUserReadOnlyRepository _repositoryRead;
    private readonly IValidator<GetUserByIdDto> _validator;
    
    public GetUserByIdUseCase(
        IUserReadOnlyRepository repositoryRead,
        IValidator<GetUserByIdDto> validator
    )
    {
        _repositoryRead = repositoryRead;
        _validator = validator;
    }
    
    public async Task<User?> ExecuteAsync(GetUserByIdDto input, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);
        // TODO: Retrieve via _repositoryRead
        return null;
    }
}
