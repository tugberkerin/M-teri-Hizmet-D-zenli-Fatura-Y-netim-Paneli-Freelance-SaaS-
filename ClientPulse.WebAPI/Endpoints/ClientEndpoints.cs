using ClientPulse.Core.Features.Clients.Commands.CreateClient;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ClientPulse.WebAPI.Endpoints;

public static class ClientEndpoints
{
    public static void MapClientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clients").WithTags("Clients");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new ClientPulse.Core.Features.Clients.Queries.GetClients.GetClientsQuery());
            return Results.Ok(result.Data);
        })
        .WithName("GetClients");

        group.MapGet("/{id:guid}", async (System.Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new ClientPulse.Core.Features.Clients.Queries.GetClientById.GetClientByIdQuery(id));
            if (result.IsSuccess) return Results.Ok(result.Data);
            return Results.NotFound(new { errors = result.Errors });
        })
        .WithName("GetClientById");

        group.MapPost("/", async (CreateClientCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Data); 
            }
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("CreateClient");

        group.MapPut("/{id:guid}", async (System.Guid id, ClientPulse.Core.Features.Clients.Commands.UpdateClient.UpdateClientCommand command, IMediator mediator) =>
        {
            if (id != command.Id) return Results.BadRequest(new { errors = new[] { "ID eşleşmiyor." } });
            var result = await mediator.Send(command);
            if (result.IsSuccess) return Results.Ok(true);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("UpdateClient");

        group.MapDelete("/{id:guid}", async (System.Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new ClientPulse.Core.Features.Clients.Commands.DeleteClient.DeleteClientCommand(id));
            if (result.IsSuccess) return Results.Ok(true);
            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("DeleteClient");
    }
}
