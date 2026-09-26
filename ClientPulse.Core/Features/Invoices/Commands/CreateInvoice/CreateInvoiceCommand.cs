using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Entities;
using ClientPulse.Core.Enums;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Invoices.Commands.CreateInvoice;

public record CreateInvoiceCommand(
    Guid ClientId,
    decimal Amount,
    string Currency,
    DateTime IssueDate,
    DateTime DueDate,
    string? Notes,
    int InstallmentsCount = 1
) : IRequest<Result<Guid>>;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<Guid>>
{
    private readonly IAppDbContext _context;

    public CreateInvoiceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
        {
            return Result<Guid>.Failure("Belirtilen müşteri bulunamadı.");
        }

        int count = request.InstallmentsCount <= 0 ? 1 : request.InstallmentsCount;
        decimal installmentAmount = Math.Round(request.Amount / count, 2);
        DateTime baseDueDate = request.DueDate == default ? DateTime.UtcNow.AddDays(14) : request.DueDate;
        DateTime issueDate = request.IssueDate == default ? DateTime.UtcNow : request.IssueDate;
        Guid firstInvoiceId = Guid.Empty;

        for (int i = 1; i <= count; i++)
        {
            var dueDate = baseDueDate.AddMonths(i - 1);
            string note = count == 1 
                ? request.Notes ?? "Fatura" 
                : (string.IsNullOrWhiteSpace(request.Notes) ? $"Taksit {i}/{count}" : $"{request.Notes} (Taksit {i}/{count})");

            var invoice = new Invoice
            {
                ClientId = request.ClientId,
                Amount = installmentAmount,
                Currency = string.IsNullOrWhiteSpace(request.Currency) ? "TRY" : request.Currency,
                IssueDate = issueDate,
                DueDate = dueDate,
                Status = InvoiceStatus.Pending,
                Notes = note
            };

            _context.Invoices.Add(invoice);
            if (i == 1)
            {
                firstInvoiceId = invoice.Id;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(firstInvoiceId);
    }
}
