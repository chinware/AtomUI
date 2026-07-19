using Shouldly;

namespace AtomUI.Desktop.Controls.Tests.Theme;

internal static class ThemeAssetScopeAssertions
{
    internal static void AssertDirectoryUsesSharedTokenScope(string relativeDirectory)
    {
        var themeFiles = Directory.GetFiles(
            GetRepoFile(relativeDirectory),
            "*.axaml",
            SearchOption.AllDirectories);

        themeFiles.ShouldNotBeEmpty();
        var sharedTokenFiles = new List<string>();
        foreach (var themeFile in themeFiles)
        {
            var text = File.ReadAllText(themeFile);
            text.ShouldNotContain("TokenSharedTokenResource");
            if (text.Contains("{atom:SharedTokenResource ", StringComparison.Ordinal))
            {
                text.ShouldContain("themeResources:ControlTokenScope.Identity=");
                sharedTokenFiles.Add(themeFile);
            }
        }

        sharedTokenFiles.ShouldNotBeEmpty();
    }

    internal static void AssertFileUsesSharedTokenScope(string relativePath)
    {
        var text = File.ReadAllText(GetRepoFile(relativePath));
        text.ShouldContain("{atom:SharedTokenResource ");
        text.ShouldContain("themeResources:ControlTokenScope.Identity=");
        text.ShouldNotContain("TokenSharedTokenResource");
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = AppContext.BaseDirectory;
        while (directory is not null)
        {
            var candidate = Path.Combine(directory, relativePath);
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new FileNotFoundException(relativePath);
    }
}
