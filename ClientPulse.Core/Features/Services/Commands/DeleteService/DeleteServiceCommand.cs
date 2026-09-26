using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Services.Commands.DeleteService;

public record DeleteServiceCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Result<bool>>
{
    private readonly IAppDbContext _context;

    public DeleteServiceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (service == null)
        {
            return Result<bool>.Failure("Hizmet bulunamadı.");
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
