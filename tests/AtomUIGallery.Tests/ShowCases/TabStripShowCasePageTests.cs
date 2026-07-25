using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class TabStripShowCasePageTests
{
    [Fact]
    public void TabStrip_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml");

        source.ShouldContain("TabStripShowCaseLangResource PageSubtitle");
        source.ShouldContain("TabStripShowCaseLangResource PageDescription");
        source.ShouldNotContain("TabStripShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TabStripShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TabStripShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TabStripShowCaseLangResource ComponentCategory");
        source.ShouldContain("TabStripShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TabStripShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TabStripShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TabStripShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:TabStripShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(14);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(14);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(14);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:TabStripViewModel\"").ShouldBe(14);
        CountOccurrences(source, "BadgeText=\"v6.0.8\"").ShouldBe(2);
        source.ShouldContain("TabStripShowCaseLangResource TabStripBasicTitle");
        source.ShouldContain("TabStripShowCaseLangResource TabStripItemsSourceTitle");
        source.ShouldContain("TabStripShowCaseLangResource TabStripReorderTitle");
        source.ShouldContain("TabStripShowCaseLangResource TabStripReorderPlacementTitle");
        source.ShouldContain("TabStripShowCaseLangResource TabStripAddCloseTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void TabStrip_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TabStripShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractTabStripExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractTabStripExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(StripTabStripBehaviorMarkup(source));
    }

    private static string StripTabStripBehaviorMarkup(string source)
    {
        var normalized = Regex.Replace(
            source,
            "\\s*OptionCheckedChanged=\"Handle(?:Card)?(?:ReorderPlacement|Placement|SizeType)OptionCheckedChanged\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*AddTabRequest=\"HandleAddTabRequest\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        return normalized;
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
}
