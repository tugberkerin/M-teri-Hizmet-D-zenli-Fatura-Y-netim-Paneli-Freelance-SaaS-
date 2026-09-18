using System;
using ClientPulse.Core.Common;

namespace ClientPulse.Core.Entities;

public class Service : BaseEntity
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
}
