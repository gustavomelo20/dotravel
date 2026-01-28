using Domain.Entities.Users;

namespace Domain.Repositories.Users;

public interface IUserWriteOnlyRepository
{
	Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
	Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}