using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.UptimeMonitors.Commands.DeleteUptimeMonitor;

public record DeleteUptimeMonitorCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteUptimeMonitorCommandHandler : IRequestHandler<DeleteUptimeMonitorCommand, Result<bool>>
{
    private readonly IAppDbContext _context;

    public DeleteUptimeMonitorCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteUptimeMonitorCommand request, CancellationToken cancellationToken)
    {
        var monitor = await _context.UptimeMonitors.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (monitor == null)
        {
            return Result<bool>.Failure("Uptime monitör bulunamadı.");
        }

        _context.UptimeMonitors.Remove(monitor);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
