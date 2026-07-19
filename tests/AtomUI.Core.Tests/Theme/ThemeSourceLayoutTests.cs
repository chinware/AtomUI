using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeSourceLayoutTests
{
    [Fact]
    public void Root_Source_Files_Use_The_Theme_Namespace()
    {
        var themeRoot = FindThemeRoot();
        var invalidFiles = FindInvalidNamespaces(themeRoot, "namespace AtomUI.Theme;");

        invalidFiles.ShouldBeEmpty();
    }

    [Fact]
    public void Algorithms_Source_Files_Use_The_Algorithms_Namespace()
    {
        var algorithmsRoot = Path.Combine(FindThemeRoot(), "Algorithms");
        var invalidFiles = FindInvalidNamespaces(
            algorithmsRoot,
            "namespace AtomUI.Theme.Algorithms;");

        invalidFiles.ShouldBeEmpty();
    }

    private static string[] FindInvalidNamespaces(string directory, string expectedNamespace)
    {
        return Directory
               .EnumerateFiles(directory, "*.cs", SearchOption.TopDirectoryOnly)
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
