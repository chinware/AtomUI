using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUIGallery.ShowCases.ColorPicker;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Shouldly;
using Xunit;
using AtomUIColorPicker = AtomUI.Desktop.Controls.ColorPicker;
using AtomUIPopup = AtomUI.Desktop.Controls.Popup;
using AtomUITabStrip = AtomUI.Desktop.Controls.TabStrip;
using AtomUITabStripItem = AtomUI.Desktop.Controls.TabStripItem;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUIGallery.Tests.ShowCases;

public class ColorPickerShowCasePageTests
{
    [Fact]
    public void ColorPicker_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ColorPicker/Views/ColorPickerShowCase.axaml");

        source.ShouldContain("ColorPickerShowCaseLangResource PageSubtitle");
        source.ShouldContain("ColorPickerShowCaseLangResource PageDescription");
        source.ShouldNotContain("ColorPickerShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("ColorPickerShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("ColorPickerShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("ColorPickerShowCaseLangResource ComponentCategory");
        source.ShouldContain("ColorPickerShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("ColorPickerShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("ColorPickerShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("ColorPickerShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
        source.ShouldContain("GalleryShowCaseHost.SemanticPartsContentTemplate");
        source.ShouldContain("Name=\"ColorPickerSemanticPreview\"");
        source.ShouldContain("SourceKey=\"colorpicker-semantic-part\"");
        source.ShouldContain("ColorPickerShowCaseLangResource StyleClassTitle");
        source.ShouldContain("ColorPickerShowCaseLangResource SemanticPopupRootDescription");
        source.ShouldContain("semantic-styles-demo");
        source.ShouldContain("semantic-styles-white-popup");
        source.ShouldContain("semantic-styles-purple-popup");
        source.ShouldContain("atom:ColorPickerPopupRootStyle");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:ColorPickerShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("ColorPickerShowCaseLangResource BasicTitle");
        source.ShouldContain("ColorPickerShowCaseLangResource ValueBindingTitle");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("ColorPickerShowCaseLangResource PresetColorsTitle");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("ColorPickerShowCaseLangResource P2LabelSizeTypeSmall");
        source.ShouldContain("ColorPickerShowCaseLangResource P2LabelSizeTypeMiddle");
        source.ShouldContain("ColorPickerShowCaseLangResource P2LabelSizeTypeLarge");
        source.ShouldContain("ColorPickerShowCaseLangResource P2LabelSizeTypeCustom");
        source.ShouldContain("IsOccupyEntireRow=\"True\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void ColorPicker_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ColorPicker/Views/ColorPickerShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ColorPickerShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractColorPickerExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    [Fact]
    public void ColorPicker_Semantic_Popup_Is_Pinned_Open_On_First_Tab_Selection()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new ColorPickerShowCase
        {
            DataContext = new ColorPickerViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 900, window =>
        {
            page.GetVisualDescendants()
                .OfType<AtomUIColorPicker>()
                .ShouldNotContain(static picker => picker.Name == "ColorPickerSemanticOwner");

            var host = page.GetVisualDescendants().OfType<GalleryShowCaseHost>().Single();
            var tabStrip = page.GetVisualDescendants().OfType<AtomUITabStrip>().Single();
            Click(tabStrip.Items.OfType<AtomUITabStripItem>().ElementAt(1), window);
            Dispatcher.UIThread.RunJobs();

            var semanticPicker = page.GetVisualDescendants()
                                     .OfType<AtomUIColorPicker>()
                                     .Single(static picker => picker.Name == "ColorPickerSemanticOwner");
            var popup = semanticPicker.GetVisualDescendants()
                                      .OfType<AtomUIPopup>()
                                      .Single(static candidate => candidate.Name == "PART_Popup");

            semanticPicker.IsEffectivelyVisible.ShouldBeTrue();
            semanticPicker.IsPopupPinnedOpen.ShouldBeTrue();
            popup.IsOpen.ShouldBeTrue();

            host.SelectedTab = GalleryShowCaseTab.Examples;
            Dispatcher.UIThread.RunJobs();

            semanticPicker.IsEffectivelyVisible.ShouldBeFalse();
            popup.IsOpen.ShouldBeFalse();

            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            page.GetVisualDescendants()
                .OfType<AtomUIColorPicker>()
                .Single(static picker => picker.Name == "ColorPickerSemanticOwner")
                .ShouldBeSameAs(semanticPicker);
            semanticPicker.IsEffectivelyVisible.ShouldBeTrue();
            popup.IsOpen.ShouldBeTrue();
        });
    }

    private static string ExtractColorPickerExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string NormalizeMarkup(string source)
    {
        return ShowCaseSnapshotMarkup.Normalize(source);
    }

    private static string ComputeSha256(string source)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string ReadSnapshotHash(string source)
    {
        return source
            .Split('\n')
            .First(line => line.StartsWith("sha256:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim();
    }

    private static int ReadSnapshotCount(string source)
    {
        return int.Parse(source
            .Split('\n')
            .First(line => line.StartsWith("count:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim());
    }

    private static int CountShowCaseItemElements(string source)
    {
        return Regex.Matches(source, @"<gallery:ShowCaseItem(\s|>)", RegexOptions.CultureInvariant).Count;
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

    private static void ShowInWindow(
        Control content,
        double width,
        double height,
        Action<AvaloniaWindow> assertion)
    {
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            EnableOverlayLayer = true,
            Child              = content
        };
        EnablePopupOverlayLayer(visualLayerManager);

        var window = new AvaloniaWindow
        {
            Content = visualLayerManager,
            Width   = width,
            Height  = height
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static void Click(Control control, AvaloniaWindow window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);
        point.ShouldNotBeNull();

        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }

    private sealed class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }
}
