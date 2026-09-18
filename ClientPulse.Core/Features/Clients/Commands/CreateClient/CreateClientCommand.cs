using ClientPulse.Core.Common;
using MediatR;
using System;

namespace ClientPulse.Core.Features.Clients.Commands.CreateClient;

public record CreateClientCommand(string Name, string Email, string? Phone, string? CompanyName) : IRequest<Result<Guid>>;
