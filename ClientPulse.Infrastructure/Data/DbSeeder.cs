using System;
using System.Linq;
using System.Threading.Tasks;
using ClientPulse.Core.Entities;
using ClientPulse.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClientPulse.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // For PostgreSQL (Supabase), tables must be created manually via SQL Editor.
        // EnsureCreatedAsync is skipped to avoid crashing if tables don't exist yet.
        if (context.Database.IsNpgsql())
        {
            try
            {
                // Check if database has been seeded
                if (context.Clients.Any())
                    return;
            }
            catch
            {
                // Tables don't exist yet — Supabase SQL schema not applied yet.
                // Run the supabase_schema.sql in Supabase SQL Editor first.
                Console.WriteLine("[DbSeeder] Supabase tablolar henuz olusturulmamis. supabase_schema.sql dosyasini Supabase SQL Editor'da calistirin.");
                return;
            }
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        // Check if database has been seeded (SQLite path)
        if (context.Clients.Any())
        {
            return;
        }

        var client1 = new Client
        {
            Name = "Ahmet Yılmaz",
            Email = "ahmet@yilmaz.com",
            Phone = "+90 532 111 2233",
            CompanyName = "Yılmaz Yazılım Ltd."
        };

        var client2 = new Client
        {
            Name = "Selin Demir",
            Email = "selin@demir.io",
            Phone = "+90 533 444 5566",
            CompanyName = "Demir Medya"
        };

        var client3 = new Client
        {
            Name = "Murat Kaya",
            Email = "murat@kaya.net",
            Phone = "+90 535 777 8899",
            CompanyName = "Kaya Danışmanlık"
        };

        var client4 = new Client
        {
            Name = "Ece Şahin",
            Email = "ece@sahin.co",
            Phone = "+90 536 999 0011",
            CompanyName = "Şahin E-Ticaret"
        };

        context.Clients.AddRange(client1, client2, client3, client4);
        await context.SaveChangesAsync();

        // Seed Services
        var services = new[]
        {
            new Service { ClientId = client1.Id, Name = "Web Geliştirme", Description = "Kurumsal web sitesi tasarımı ve kodlaması", Price = 8500, Currency = "TRY" },
            new Service { ClientId = client2.Id, Name = "SEO Danışmanlık", Description = "Aylık arama motoru optimizasyonu desteği", Price = 3200, Currency = "TRY" },
            new Service { ClientId = client3.Id, Name = "UI/UX Tasarım", Description = "Mobil uygulama arayüz ve deneyim tasarımı", Price = 6100, Currency = "TRY" },
            new Service { ClientId = client4.Id, Name = "Mobil Uygulama", Description = "iOS & Android Cross-platform uygulama geliştirme", Price = 12000, Currency = "TRY" }
        };
        context.Services.AddRange(services);

        // Seed Invoices
        var invoices = new[]
        {
            new Invoice { ClientId = client1.Id, Amount = 8500, Currency = "TRY", IssueDate = DateTime.UtcNow.AddDays(-10), DueDate = DateTime.UtcNow.AddDays(5), Status = InvoiceStatus.Pending, Notes = "Web sitesi teslimatı faturası" },
            new Invoice { ClientId = client2.Id, Amount = 3200, Currency = "TRY", IssueDate = DateTime.UtcNow.AddDays(-20), DueDate = DateTime.UtcNow.AddDays(-2), Status = InvoiceStatus.Overdue, Notes = "Eylül ayı SEO danışmanlığı" },
            new Invoice { ClientId = client3.Id, Amount = 6100, Currency = "TRY", IssueDate = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(1), Status = InvoiceStatus.Paid, Notes = "Figma tasarım teslimi" },
            new Invoice { ClientId = client4.Id, Amount = 12000, Currency = "TRY", IssueDate = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(10), Status = InvoiceStatus.Pending, Notes = "Mobil uygulama 1. taksit" }
        };
        context.Invoices.AddRange(invoices);

        // Seed Uptime Monitors
        var monitors = new[]
        {
            new UptimeMonitor { ClientId = client1.Id, Name = "Ana Website", Url = "https://ahmetyilmaz.com", CheckIntervalMinutes = 5, IsActive = true },
            new UptimeMonitor { ClientId = client2.Id, Name = "API Sunucu", Url = "https://api.selindemir.io", CheckIntervalMinutes = 3, IsActive = true },
            new UptimeMonitor { ClientId = client3.Id, Name = "Müşteri Paneli", Url = "https://panel.muratkaya.net", CheckIntervalMinutes = 5, IsActive = true },
            new UptimeMonitor { ClientId = client4.Id, Name = "E-Ticaret Mağazası", Url = "https://shop.ecesahin.co", CheckIntervalMinutes = 10, IsActive = true }
        };
        context.UptimeMonitors.AddRange(monitors);

        await context.SaveChangesAsync();
    }
}
