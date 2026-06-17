using System;
using System.IO;
using System.Linq;
using AtomUIGallery.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomRibbonBadge = AtomUI.Desktop.Controls.RibbonBadge;
using AtomSeparator = AtomUI.Desktop.Controls.Separator;
using AvaloniaWindow = Avalonia.Controls.Window;

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

    [Fact]
    public void ShowCaseItem_Integrates_RibbonBadge_For_Feature_Version_Marker()
    {
        var itemSource = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItem.axaml.cs");
        var itemTheme  = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItemTheme.axaml");

        itemSource.ShouldContain("BadgeTextProperty");
        itemSource.ShouldContain("BadgeColorProperty");
        itemSource.ShouldContain("IsBadgeVisibleProperty");
        itemTheme.ShouldContain("Selector=\"^[IsBadgeVisible=True]\"");
        itemTheme.ShouldContain("<atom:RibbonBadge");
        itemTheme.ShouldContain("Text=\"{TemplateBinding BadgeText}\"");
        itemTheme.ShouldContain("RibbonColor=\"{TemplateBinding BadgeColor}\"");
        itemTheme.ShouldNotContain("RibbonBadgeText");
        itemTheme.ShouldNotContain("FeatureBadgeText");
        itemTheme.ShouldNotContain("PART_FeatureBadge");
        itemTheme.ShouldNotContain("HorizontalAlignment=\"Right\"");
        itemTheme.ShouldNotContain("VerticalAlignment=\"Top\"");
        itemTheme.ShouldNotContain("IsHitTestVisible=\"False\"");
    }

    [Fact]
    public void ShowCaseItem_Badge_Template_Keeps_Card_Content()
    {
        AvaloniaTestApp.EnsureInitialized();

        var item = new ShowCaseItem
        {
            Title       = "Feature",
            Description = "Feature item",
            BadgeText   = "v6.0.5",
            Content     = new TextBlock
            {
                Text = "content"
            }
        };

        ShowInWindow(item, () =>
        {
            var ribbonBadge = item.GetVisualDescendants()
                                  .OfType<AtomRibbonBadge>()
                                  .SingleOrDefault();
            ribbonBadge.ShouldNotBeNull();
            ribbonBadge.Text.ShouldBe("v6.0.5");

            item.GetVisualDescendants()
                .OfType<AtomSeparator>()
                .Single()
                .Title
                .ShouldBe("Feature");
            item.GetVisualDescendants()
                .OfType<TextBlock>()
                .Any(textBlock => textBlock.Text == "Feature item")
                .ShouldBeTrue();
            item.GetVisualDescendants()
                .OfType<TextBlock>()
                .Any(textBlock => textBlock.Text == "content")
                .ShouldBeTrue();
        });
    }

    [Fact]
    public void ShowCaseItem_Deferred_Content_Template_Builds_Only_When_Materialized()
    {
        AvaloniaTestApp.EnsureInitialized();
        var oldValue = SetDeferredLoadingEnvironment(null);
        GalleryShowCaseRuntimeOptions.ResetDeferredLoadingDisabledOverride();
        try
        {
            var buildCount = 0;
            var item = new ShowCaseItem
            {
                IsDeferredContentEnabled = true,
                DataContext              = "demo data",
                DeferredContentTemplate  = new FuncDataTemplate<object?>((data, _) =>
                {
                    buildCount++;
                    return new TextBlock
                    {
                        Text = data?.ToString()
                    };
                }, true)
            };

            buildCount.ShouldBe(0);
            item.IsDeferredContentMaterialized.ShouldBeFalse();

            item.MaterializeDeferredContent();

            buildCount.ShouldBe(1);
            item.IsDeferredContentMaterialized.ShouldBeTrue();
            item.Content.ShouldBeOfType<TextBlock>().Text.ShouldBe("demo data");

            item.MaterializeDeferredContent();
            buildCount.ShouldBe(1);
        }
        finally
        {
            SetDeferredLoadingEnvironment(oldValue);
            GalleryShowCaseRuntimeOptions.ResetDeferredLoadingDisabledOverride();
        }
    }

    [Fact]
    public void ShowCaseItem_Materializes_Deferred_Template_Immediately_When_Diagnostics_Disables_Defer()
    {
        AvaloniaTestApp.EnsureInitialized();
        var oldValue = SetDeferredLoadingEnvironment(null);
        GalleryShowCaseRuntimeOptions.ResetDeferredLoadingDisabledOverride();
        try
        {
            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled = true;

            var buildCount = 0;
            var item = new ShowCaseItem
            {
                IsDeferredContentEnabled = true,
                DataContext              = "diagnostic data",
                DeferredContentTemplate  = new FuncDataTemplate<object?>((data, _) =>
                {
                    buildCount++;
                    return new TextBlock
                    {
                        Text = data?.ToString()
                    };
                }, true)
            };

            buildCount.ShouldBe(1);
            item.IsDeferredContentMaterialized.ShouldBeTrue();
            item.Content.ShouldBeOfType<TextBlock>().Text.ShouldBe("diagnostic data");
        }
        finally
        {
            SetDeferredLoadingEnvironment(oldValue);
            GalleryShowCaseRuntimeOptions.ResetDeferredLoadingDisabledOverride();
        }
    }

    [Fact]
    public void ShowCase_Runtime_Options_Do_Not_Let_Runtime_Stop_Override_Environment_Disable()
    {
        GalleryShowCaseRuntimeOptions.ResetDeferredLoadingDisabledOverride();
        var oldValue = SetDeferredLoadingEnvironment("1");
        try
        {
            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled.ShouldBeTrue();

            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled = false;

            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled.ShouldBeTrue();
        }
        finally
        {
            SetDeferredLoadingEnvironment(oldValue);
            GalleryShowCaseRuntimeOptions.ResetDeferredLoadingDisabledOverride();
        }
    }

    [Fact]
    public void ShowCasePanel_Supports_Opt_In_Viewport_Driven_Deferred_Loading()
    {
        var panelSource = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCasePanel.axaml.cs");
        var itemSource  = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItem.axaml.cs");
        var itemTheme   = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItemTheme.axaml");
        var itemToken   = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItemToken.cs");

        panelSource.ShouldContain("IsDeferredLoadingEnabledProperty");
        panelSource.ShouldContain("InitialDeferredLoadItemCountProperty");
        panelSource.ShouldContain("DeferredLoadBatchSizeProperty");
        panelSource.ShouldContain("DeferredLoadViewportBufferProperty");
        panelSource.ShouldContain("EffectiveViewportChanged += HandleEffectiveViewportChanged");
        panelSource.ShouldContain("EffectiveViewportChanged -= HandleEffectiveViewportChanged");
        CountOccurrences(panelSource, "EffectiveViewportChanged +=").ShouldBe(1);
        panelSource.ShouldContain("MaterializeDeferredContentInViewport");
        panelSource.ShouldContain("MaterializeAllDeferredContent");
        panelSource.ShouldContain("GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled");
        panelSource.ShouldContain("!GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled");
        panelSource.ShouldContain("GalleryShowCaseRuntimeOptions.DeferredLoadingDisabledChanged += HandleDeferredLoadingDisabledChanged");
        panelSource.ShouldContain("GalleryShowCaseRuntimeOptions.DeferredLoadingDisabledChanged -= HandleDeferredLoadingDisabledChanged");

        itemSource.ShouldContain("IsDeferredContentEnabledProperty");
        itemSource.ShouldContain("DeferredContentTemplateProperty");
        itemSource.ShouldContain("DeferredContentProperty");
        itemSource.ShouldContain("DeferredPlaceholderHeightProperty");
        itemSource.ShouldContain("public void MaterializeDeferredContent()");
        itemSource.ShouldContain("GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled");
        itemTheme.ShouldContain("PART_DeferredPlaceholder");
        itemTheme.ShouldContain("DeferredPlaceholderHeight");
        itemTheme.ShouldContain("ShowCaseItemTokenResource DeferredPlaceholderHeight");
        itemToken.ShouldContain("DeferredPlaceholderHeight");
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

    private static int CountOccurrences(string source, string value)
    {
        var count      = 0;
        var startIndex = 0;
        while (true)
        {
            var matchIndex = source.IndexOf(value, startIndex, StringComparison.Ordinal);
            if (matchIndex < 0)
            {
                return count;
            }

            count++;
            startIndex = matchIndex + value.Length;
        }
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Content = content,
            Width   = 640,
            Height  = 480
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            content.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static string? SetDeferredLoadingEnvironment(string? value)
    {
        var oldValue = Environment.GetEnvironmentVariable(
            GalleryShowCaseRuntimeOptions.DisableDeferredLoadingEnvironmentVariable);
        Environment.SetEnvironmentVariable(
            GalleryShowCaseRuntimeOptions.DisableDeferredLoadingEnvironmentVariable,
            value);
        return oldValue;
    }
}
