using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Clients.Queries.GetClients;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, Result<List<ClientDto>>>
{
    private readonly IAppDbContext _context;

    public GetClientsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ClientDto>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _context.Clients
            .AsNoTracking()
            .Select(c => new ClientDto(
                c.Id,
                c.Name,
                c.Email,
                c.Phone,
                c.CompanyName,
                c.Services.Count,
                c.Invoices.Count,
                c.CreatedAt
            ))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<List<ClientDto>>.Success(clients);
    }
}
