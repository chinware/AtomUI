using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class FloatButtonShowCasePageTests
{
    [Fact]
    public void FloatButton_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/FloatButton/Views/FloatButtonShowCase.axaml");

        source.ShouldContain("FloatButtonShowCaseLangResource PageSubtitle");
        source.ShouldContain("FloatButtonShowCaseLangResource PageDescription");
        source.ShouldNotContain("FloatButtonShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("FloatButtonShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("FloatButtonShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("FloatButtonShowCaseLangResource ComponentCategory");
        source.ShouldContain("FloatButtonShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("FloatButtonShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("FloatButtonShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("FloatButtonShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
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
        source.ShouldContain("Description=\"{gallery:FloatButtonShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(12);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(12);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:FloatButtonViewModel\"").ShouldBe(12);
        source.ShouldContain("FloatButtonShowCaseLangResource BasicTitle");
        source.ShouldContain("FloatButtonShowCaseLangResource CommandTitle");
        source.ShouldContain("FloatButtonShowCaseLangResource ControlledModeTitle");
        source.ShouldContain("FloatButtonShowCaseLangResource PlacementTitle");
        source.ShouldContain("FloatButtonShowCaseLangResource BackTopTitle");
        source.ShouldContain("IsOccupyEntireRow=\"True\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void FloatButton_Command_ShowCase_Displays_Feedback_Near_Action_Buttons()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/FloatButton/Views/FloatButtonShowCase.axaml");

        source.ShouldContain("Name=\"CommandFeedbackCard\"");
        source.ShouldContain("HorizontalAlignment=\"Right\"");
        source.ShouldContain("VerticalAlignment=\"Bottom\"");
        source.ShouldContain("Margin=\"24,24,160,24\"");
        source.ShouldContain("Text=\"{Binding CommandClickCount}\"");
        source.ShouldContain("Text=\"{Binding LastCommandSource}\"");
    }

    [Fact]
    public void FloatButton_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/FloatButton/Views/FloatButtonShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/FloatButtonShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractFloatButtonExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractFloatButtonExampleItems(string source)
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
