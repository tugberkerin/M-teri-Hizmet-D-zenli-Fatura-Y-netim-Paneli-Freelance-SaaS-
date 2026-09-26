using System;
using ClientPulse.Core.Enums;
using ClientPulse.Core.Features.Invoices.Commands.CreateInvoice;
using ClientPulse.Core.Features.Invoices.Commands.DeleteInvoice;
using ClientPulse.Core.Features.Invoices.Commands.UpdateInvoiceStatus;
using ClientPulse.Core.Features.Invoices.Queries.GetInvoices;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ClientPulse.WebAPI.Endpoints;

public record UpdateStatusBodyRequest(InvoiceStatus Status);

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoices").WithTags("Invoices");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetInvoicesQuery());
            return Results.Ok(result.Data);
        })
        .WithName("GetInvoices");

        group.MapPost("/", async (CreateInvoiceCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess) return Results.Ok(result.Data);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("CreateInvoice");

        group.MapPut("/{id:guid}/status", async (Guid id, UpdateStatusBodyRequest req, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateInvoiceStatusCommand(id, req.Status));
            if (result.IsSuccess) return Results.Ok(true);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("UpdateInvoiceStatus");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteInvoiceCommand(id));
            if (result.IsSuccess) return Results.Ok(true);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("DeleteInvoice");
    }
}
