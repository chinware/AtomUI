using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class SelectShowCasePageTests
{
    [Fact]
    public void Select_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml");

        source.ShouldContain("SelectShowCaseLangResource PageSubtitle");
        source.ShouldContain("SelectShowCaseLangResource PageDescription");
        source.ShouldNotContain("SelectShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("SelectShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("SelectShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("SelectShowCaseLangResource ComponentCategory");
        source.ShouldContain("SelectShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("SelectShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("SelectShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("SelectShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:SelectShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(15);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(15);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(15);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:SelectViewModel\"").ShouldBe(15);
        source.ShouldContain("SelectShowCaseLangResource BasicTitle");
        source.ShouldContain("SelectShowCaseLangResource BindingTitle");
        source.ShouldContain("SelectShowCaseLangResource BindingDescription");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("SelectedOption=\"{Binding BoundSelectedOption}\"");
        source.ShouldContain("SelectedOptions=\"{Binding BoundSelectedOptions}\"");
        source.ShouldContain("SelectShowCaseLangResource CustomDropdownOptionsTitle");
        source.ShouldContain("SelectShowCaseLangResource SizesTitle");
        source.ShouldContain("OptionCheckedChanged=\"HandleSizeTypeChanged\"");
        source.ShouldContain("CustomizableSizeType.Custom");
        source.ShouldContain("SelectShowCaseLangResource P2ContentCustom");
        source.ShouldContain("atom|Select.size-demo-select[SizeType=Custom]");
        source.ShouldContain("Property=\"Height\" Value=\"38\"");
        source.ShouldContain("Property=\"FontSize\" Value=\"15\"");
        source.ShouldContain("AttachedToVisualTree=\"HandleCustomSearchSelectAttached\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Select_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/SelectShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractSelectExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractSelectExampleItems(string source)
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
        source = Regex.Replace(
            source,
            @"\s*AttachedToVisualTree=""HandleCustomSearchSelectAttached""",
            string.Empty,
            RegexOptions.CultureInvariant);

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
