using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Services.Queries.GetServices;

public record ServiceDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    DateTime CreatedAt
);

public record GetServicesQuery() : IRequest<Result<List<ServiceDto>>>;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, Result<List<ServiceDto>>>
{
    private readonly IAppDbContext _context;

    public GetServicesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ServiceDto>>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _context.Services
            .AsNoTracking()
            .Include(s => s.Client)
            .Select(s => new ServiceDto(
                s.Id,
                s.ClientId,
                s.Client.Name,
                s.Name,
                s.Description,
                s.Price,
                s.Currency,
                s.CreatedAt
            ))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<List<ServiceDto>>.Success(services);
    }
}
