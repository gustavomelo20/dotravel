using Application.Dtos.Users;
using Domain.Entities.Users;

namespace Application.Interfaces.Users;

public interface IListUsers
{
	Task<IList<User>> ExecuteAsync(ListUsersDto input, CancellationToken cancellationToken = default);
}
