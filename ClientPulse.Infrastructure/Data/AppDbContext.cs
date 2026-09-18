using ClientPulse.Core.Entities;
using ClientPulse.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<UptimeMonitor> UptimeMonitors => Set<UptimeMonitor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Optimistic Locking using PostgreSQL xmin (mapped to Version property)
        modelBuilder.Entity<Client>().Property(x => x.Version).IsRowVersion();
        modelBuilder.Entity<Service>().Property(x => x.Version).IsRowVersion();
        modelBuilder.Entity<Invoice>().Property(x => x.Version).IsRowVersion();
        modelBuilder.Entity<UptimeMonitor>().Property(x => x.Version).IsRowVersion();
        
        // Optional constraints and relations config
        modelBuilder.Entity<Client>()
            .HasIndex(c => c.Email)
            .IsUnique();
            
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Services)
            .WithOne(s => s.Client)
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Invoices)
            .WithOne(i => i.Client)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Client>()
            .HasMany(c => c.UptimeMonitors)
            .WithOne(u => u.Client)
            .HasForeignKey(u => u.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
