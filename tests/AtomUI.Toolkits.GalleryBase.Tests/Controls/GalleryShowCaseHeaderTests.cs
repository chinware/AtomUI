using AtomUI.Desktop.Controls;
using AtomUI.Theme;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUITextBlock = AtomUI.Desktop.Controls.TextBlock;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class GalleryShowCaseHeaderTests
{
    static GalleryShowCaseHeaderTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Uses_Documented_Defaults()
    {
        var header = new GalleryShowCaseHeader();

        header.CategoryTagColor.ShouldBe("blue");
        header.StatusTagColor.ShouldBe("success");
        header.IntroducedVersionTagColor.ShouldBe("blue");
        header.IsIntroducedVersionTagBordered.ShouldBeFalse();
    }

    [Fact]
    public void Registers_Token_Resource_Scope()
    {
        var header = new GalleryShowCaseHeader();

        var scopeProvider = ControlTokenResourceScopeHost.GetTokenResourceScopeProvider(header);

        scopeProvider.ShouldNotBeNull();
        scopeProvider.Id.ShouldBe(GalleryShowCaseHeader.LanguageId);
    }

    [Fact]
    public void Hides_Optional_Sections_When_Values_Are_Blank()
    {
        var header = new GalleryShowCaseHeader
        {
            Title             = "ImagePreviewer",
            Category          = "  ",
            Status            = null,
            IntroducedVersion = "",
            Subtitle          = " ",
            Description       = null,
            Namespace         = "",
            Package           = null,
            BaseClass         = " "
        };

        ShowInWindow(header, () =>
        {
            Find<Tag>(header, "PART_CategoryTag").IsVisible.ShouldBeFalse();
            Find<Tag>(header, "PART_StatusTag").IsVisible.ShouldBeFalse();
            Find<Tag>(header, "PART_IntroducedVersionTag").IsVisible.ShouldBeFalse();
            Find<AtomUITextBlock>(header, "PART_SubtitleText").IsVisible.ShouldBeFalse();
            Find<AtomUITextBlock>(header, "PART_DescriptionText").IsVisible.ShouldBeFalse();
            Find<Border>(header, "PART_MetadataCard").IsVisible.ShouldBeFalse();
        });
    }

    [Fact]
    public void Shows_Provided_Tags_And_Metadata_In_Fixed_Order()
    {
        var header = new GalleryShowCaseHeader
        {
            Title             = "Splash",
            Category          = "Other",
            Status            = "Preview",
            StatusTagColor    = "processing",
            IntroducedVersion = "v6.0.7",
            Subtitle          = "Splash screen",
            Description       = "Display startup progress.",
            Namespace         = "AtomUI.Desktop.Controls",
            Package           = "AtomUI.Desktop.Controls.Extras",
            BaseClass         = "ContentControl"
        };

        ShowInWindow(header, () =>
        {
            Find<Tag>(header, "PART_CategoryTag").Text.ShouldBe("Other");
            Find<Tag>(header, "PART_StatusTag").Text.ShouldBe("Preview");
            Find<Tag>(header, "PART_StatusTag").TagColor.ShouldBe("processing");
            Find<Tag>(header, "PART_IntroducedVersionTag").Text.ShouldBe("v6.0.7");
            Find<Tag>(header, "PART_IntroducedVersionTag").IsBordered.ShouldBeFalse();

            var metadataItems = Find<WrapPanel>(header, "PART_MetadataItems")
                .Children
                .OfType<StackPanel>()
                .ToList();

            metadataItems.Count.ShouldBe(3);
            ReadMetadataValue(metadataItems[0]).ShouldBe("AtomUI.Desktop.Controls");
            ReadMetadataValue(metadataItems[1]).ShouldBe("AtomUI.Desktop.Controls.Extras");
            ReadMetadataValue(metadataItems[2]).ShouldBe("ContentControl");
        });
    }

    private static string? ReadMetadataValue(StackPanel item)
    {
        return item.Children
                   .OfType<AtomUITextBlock>()
                   .Last()
                   .Text;
    }

    private static T Find<T>(Control root, string name)
        where T : Control
    {
        var control = root.GetVisualDescendants()
                          .OfType<T>()
                          .FirstOrDefault(descendant => descendant.Name == name);
        control.ShouldNotBeNull($"Expected template part {name} to exist.");
        return control;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 760,
            Height  = 240,
            Content = content
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        content.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();

        try
        {
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
