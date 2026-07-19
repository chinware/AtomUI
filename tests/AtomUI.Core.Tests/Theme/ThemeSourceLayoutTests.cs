using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeSourceLayoutTests
{
    private static readonly string[] s_responsibilityAreas =
    [
        "Algorithms",
        "Compilation",
        "Configuration",
        "Definitions",
        "DesignTokens",
        "Resources",
        "Schema"
    ];

    [Fact]
    public void Root_Source_Files_Use_The_Theme_Namespace()
    {
        var themeRoot = FindThemeRoot();
        var invalidFiles = FindInvalidNamespaces(
            themeRoot,
            "namespace AtomUI.Theme;",
            SearchOption.TopDirectoryOnly);

        invalidFiles.ShouldBeEmpty();
    }

    [Fact]
    public void Responsibility_Directories_Match_The_Declared_Areas()
    {
        var actualAreas = Directory
                          .EnumerateDirectories(FindThemeRoot(), "*", SearchOption.TopDirectoryOnly)
                          .Select(Path.GetFileName)
                          .OrderBy(static name => name, StringComparer.Ordinal)
                          .ToArray();

        actualAreas.ShouldBe(s_responsibilityAreas);
    }

    [Theory]
    [InlineData("Algorithms")]
    [InlineData("Compilation")]
    [InlineData("Configuration")]
    [InlineData("Definitions")]
    [InlineData("DesignTokens")]
    [InlineData("Resources")]
    [InlineData("Schema")]
    public void Responsibility_Source_Files_Use_The_Directory_Namespace(string area)
    {
        var areaRoot = Path.Combine(FindThemeRoot(), area);
        Directory.Exists(areaRoot).ShouldBeTrue($"Theme responsibility directory '{area}' must exist.");
        var invalidFiles = FindInvalidNamespaces(
            areaRoot,
            $"namespace AtomUI.Theme.{area};",
            SearchOption.AllDirectories);

        invalidFiles.ShouldBeEmpty();
    }

    private static string[] FindInvalidNamespaces(
        string directory,
        string expectedNamespace,
        SearchOption searchOption)
    {
        return Directory
               .EnumerateFiles(directory, "*.cs", searchOption)
               .Select(static path => new
               {
                   FileName = Path.GetFileName(path),
                   Namespace = File.ReadLines(path).FirstOrDefault(static line =>
                       line.StartsWith("namespace ", StringComparison.Ordinal))
               })
               .Where(source => source.Namespace != expectedNamespace)
               .Select(static source => $"{source.FileName}: {source.Namespace ?? "<missing>"}")
               .ToArray();
    }

    private static string FindThemeRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "src", "AtomUI.Core", "Theme");
            if (Directory.Exists(candidate) &&
                File.Exists(Path.Combine(directory.FullName, "src", "AtomUI.Core", "AtomUI.Core.csproj")))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate src/AtomUI.Core/Theme.");
    }
}
