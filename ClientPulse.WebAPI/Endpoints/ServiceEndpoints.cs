using System;
using ClientPulse.Core.Features.Services.Commands.CreateService;
using ClientPulse.Core.Features.Services.Commands.DeleteService;
using ClientPulse.Core.Features.Services.Queries.GetServices;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ClientPulse.WebAPI.Endpoints;

public static class ServiceEndpoints
{
    public static void MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/services").WithTags("Services");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetServicesQuery());
            return Results.Ok(result.Data);
        })
        .WithName("GetServices");

        group.MapPost("/", async (CreateServiceCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess) return Results.Ok(result.Data);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("CreateService");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteServiceCommand(id));
            if (result.IsSuccess) return Results.Ok(true);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("DeleteService");
    }
}
