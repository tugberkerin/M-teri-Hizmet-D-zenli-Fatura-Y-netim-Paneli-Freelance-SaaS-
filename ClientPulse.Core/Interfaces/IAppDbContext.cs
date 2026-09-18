using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Interfaces;

public interface IAppDbContext
{
    DbSet<Client> Clients { get; }
    DbSet<Service> Services { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<UptimeMonitor> UptimeMonitors { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
