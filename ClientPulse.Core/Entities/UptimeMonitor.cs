using System;
using ClientPulse.Core.Common;

namespace ClientPulse.Core.Entities;

public class UptimeMonitor : BaseEntity
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string Url { get; set; } = default!;
    
    public int CheckIntervalMinutes { get; set; } = 5;
    public bool IsActive { get; set; } = true;
}
