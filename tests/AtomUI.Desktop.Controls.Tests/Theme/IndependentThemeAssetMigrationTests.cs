using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class IndependentThemeAssetMigrationTests
{
    [Theory]
    [InlineData("Drawer/Themes/DrawerTheme.axaml", "atom:Drawer")]
    [InlineData("Primitives/ArrowDecoratedBox/Themes/DualMonthArrowDecoratedBoxTheme.axaml", "atom:DualMonthArrowDecoratedBox")]
    [InlineData("ImagePreviewer/Themes/ImagePreviewerOverlayHostTheme.axaml", "atom:ImagePreviewerOverlayHost")]
    [InlineData("Statistic/Themes/AbstractStatisticDefaultTheme.axaml", "atom:AbstractStatistic")]
    public void Former_Aggregate_Only_Themes_Are_Independent_Leaf_Assets(
        string assetPath,
        string targetType)
    {
        var repositoryRoot = GetRepositoryRoot();
        var relativePath   = $"src/AtomUI.Desktop.Controls/{assetPath}";
        var fullPath       = Path.Combine(repositoryRoot, relativePath);

        File.Exists(fullPath).ShouldBeTrue($"Expected independent theme asset to exist: {relativePath}");
        File.ReadAllText(fullPath).ShouldContain($"TargetType=\"{targetType}\"");

        var manifest = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "src/AtomUI.Desktop.Controls/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.ThemeAssetManifestGenerator/GeneratedControlThemeAssetManifest.g.cs"));
        manifest.ShouldContain(assetPath);
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root.");
    }
}
