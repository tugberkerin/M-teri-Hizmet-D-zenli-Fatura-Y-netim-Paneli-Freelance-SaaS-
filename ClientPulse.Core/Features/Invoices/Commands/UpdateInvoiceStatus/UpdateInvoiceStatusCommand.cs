using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Enums;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Invoices.Commands.UpdateInvoiceStatus;

public record UpdateInvoiceStatusCommand(Guid Id, InvoiceStatus Status) : IRequest<Result<bool>>;

public class UpdateInvoiceStatusCommandHandler : IRequestHandler<UpdateInvoiceStatusCommand, Result<bool>>
{
    private readonly IAppDbContext _context;

    public UpdateInvoiceStatusCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateInvoiceStatusCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        if (invoice == null)
        {
            return Result<bool>.Failure("Fatura bulunamadı.");
        }

        invoice.Status = request.Status;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
