using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class DataBseContext : DbContext
{
    public DataBseContext(DbContextOptions options) : base(options) { }

    public DbSet<User> USers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataBseContext).Assembly);
        
        modelBuilder.Entity<User>(entity =>
        {

        });
    }
}