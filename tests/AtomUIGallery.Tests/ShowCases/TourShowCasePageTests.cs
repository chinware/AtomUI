using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Shouldly;
using Xunit;
using AtomUITour = AtomUI.Desktop.Controls.Tour;

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
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("IsStickyMirrorEnabled=\"False\"");
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
        CountShowCaseItemElements(source).ShouldBe(7);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(7);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(7);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:TourViewModel\"").ShouldBe(7);
        source.ShouldContain("TourShowCaseLangResource BasicTitle");
        source.ShouldContain("TourShowCaseLangResource CustomHighlightedAreaStyleTitle");
        source.ShouldContain("IsOccupyEntireRow=\"True\"");
        source.ShouldNotContain("ElementName=");
        source.ShouldContain("<atom:TextTourIndicator />");
        source.ShouldContain("<views:SkipTourActionButton");
        source.ShouldContain("MaskColor=\"#662800FF\"");
        source.ShouldContain("IsShowMask=\"False\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
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
