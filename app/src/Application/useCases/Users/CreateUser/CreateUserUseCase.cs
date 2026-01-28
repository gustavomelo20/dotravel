using Application.Dtos.Users;
using Application.Interfaces.Users;
using Domain.Entities.Users;
using Domain.Repositories.Users;
using FluentValidation;

namespace Application.useCases.Users.CreateUser;

public class CreateUserUseCase : ICreateUser
{
    private readonly IUserWriteOnlyRepository _repositoryWrite;
    private readonly IValidator<CreateUserDto> _validator;
    
    public CreateUserUseCase(
        IUserWriteOnlyRepository repositoryWrite,
        IValidator<CreateUserDto> validator
    )
    {
        _repositoryWrite = repositoryWrite;
        _validator = validator;
    }
    
    public async Task<User> ExecuteAsync(CreateUserDto input, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);
        return new User();
    }
}