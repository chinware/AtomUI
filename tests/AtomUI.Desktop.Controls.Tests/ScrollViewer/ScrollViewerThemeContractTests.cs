using System.IO;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ScrollViewers;

public class ScrollViewerThemeContractTests
{
    [Fact]
    public void Lite_And_AutoHide_Modes_Overlay_Content_Behind_ScrollBars()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/ScrollViewer/Themes/ScrollViewerTheme.axaml");

        source.ShouldContain("Name=\"PART_ContentHost\"");
        source.ShouldContain("Selector=\"^[AllowAutoHide=True] /template/ atom|ScopeAwareOverlayLayerPanel#PART_ContentHost\"");
        source.ShouldContain("Selector=\"^[IsLiteMode=True] /template/ atom|ScopeAwareOverlayLayerPanel#PART_ContentHost\"");
        source.ShouldContain("<Setter Property=\"Grid.ColumnSpan\" Value=\"2\" />");
        source.ShouldContain("<Setter Property=\"Grid.RowSpan\" Value=\"2\" />");
        source.ShouldNotContain("Selector=\"^[AllowAutoHide=True] /template/ ScrollContentPresenter#PART_ContentPresenter\"");
        source.ShouldNotContain("Selector=\"^[IsLiteMode=True] /template/ ScrollContentPresenter#PART_ContentPresenter\"");
    }

    [Fact]
    public void Avalonia_AutoHide_Mode_Overlays_Content_Behind_ScrollBars()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/ScrollViewer/Themes/AvaScrollViewerTheme.axaml");

        source.ShouldContain("Name=\"PART_ContentHost\"");
        source.ShouldContain("Selector=\"^[AllowAutoHide=True] /template/ atom|ScopeAwareOverlayLayerPanel#PART_ContentHost\"");
        source.ShouldContain("<Setter Property=\"Grid.ColumnSpan\" Value=\"2\" />");
        source.ShouldContain("<Setter Property=\"Grid.RowSpan\" Value=\"2\" />");
        source.ShouldNotContain("Selector=\"^[AllowAutoHide=True] /template/ ScrollContentPresenter#ContentPresenter\"");
        source.ShouldNotContain("Selector=\"^[AllowAutoHide=True] /template/ ScrollContentPresenter#PART_ContentPresenter\"");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
