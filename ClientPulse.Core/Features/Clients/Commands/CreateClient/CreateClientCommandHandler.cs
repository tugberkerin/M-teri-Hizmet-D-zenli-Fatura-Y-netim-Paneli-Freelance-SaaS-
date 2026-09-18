using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Entities;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Clients.Commands.CreateClient;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<System.Guid>>
{
    private readonly IAppDbContext _context;

    public CreateClientCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<System.Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        // Benzersizlik kontrolü (E-posta üzerinden)
        var exists = await _context.Clients
            .AnyAsync(c => c.Email == request.Email, cancellationToken);
            
        if (exists)
        {
            return Result<System.Guid>.Failure("Bu e-posta adresiyle kayıtlı bir müşteri zaten var.");
        }

        var client = new Client
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            CompanyName = request.CompanyName
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<System.Guid>.Success(client.Id);
    }
}
