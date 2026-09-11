using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.SourceCode;

public class GalleryStickyTabsHostLayoutTests
{
    [Fact]
    public void Sticky_Mirror_Tracks_Bounds_Without_A_Permanent_LayoutUpdated_Feedback_Source()
    {
        var source = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Toolkits.GalleryBase/Controls/GalleryStickyTabsHost.cs"));

        source.ShouldContain("GetObservable(BoundsProperty)");
        source.ShouldContain("_stickyContentHostBoundsSubscription");
        source.ShouldNotContain("_inlineStickyContentHost.LayoutUpdated += HandleStickyContentHostLayoutUpdated");
        source.ShouldNotContain("private void HandleStickyContentHostLayoutUpdated");
        source.ShouldNotContain("InvalidateStickyMirrorBrush();");
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
