$ErrorActionPreference = "Stop"

$SolutionName = "ClientPulse"

Write-Host "Creating Solution: $SolutionName" -ForegroundColor Cyan
dotnet new sln -n $SolutionName

Write-Host "Creating Core Layer..." -ForegroundColor Cyan
dotnet new classlib -n "$SolutionName.Core" -f net9.0
dotnet sln add "$SolutionName.Core/$SolutionName.Core.csproj"

Write-Host "Creating Infrastructure Layer..." -ForegroundColor Cyan
dotnet new classlib -n "$SolutionName.Infrastructure" -f net9.0
dotnet sln add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj"
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" reference "$SolutionName.Core/$SolutionName.Core.csproj"

Write-Host "Creating WebAPI Layer..." -ForegroundColor Cyan
dotnet new webapi -n "$SolutionName.WebAPI" -f net9.0
dotnet sln add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj"
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" reference "$SolutionName.Core/$SolutionName.Core.csproj"
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" reference "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj"

Write-Host "Adding NuGet Packages to Core..." -ForegroundColor Cyan
dotnet add "$SolutionName.Core/$SolutionName.Core.csproj" package MediatR

Write-Host "Adding NuGet Packages to Infrastructure..." -ForegroundColor Cyan
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" package MongoDB.Driver
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" package Microsoft.Extensions.Caching.Hybrid
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" package Microsoft.Extensions.Caching.StackExchangeRedis
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" package Polly.Core

Write-Host "Adding NuGet Packages to WebAPI..." -ForegroundColor Cyan
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" package Microsoft.AspNetCore.OpenApi
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" package Scalar.AspNetCore

Write-Host "ClientPulse Phase 1 Setup Completed Successfully!" -ForegroundColor Green
