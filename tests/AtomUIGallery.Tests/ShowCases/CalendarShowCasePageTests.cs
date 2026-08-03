using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class CalendarShowCasePageTests
{
    [Fact]
    public void Calendar_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");

        source.ShouldContain("CalendarShowCaseLangResource PageSubtitle");
        source.ShouldContain("CalendarShowCaseLangResource PageDescription");
        source.ShouldNotContain("CalendarShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("CalendarShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("CalendarShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("CalendarShowCaseLangResource ComponentCategory");
        source.ShouldContain("CalendarShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("CalendarShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("CalendarShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("CalendarShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("MaxColumns=\"1\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:CalendarShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("CalendarShowCaseLangResource BasicTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Calendar_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/CalendarShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractCalendarExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    [Fact]
    public void Calendar_Card_ShowCase_UsesExternalContainerAndReferenceCopy()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var enUs = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/zh_TW.cs");

        source.ShouldContain("CalendarShowCaseLangResource CardTitle");
        source.ShouldContain("CalendarShowCaseLangResource CardDescription");
        source.ShouldContain("<Border Width=\"300\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldContain("BorderBrush=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
        source.ShouldContain("BorderThickness=\"{atom:SharedTokenResource BorderThickness}\"");
        source.ShouldContain("CornerRadius=\"{atom:SharedTokenResource BorderRadiusLG}\"");

        enUs.ShouldContain("public const string CardTitle = \"Card\";");
        enUs.ShouldContain("public const string CardDescription = \"Nested inside a container element for rendering in limited space.\";");
        zhCn.ShouldContain("public const string CardTitle = \"卡片模式\";");
        zhCn.ShouldContain("public const string CardDescription = \"用于嵌套在空间有限的容器中。\";");
        zhTw.ShouldContain("public const string CardTitle = \"卡片模式\";");
        zhTw.ShouldContain("public const string CardDescription = \"用於嵌套在空間有限的容器中。\";");
    }

    private static string ExtractCalendarExampleItems(string source)
    {
        const string firstItemMarker = "<gallery:ShowCaseItem";
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

    private static int CountOccurrences(string source, string value)
    {
        var count = 0;
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
