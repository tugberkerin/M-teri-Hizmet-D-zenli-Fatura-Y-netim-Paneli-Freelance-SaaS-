using System;
using ClientPulse.Core.Features.UptimeMonitors.Commands.CreateUptimeMonitor;
using ClientPulse.Core.Features.UptimeMonitors.Commands.DeleteUptimeMonitor;
using ClientPulse.Core.Features.UptimeMonitors.Queries.GetUptimeMonitors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ClientPulse.WebAPI.Endpoints;

public static class UptimeEndpoints
{
    public static void MapUptimeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/uptime").WithTags("UptimeMonitors");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUptimeMonitorsQuery());
            return Results.Ok(result.Data);
        })
        .WithName("GetUptimeMonitors");

        group.MapPost("/", async (CreateUptimeMonitorCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess) return Results.Ok(result.Data);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("CreateUptimeMonitor");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteUptimeMonitorCommand(id));
            if (result.IsSuccess) return Results.Ok(true);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("DeleteUptimeMonitor");
    }
}
