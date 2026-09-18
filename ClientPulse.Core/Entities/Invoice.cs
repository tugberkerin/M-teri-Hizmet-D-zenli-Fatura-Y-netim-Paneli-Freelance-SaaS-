using System;
using ClientPulse.Core.Common;
using ClientPulse.Core.Enums;

namespace ClientPulse.Core.Entities;

public class Invoice : BaseEntity
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = default!;
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public string? Notes { get; set; }
}
