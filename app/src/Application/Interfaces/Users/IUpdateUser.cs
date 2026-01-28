using Application.Dtos.Users;
using Domain.Entities.Users;

namespace Application.Interfaces.Users;

public interface IUpdateUser
{
	Task<User> ExecuteAsync(UpdateUserDto input, CancellationToken cancellationToken = default);
}
