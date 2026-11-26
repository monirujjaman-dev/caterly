using System.Reflection;

namespace Caterly.Domain.UnitTests;

public class DomainAssemblyTests
{
    private static readonly Assembly DomainAssembly = typeof(AssemblyMarker).Assembly;

    [Fact]
    public void Domain_has_no_references_to_other_Caterly_layers()
    {
        var caterlyReferences = DomainAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name)
            .Where(name => name is not null && name.StartsWith("Caterly.", StringComparison.Ordinal))
            .ToList();

        Assert.Empty(caterlyReferences);
    }

    [Fact]
    public void Domain_does_not_reference_AspNetCore_or_EntityFramework()
    {
        var forbidden = DomainAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(name =>
                name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)
                || name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal))
            .ToList();

        Assert.Empty(forbidden);
    }
}
