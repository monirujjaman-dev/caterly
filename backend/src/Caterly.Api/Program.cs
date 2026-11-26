using Caterly.Application;
using Caterly.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Centralized error handling: unhandled exceptions and empty error status codes become
// RFC 9457 ProblemDetails responses. Exception details are never written to the response.
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");

app.Run();

/// <summary>Entry point; exposed for <c>WebApplicationFactory</c> in integration tests.</summary>
public partial class Program;
