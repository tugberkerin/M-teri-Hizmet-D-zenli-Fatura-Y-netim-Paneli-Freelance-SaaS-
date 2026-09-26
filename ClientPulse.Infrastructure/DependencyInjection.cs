using ClientPulse.Core.Interfaces;
using ClientPulse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientPulse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<AppDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connStr) && !connStr.Contains("Host=localhost;Database=ClientPulseDb;Username=postgres;Password=postgres"))
            {
                options.UseNpgsql(connStr);
            }
            else
            {
                // Use local SQLite database file for 100% data persistence on disk
                options.UseSqlite("Data Source=clientpulse.db");
            }
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}
