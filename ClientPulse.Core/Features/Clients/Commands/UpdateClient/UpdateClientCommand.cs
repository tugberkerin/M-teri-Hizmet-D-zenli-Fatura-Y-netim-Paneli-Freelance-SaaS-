using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Clients.Commands.UpdateClient;

public record UpdateClientCommand(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? CompanyName
) : IRequest<Result<bool>>;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Result<bool>>
{
    private readonly IAppDbContext _context;

    public UpdateClientCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (client == null)
        {
            return Result<bool>.Failure("Müşteri bulunamadı.");
        }

        var emailExists = await _context.Clients.AnyAsync(c => c.Email == request.Email && c.Id != request.Id, cancellationToken);
        if (emailExists)
        {
            return Result<bool>.Failure("Bu e-posta başka bir müşteri tarafından kullanılıyor.");
        }

        client.Name = request.Name;
        client.Email = request.Email;
        client.Phone = request.Phone;
        client.CompanyName = request.CompanyName;
        client.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
