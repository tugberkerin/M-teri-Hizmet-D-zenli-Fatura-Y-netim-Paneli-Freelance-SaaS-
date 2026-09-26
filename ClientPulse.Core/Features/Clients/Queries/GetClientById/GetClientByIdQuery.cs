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

namespace ClientPulse.Core.Features.Clients.Queries.GetClientById;

public record ClientServiceSubDto(Guid Id, string Name, string? Description, decimal Price, string Currency);
public record ClientInvoiceSubDto(Guid Id, decimal Amount, string Currency, DateTime IssueDate, DateTime DueDate, InvoiceStatus Status, string? Notes);
public record ClientUptimeSubDto(Guid Id, string Name, string Url, int CheckIntervalMinutes, bool IsActive);

public record ClientDetailDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? CompanyName,
    DateTime CreatedAt,
    List<ClientServiceSubDto> Services,
    List<ClientInvoiceSubDto> Invoices,
    List<ClientUptimeSubDto> UptimeMonitors
);

public record GetClientByIdQuery(Guid Id) : IRequest<Result<ClientDetailDto>>;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, Result<ClientDetailDto>>
{
    private readonly IAppDbContext _context;

    public GetClientByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ClientDetailDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .AsNoTracking()
            .Include(c => c.Services)
            .Include(c => c.Invoices)
            .Include(c => c.UptimeMonitors)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (client == null)
        {
            return Result<ClientDetailDto>.Failure("Müşteri bulunamadı.");
        }

        var dto = new ClientDetailDto(
            client.Id,
            client.Name,
            client.Email,
            client.Phone,
            client.CompanyName,
            client.CreatedAt,
            client.Services.Select(s => new ClientServiceSubDto(s.Id, s.Name, s.Description, s.Price, s.Currency)).ToList(),
            client.Invoices.Select(i => new ClientInvoiceSubDto(i.Id, i.Amount, i.Currency, i.IssueDate, i.DueDate, i.Status, i.Notes)).ToList(),
            client.UptimeMonitors.Select(u => new ClientUptimeSubDto(u.Id, u.Name, u.Url, u.CheckIntervalMinutes, u.IsActive)).ToList()
        );

        return Result<ClientDetailDto>.Success(dto);
    }
}
