using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AtomUITour = AtomUI.Desktop.Controls.Tour;
using TourShowCasePage = AtomUIGallery.ShowCases.Tour.TourShowCase;
using TourShowCaseViewModel = AtomUIGallery.ShowCases.Tour.TourViewModel;

namespace AtomUIGallery.Tests.ShowCases;

public class TourShowCasePageTests
{
    [Fact]
    public void Tour_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tour/Views/TourShowCase.axaml");

        source.ShouldContain("TourShowCaseLangResource PageSubtitle");
        source.ShouldContain("TourShowCaseLangResource PageDescription");
        source.ShouldNotContain("TourShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TourShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TourShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TourShowCaseLangResource ComponentCategory");
        source.ShouldContain("TourShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TourShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TourShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TourShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("gallery:GalleryShowCaseHost.SemanticPartsContentTemplate");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:TourShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(8);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(8);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(8);
        // 8 个示例模板 + 1 个语义部件预览模板
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:TourViewModel\"").ShouldBe(9);
        source.ShouldContain("TourShowCaseLangResource BasicTitle");
        source.ShouldContain("TourShowCaseLangResource CustomHighlightedAreaStyleTitle");
        source.ShouldContain("IsOccupyEntireRow=\"True\"");
        source.ShouldNotContain("ElementName=");
        source.ShouldContain("<atom:TextTourIndicator />");
        source.ShouldContain("<views:SkipTourActionButton");
        source.ShouldContain("MaskColor=\"#662800FF\"");
        source.ShouldContain("IsShowMask=\"False\"");

        // 语义预览：对齐 antd Tour _semantic 演示（默认打开、锚点按钮、封面步骤）。
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Tour}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #TourSemanticOwner}\"");
        source.ShouldContain("IsPopupPinnedOpen=\"True\"");
        source.ShouldContain("Name=\"TourSemanticAnchorButton\"");
        source.ShouldNotContain("Loaded=\"HandleSemanticStageLoaded\"");
        source.ShouldContain("AttachedToVisualTree=\"HandleSemanticStageAttached\"");
        source.ShouldContain("PreviewStageMinHeight=\"600\"");
        source.ShouldContain("Path=\"root\"");
        source.ShouldContain("Path=\"popup.root\"");
        source.ShouldContain("Path=\"popup.mask\"");
        source.ShouldContain("Path=\"popup.section\"");
        source.ShouldContain("Path=\"popup.cover\"");
        source.ShouldContain("Path=\"popup.close\"");
        source.ShouldContain("Path=\"popup.header\"");
        source.ShouldContain("Path=\"popup.title\"");
        source.ShouldContain("Path=\"popup.description\"");
        source.ShouldContain("Path=\"popup.footer\"");
        source.ShouldContain("Path=\"popup.actions\"");
        source.ShouldContain("Path=\"popup.indicators\"");
        source.ShouldContain("Path=\"popup.indicator\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Tour_ShowCase_Customizes_Semantic_Parts_Only_Via_Dedicated_Part_Styles()
    {
        // 专用 Style 声明式形态锁定：语义部件的定制必须经源生成器产出的
        // Tour{Part}Style 类（含嵌套子样式）声明，禁止退化为 owner 作用域的
        // 裸类选择器 Style（如 Selector="^ atom|Button.previous-btn"）。
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tour/Views/TourShowCase.axaml");

        source.ShouldContain("<atom:TourPopupActionsStyle x:SetterTargetType=\"StackPanel\">");
        // 按钮定制嵌套在专用 Part Style 内：其前的选择器行必须仍是专用 Style 范围，
        // 即不存在脱离 Part Style 的裸 previous-btn/next-btn 选择器。
        Regex.Replace(source, @"<atom:TourPopup\w+Style[\s\S]*?</atom:TourPopup\w+Style>", string.Empty)
             .ShouldNotContain("atom|Button.previous-btn");
        Regex.Replace(source, @"<atom:TourPopup\w+Style[\s\S]*?</atom:TourPopup\w+Style>", string.Empty)
             .ShouldNotContain("atom|Button.next-btn");
        source.ShouldContain("atom:TourPopupActionsStyle");
    }

    [Fact]
    public void Tour_ShowCase_Resolves_PreRealized_Step_From_Tour_Steps()
    {
        AvaloniaTestApp.EnsureInitialized();

        var target = new Border { Name = "Target" };
        var step   = new TourStep { Name = "Step" };
        var tour   = new AtomUITour();
        tour.Steps.Add(step);

        var root = new StackPanel
        {
            Children =
            {
                target,
                tour
            }
        };

        AtomUIGallery.ShowCases.Tour.TourShowCase.SetTourStepTarget(root, step.Name, target.Name);

        step.Target.ShouldBeSameAs(target);
    }

    [Fact]
    public void Tour_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tour/Views/TourShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TourShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractTourExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    [Fact]
    public void Tour_ShowCase_Materializes_All_Deferred_Example_Contents()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new TourShowCasePage
        {
            DataContext = new TourShowCaseViewModel(new TourTestScreen())
        };

        ShowInWindow(page, 1280, 1400, () =>
        {
            var panel = page.GetVisualDescendants().OfType<ShowCasePanel>().Single();
            var items = panel.Children
                             .OfType<ShowCaseItem>()
                             .ToArray();
            items.Length.ShouldBe(8);
            foreach (var item in items)
            {
                Should.NotThrow(item.MaterializeDeferredContent);
            }
            Dispatcher.UIThread.RunJobs();
        });
    }

    private static void ShowInWindow(Control content, double width, double height, Action assertion)
    {
        var visualLayerManager = new Avalonia.Controls.Primitives.VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child = content
        };
        var window = new AvaloniaWindow
        {
            Content   = visualLayerManager,
            Width     = width,
            Height    = height
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private sealed class TourTestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }

    private static string ExtractTourExampleItems(string source)
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

    private static int CountShowCaseItemElements(string source)
    {
        return Regex.Matches(source, @"<gallery:ShowCaseItem(\s|>)").Count;
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
}
