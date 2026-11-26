using Microsoft.Extensions.DependencyInjection;

namespace Caterly.Application;

public static class DependencyInjection
{
    /// <summary>Registers Application-layer services. Empty until the first module is added.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
