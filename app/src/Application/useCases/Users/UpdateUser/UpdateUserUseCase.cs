using Application.Dtos.Users;
using Application.Interfaces.Users;
using Domain.Entities.Users;
using Domain.Repositories.Users;
using FluentValidation;

namespace Application.useCases.Users.UpdateUser;

public class UpdateUserUseCase : IUpdateUser
{
    private readonly IUserWriteOnlyRepository _repositoryWrite;
    private readonly IValidator<UpdateUserDto> _validator;
    
    public UpdateUserUseCase(
        IUserWriteOnlyRepository repositoryWrite,
        IValidator<UpdateUserDto> validator
    )
    {
        _repositoryWrite = repositoryWrite;
        _validator = validator;
    }
    
    public async Task<User> ExecuteAsync(UpdateUserDto input, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);
        // TODO: Apply updates via _repositoryWrite
        return new User();
    }
}
