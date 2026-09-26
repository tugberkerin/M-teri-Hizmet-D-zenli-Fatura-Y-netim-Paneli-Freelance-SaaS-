using System;
using System.Collections.Generic;
using ClientPulse.Core.Common;
using MediatR;

namespace ClientPulse.Core.Features.Clients.Queries.GetClients;

public record ClientDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? CompanyName,
    int ServicesCount,
    int InvoicesCount,
    DateTime CreatedAt
);

public record GetClientsQuery() : IRequest<Result<List<ClientDto>>>;
