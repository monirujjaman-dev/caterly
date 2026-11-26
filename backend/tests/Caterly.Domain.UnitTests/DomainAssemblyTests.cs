using System.Reflection;
using System.Xml.Linq;

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

    // GetReferencedAssemblies() omits references the compiler drops as unused,
    // so also check what the project file declares.
    [Fact]
    public void Domain_project_declares_no_project_or_forbidden_package_references()
    {
        var project = XDocument.Load(FindDomainProjectFile());

        var projectReferences = project.Descendants("ProjectReference").ToList();
        Assert.Empty(projectReferences);

        string[] forbiddenPrefixes =
        [
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore",
            "Npgsql",
            "Caterly.",
        ];
        var forbiddenPackages = project
            .Descendants("PackageReference")
            .Select(p => (string?)p.Attribute("Include") ?? string.Empty)
            .Where(name => forbiddenPrefixes.Any(prefix =>
                name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        Assert.Empty(forbiddenPackages);
    }

    private static string FindDomainProjectFile()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Caterly.slnx")))
        {
            dir = dir.Parent;
        }

        Assert.NotNull(dir);
        var path = Path.Combine(dir.FullName, "src", "Caterly.Domain", "Caterly.Domain.csproj");
        Assert.True(File.Exists(path), $"Domain project file not found at {path}");
        return path;
    }
}
