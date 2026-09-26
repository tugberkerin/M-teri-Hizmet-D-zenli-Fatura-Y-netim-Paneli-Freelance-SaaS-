using System;
using System.Threading;
using System.Threading.Tasks;
using ClientPulse.Core.Common;
using ClientPulse.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClientPulse.Core.Features.Invoices.Commands.DeleteInvoice;

public record DeleteInvoiceCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, Result<bool>>
{
    private readonly IAppDbContext _context;

    public DeleteInvoiceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        if (invoice == null)
        {
            return Result<bool>.Failure("Fatura bulunamadı.");
        }

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
