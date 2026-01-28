using Domain.Entities.Users;
using Domain.Repositories.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories.Users;

public class UserRepository : IUserReadOnlyRepository, IUserWriteOnlyRepository
{
	private readonly DataBseContext _context;

	public UserRepository(DataBseContext context)
	{
		_context = context;
	}

	public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _context.USers
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<IList<User>> ListAsync(string? search = null, int? page = null, int? pageSize = null, CancellationToken cancellationToken = default)
	{
		IQueryable<User> query = _context.USers.AsNoTracking();

		if (!string.IsNullOrWhiteSpace(search))
		{
			var term = search.Trim();
			query = query.Where(u => EF.Functions.Like(u.Name, $"%{term}%") || EF.Functions.Like(u.Email, $"%{term}%"));
		}

		if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
		{
			var skip = (page.Value - 1) * pageSize.Value;
			query = query.Skip(skip).Take(pageSize.Value);
		}

		return await query.ToListAsync(cancellationToken);
	}

	public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
	{
		await _context.USers.AddAsync(user, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return user;
	}

	public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
	{
		var existing = await _context.USers.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
		if (existing is null)
		{
			throw new KeyNotFoundException($"User with id {user.Id} not found");
		}

		existing.Name = user.Name;
		existing.Email = user.Email;
		existing.Password = user.Password;
		existing.UpdatedAt = DateTime.UtcNow;

		await _context.SaveChangesAsync(cancellationToken);
		return existing;
	}

	public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var existing = await _context.USers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
		if (existing is null)
		{
			return false;
		}

		_context.USers.Remove(existing);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}