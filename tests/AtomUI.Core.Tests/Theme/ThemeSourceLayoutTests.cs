using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeSourceLayoutTests
{
    [Fact]
    public void Root_Source_Files_Use_The_Theme_Namespace()
    {
        var themeRoot = FindThemeRoot();
        var invalidFiles = Directory
                           .EnumerateFiles(themeRoot, "*.cs", SearchOption.TopDirectoryOnly)
                           .Select(static path => new
                           {
                               FileName = Path.GetFileName(path),
                               Namespace = File.ReadLines(path).FirstOrDefault(static line =>
                                   line.StartsWith("namespace ", StringComparison.Ordinal))
                           })
                           .Where(static source => source.Namespace != "namespace AtomUI.Theme;")
                           .Select(static source => $"{source.FileName}: {source.Namespace ?? "<missing>"}")
                           .ToArray();

        invalidFiles.ShouldBeEmpty();
    }

    private static string FindThemeRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "src", "AtomUI.Core", "Theme");
            if (File.Exists(Path.Combine(candidate, "Theme.cs")))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate src/AtomUI.Core/Theme.");
    }
}
