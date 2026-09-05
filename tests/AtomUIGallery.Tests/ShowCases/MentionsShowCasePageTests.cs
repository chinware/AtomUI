using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class MentionsShowCasePageTests
{
    [Fact]
    public void Mentions_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Mentions/Views/MentionsShowCase.axaml");

        source.ShouldContain("MentionsShowCaseLangResource PageSubtitle");
        source.ShouldContain("MentionsShowCaseLangResource PageDescription");
        source.ShouldNotContain("MentionsShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("MentionsShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("MentionsShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("MentionsShowCaseLangResource ComponentCategory");
        source.ShouldContain("MentionsShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("MentionsShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("MentionsShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("MentionsShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
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
        source.ShouldContain("Description=\"{gallery:MentionsShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(12);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(12);
        // 12 个示例模板 + 1 个语义部件预览模板
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:MentionsViewModel\"").ShouldBe(13);
        source.ShouldContain("MentionsShowCaseLangResource BasicTitle");
        source.ShouldContain("MentionsShowCaseLangResource ValueBindingTitle");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("MentionsShowCaseLangResource SizeTypeTitle");
        source.ShouldContain("MentionsShowCaseLangResource SizeTypeDescription");
        source.ShouldContain("MentionsShowCaseLangResource VariantsTitle");
        source.ShouldContain("MentionsShowCaseLangResource CustomizeTriggerTokenTitle");
        source.ShouldContain("MentionsShowCaseLangResource WithClearIconTitle");
        source.ShouldContain("SizeType=\"Large\"");
        source.ShouldContain("SizeType=\"Middle\"");
        source.ShouldContain("SizeType=\"Small\"");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("MentionsShowCaseLangResource P2PlaceholderTextCustom");
        source.ShouldContain("MentionsShowCaseLangResource StyleClassTitle");
        source.ShouldContain("SourceKey=\"mentions-semantic-part\"");
        source.ShouldContain("atom:MentionsPrefixStyle");
        source.ShouldContain("atom:MentionsPopupRootStyle");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Mentions}\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Mentions_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Mentions/Views/MentionsShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/MentionsShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractMentionsExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractMentionsExampleItems(string source)
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
