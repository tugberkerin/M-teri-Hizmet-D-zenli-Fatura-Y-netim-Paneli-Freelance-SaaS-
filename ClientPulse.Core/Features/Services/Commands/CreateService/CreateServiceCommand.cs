using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Entities;
using ClientPulse.Core.Enums;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Services.Commands.CreateService;

public record CreateServiceCommand(
    Guid ClientId,
    string Name,
    string? Description,
    decimal Price,
    string Currency = "TRY",
    int InstallmentsCount = 1,
    bool AutoCreateInvoice = true,
    DateTime? FirstDueDate = null
) : IRequest<Result<Guid>>;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Result<Guid>>
{
    private readonly IAppDbContext _context;

    public CreateServiceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
        {
            return Result<Guid>.Failure("Belirtilen müşteri bulunamadı.");
        }

        var service = new Service
        {
            ClientId = request.ClientId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "TRY" : request.Currency
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync(cancellationToken);

        // Auto-generate Invoices if enabled
        if (request.AutoCreateInvoice && request.Price > 0)
        {
            int count = request.InstallmentsCount <= 0 ? 1 : request.InstallmentsCount;
            decimal installmentAmount = Math.Round(request.Price / count, 2);
            DateTime baseDueDate = request.FirstDueDate ?? DateTime.UtcNow.AddDays(14);

            for (int i = 1; i <= count; i++)
            {
                var dueDate = baseDueDate.AddMonths(i - 1);
                var note = count == 1 ? $"{request.Name} - Hizmet Faturası" : $"{request.Name} (Taksit {i}/{count})";

                var invoice = new Invoice
                {
                    ClientId = request.ClientId,
                    Amount = installmentAmount,
                    Currency = service.Currency,
                    IssueDate = DateTime.UtcNow,
                    DueDate = dueDate,
                    Status = InvoiceStatus.Pending,
                    Notes = note
                };

                _context.Invoices.Add(invoice);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result<Guid>.Success(service.Id);
    }
}
