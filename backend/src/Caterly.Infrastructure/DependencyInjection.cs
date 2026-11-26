using Microsoft.Extensions.DependencyInjection;

namespace Caterly.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Infrastructure services. Persistence (EF Core + Npgsql) is added with the first
    /// feature that needs a schema, together with its migration.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
