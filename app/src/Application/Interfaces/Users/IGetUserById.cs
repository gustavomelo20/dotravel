using Application.Dtos.Users;
using Domain.Entities.Users;

namespace Application.Interfaces.Users;

public interface IGetUserById
{
	Task<User?> ExecuteAsync(GetUserByIdDto input, CancellationToken cancellationToken = default);
}
