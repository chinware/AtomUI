using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Controls;

public class ShowCasePanelStructureTests
{
    [Fact]
    public void ShowCasePanel_Uses_Masonry_Panel_Instead_Of_Manual_Grid_Placement()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCasePanel.axaml.cs");
        var theme  = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCasePanelTheme.axaml");

        theme.ShouldContain("gallery:ShowCaseMasonryPanel");
        theme.ShouldNotContain("<Grid Margin=\"5\" Name=\"PART_MainPanel\"");
        source.ShouldNotContain("new ShowCaseItem()");
        source.ShouldNotContain("IsFake = true");
        source.ShouldNotContain("Grid.SetRow");
        source.ShouldNotContain("Grid.SetColumn");
        source.ShouldNotContain("Grid.SetColumnSpan");
        source.ShouldNotContain("LogicalChildren.Add");
    }

    [Fact]
    public void ShowCasePanel_And_Item_Use_Gallery_Tokens_For_Layout_And_Cards()
    {
        var panelTheme = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCasePanelTheme.axaml");
        var itemTheme  = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItemTheme.axaml");
        var panelToken = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCasePanelToken.cs");
        var itemToken  = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItemToken.cs");

        panelToken.ShouldContain("[ControlDesignToken]");
        itemToken.ShouldContain("[ControlDesignToken]");
        panelTheme.ShouldContain("ShowCasePanelTokenResource");
        panelTheme.ShouldContain("ContentMargin\" Value=\"{gallery:ShowCasePanelTokenResource ContentMargin}");
        panelTheme.ShouldContain("Margin=\"{TemplateBinding ContentMargin}\"");
        panelTheme.ShouldContain("VerticalScrollBarVisibility=\"Auto\"");
        panelTheme.ShouldContain("Selector=\"^[IsScrollEnabled=False]\"");
        itemTheme.ShouldContain("ShowCaseItemTokenResource");
        var panelSource = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCasePanel.axaml.cs");
        panelSource.ShouldContain("ContentMarginProperty");
        panelSource.ShouldContain("IsScrollEnabledProperty");
        panelSource.ShouldContain("public bool IsScrollEnabled");
        panelTheme.ShouldNotContain("Margin=\"5\"");
        itemTheme.ShouldNotContain("Padding=\"20\"");
        itemTheme.ShouldNotContain("CornerRadius=\"8\"");
        itemTheme.ShouldNotContain("Margin=\"0, 0, 0, 40\"");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
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

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
