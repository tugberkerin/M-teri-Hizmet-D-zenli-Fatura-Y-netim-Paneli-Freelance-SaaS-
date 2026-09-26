using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Clients.Commands.DeleteClient;

public record DeleteClientCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, Result<bool>>
{
    private readonly IAppDbContext _context;

    public DeleteClientCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (client == null)
        {
            return Result<bool>.Failure("Müşteri bulunamadı.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
