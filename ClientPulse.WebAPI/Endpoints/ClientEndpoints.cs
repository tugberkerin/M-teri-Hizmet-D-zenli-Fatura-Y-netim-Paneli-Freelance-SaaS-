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

        group.MapPost("/", async (CreateClientCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Data); 
            }

            return Results.BadRequest(new { errors = result.Errors });
        })
        .WithName("CreateClient")
        .Produces<System.Guid>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
