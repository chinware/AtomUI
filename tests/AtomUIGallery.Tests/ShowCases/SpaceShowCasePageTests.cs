using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class SpaceShowCasePageTests
{
    [Fact]
    public void Space_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Space/Views/SpaceShowCase.axaml");

        source.ShouldContain("SpaceShowCaseLangResource PageSubtitle");
        source.ShouldContain("SpaceShowCaseLangResource PageDescription");
        source.ShouldNotContain("SpaceShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("SpaceShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("SpaceShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("SpaceShowCaseLangResource ComponentCategory");
        source.ShouldContain("SpaceShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("SpaceShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("SpaceShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("SpaceShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:SpaceShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(9);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(9);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(9);
        CountOccurrences(source, "DataTemplate x:DataType=\"viewModels:SpaceViewModel\"").ShouldBe(9);
        source.ShouldContain("SpaceShowCaseLangResource BasicTitle");
        source.ShouldContain("SpaceShowCaseLangResource SizeTitle");
        source.ShouldContain("SpaceShowCaseLangResource AlignTitle");
        source.ShouldContain("SpaceShowCaseLangResource CompactFormTitle");
        source.ShouldContain("SpaceShowCaseLangResource CompactButtonTitle");
        source.ShouldContain("Name=\"SizeDemoSpace\"");
        source.ShouldContain("ValueChanged=\"HandleCustomSpacingValueChanged\"");
        source.ShouldNotContain("{Binding #CustomSizeSlider.Value");
        source.ShouldNotContain("ItemSpacing=\"{Binding CustomSpacingValue, Priority=Template}\"");
        source.ShouldNotContain("LineSpacing=\"{Binding CustomSpacingValue, Priority=Template}\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Space_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Space/Views/SpaceShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/SpaceShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractSpaceExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractSpaceExampleItems(string source)
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
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        return Path.Combine(repoRoot, relativePath);
    }
}
