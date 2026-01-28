using System.Reflection;
using Domain.Repositories.Users;
using Infrastructure.DataAccess.Repositories.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static  class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddRepositories(services);
    }
    
    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        // Configure your DbContext here once a provider/connection string is defined.
        // Example for SQL Server (requires Microsoft.EntityFrameworkCore.SqlServer package):
        // var connectionString = configuration.GetConnectionString("DefaultConnection");
        // services.AddDbContext<DataBseContext>(options => options.UseSqlServer(connectionString));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserReadOnlyRepository, UserRepository>();
        services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
    }
}