using AtomUI.Docs.LLMsGenerator.Catalog;
using AtomUI.Docs.LLMsGenerator.Config;
using Shouldly;
using Xunit;

namespace AtomUI.Docs.LLMsGenerator.Tests;

public class ControlInventoryTests
{
    [Fact]
    public void InventoryDiscoversAllDesktopControlsWithStableOutputPaths()
    {
        var config = new LLMsControlSetConfig
        {
            Id = "desktop",
            Platform = "desktop",
            DocsRoot = "docs/controls/desktop",
            GalleryRoot = "controlgallery/AtomUIGallery/ShowCases",
            SourceRoots = ["src/AtomUI.Desktop.Controls"],
            CategoryOrder = [
                "general",
                "layout",
                "navigation",
                "data-entry",
                "data-display",
                "feedback",
                "window",
                "other"
            ]
        };

        var controls = ControlInventory.Discover(TestRepository.RootPath, config);

        controls.Count.ShouldBe(76);
        controls.ShouldContain(control => control.Category == "general" &&
                                          control.Name == "button" &&
                                          control.OutputIndexPath == "docs/AI/llms/controls/button/index-cn.md" &&
                                          control.OutputSemanticPath == "docs/AI/llms/controls/button/semantic-cn.md");
        controls.ShouldAllBe(control => File.Exists(control.OverviewPath));
        controls.ShouldAllBe(control => File.Exists(control.ImplementationPath));
        controls.ShouldAllBe(control => File.Exists(control.ChangelogPath));

        var controlsWithoutTokenDocs = controls
            .Where(control => control.TokenPath is null)
            .Select(control => control.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        controlsWithoutTokenDocs.ShouldBe([
            "dropdown-button",
            "flex-panel",
            "grid",
            "masonry",
            "search-edit",
            "split-button",
            "tab-strip",
            "watermark"
        ]);
    }
}
