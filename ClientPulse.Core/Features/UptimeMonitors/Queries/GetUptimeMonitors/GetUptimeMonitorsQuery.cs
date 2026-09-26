using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.UptimeMonitors.Queries.GetUptimeMonitors;

public record UptimeMonitorDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string Name,
    string Url,
    int CheckIntervalMinutes,
    bool IsActive,
    string Status,
    int ResponseTimeMs,
    DateTime CreatedAt
);

public record GetUptimeMonitorsQuery() : IRequest<Result<List<UptimeMonitorDto>>>;

public class GetUptimeMonitorsQueryHandler : IRequestHandler<GetUptimeMonitorsQuery, Result<List<UptimeMonitorDto>>>
{
    private readonly IAppDbContext _context;

    public GetUptimeMonitorsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UptimeMonitorDto>>> Handle(GetUptimeMonitorsQuery request, CancellationToken cancellationToken)
    {
        var monitors = await _context.UptimeMonitors
            .AsNoTracking()
            .Include(u => u.Client)
            .Select(u => new UptimeMonitorDto(
                u.Id,
                u.ClientId,
                u.Client.Name,
                u.Name,
                u.Url,
                u.CheckIntervalMinutes,
                u.IsActive,
                "UP", // Simulated live status
                45,   // Simulated ping ms
                u.CreatedAt
            ))
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<List<UptimeMonitorDto>>.Success(monitors);
    }
}
