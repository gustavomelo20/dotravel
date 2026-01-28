using Application.Dtos.Users;

namespace Application.Interfaces.Users;

public interface IDeleteUser
{
	Task<bool> ExecuteAsync(DeleteUserDto input, CancellationToken cancellationToken = default);
}
