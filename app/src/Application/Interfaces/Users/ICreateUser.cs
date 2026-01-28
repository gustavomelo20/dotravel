using Application.Dtos.Users;
using Domain.Entities.Users;

namespace Application.Interfaces.Users;

public interface ICreateUser
{
	Task<User> ExecuteAsync(CreateUserDto input, CancellationToken cancellationToken = default);
}