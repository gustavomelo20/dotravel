using Application.Dtos.Users;
using Application.Interfaces.Users;
using Domain.Entities.Users;
using Domain.Repositories.Users;
using FluentValidation;

namespace Application.useCases.Users.DeleteUser;

public class DeleteUserUseCase : IDeleteUser
{
    private readonly IUserWriteOnlyRepository _repositoryWrite;
    private readonly IValidator<DeleteUserDto> _validator;
    
    public DeleteUserUseCase(
        IUserWriteOnlyRepository repositoryWrite,
        IValidator<DeleteUserDto> validator
    )
    {
        _repositoryWrite = repositoryWrite;
        _validator = validator;
    }
    
    public async Task<bool> ExecuteAsync(DeleteUserDto input, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);
        // TODO: Delete via _repositoryWrite
        return true;
    }
}
