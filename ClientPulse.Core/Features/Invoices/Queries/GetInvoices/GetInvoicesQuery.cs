using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Enums;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Invoices.Queries.GetInvoices;

public record InvoiceDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    decimal Amount,
    string Currency,
    DateTime IssueDate,
    DateTime DueDate,
    InvoiceStatus Status,
    string? Notes
);

public record GetInvoicesQuery() : IRequest<Result<List<InvoiceDto>>>;

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, Result<List<InvoiceDto>>>
{
    private readonly IAppDbContext _context;

    public GetInvoicesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<InvoiceDto>>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Client)
            .Select(i => new InvoiceDto(
                i.Id,
                i.ClientId,
                i.Client.Name,
                i.Amount,
                i.Currency,
                i.IssueDate,
                i.DueDate,
                i.Status,
                i.Notes
            ))
            .OrderByDescending(i => i.DueDate)
            .ToListAsync(cancellationToken);

        return Result<List<InvoiceDto>>.Success(invoices);
    }
}
