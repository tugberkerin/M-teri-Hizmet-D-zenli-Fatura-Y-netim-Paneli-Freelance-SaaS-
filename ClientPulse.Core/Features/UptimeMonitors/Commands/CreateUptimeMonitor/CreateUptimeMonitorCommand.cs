using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Entities;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.UptimeMonitors.Commands.CreateUptimeMonitor;

public record CreateUptimeMonitorCommand(
    Guid ClientId,
    string Name,
    string Url,
    int CheckIntervalMinutes = 5
) : IRequest<Result<Guid>>;

public class CreateUptimeMonitorCommandHandler : IRequestHandler<CreateUptimeMonitorCommand, Result<Guid>>
{
    private readonly IAppDbContext _context;

    public CreateUptimeMonitorCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateUptimeMonitorCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
        {
            return Result<Guid>.Failure("Belirtilen müşteri bulunamadı.");
        }

        var monitor = new UptimeMonitor
        {
            ClientId = request.ClientId,
            Name = request.Name,
            Url = request.Url,
            CheckIntervalMinutes = request.CheckIntervalMinutes <= 0 ? 5 : request.CheckIntervalMinutes,
            IsActive = true
        };

        _context.UptimeMonitors.Add(monitor);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(monitor.Id);
    }
}
