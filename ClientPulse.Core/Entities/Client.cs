using System.Collections.Generic;
using ClientPulse.Core.Common;

namespace ClientPulse.Core.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }
    
    // Navigation properties
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<UptimeMonitor> UptimeMonitors { get; set; } = new List<UptimeMonitor>();
}
