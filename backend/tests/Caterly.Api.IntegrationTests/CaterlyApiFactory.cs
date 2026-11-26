using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Caterly.Api.IntegrationTests;

/// <summary>
/// Test host for the API. Appends a terminal middleware that throws on <see cref="ThrowPath"/>
/// so the centralized exception handler can be verified without a production test endpoint.
/// </summary>
public sealed class CaterlyApiFactory : WebApplicationFactory<Program>
{
    public const string ThrowPath = "/__test/throw";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services => services.AddTransient<IStartupFilter, ThrowingStartupFilter>());
    }

    private sealed class ThrowingStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            next(app);
            app.Use((HttpContext context, RequestDelegate nextDelegate) =>
                context.Request.Path == ThrowPath
                    ? throw new InvalidOperationException("Sensitive internal detail")
                    : nextDelegate(context));
        };
    }
}
