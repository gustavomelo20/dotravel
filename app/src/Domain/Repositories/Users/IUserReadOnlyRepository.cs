using Domain.Entities.Users;

namespace Domain.Repositories.Users;

public interface IUserReadOnlyRepository
{
	Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IList<User>> ListAsync(string? search = null, int? page = null, int? pageSize = null, CancellationToken cancellationToken = default);
}