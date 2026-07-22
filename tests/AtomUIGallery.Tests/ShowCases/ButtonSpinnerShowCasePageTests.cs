using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ButtonSpinnerShowCasePageTests
{
    [Fact]
    public void ButtonSpinner_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerShowCase.axaml");

        source.ShouldContain("ButtonSpinnerShowCaseLangResource PageSubtitle");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource PageDescription");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource ComponentCategory");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:ButtonSpinnerShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(7);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(7);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(7);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:ButtonSpinnerViewModel\"").ShouldBe(7);
        source.ShouldContain("ButtonSpinnerShowCaseLangResource BasicTitle");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource ThreeSizesTitle");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource P2LabelSizeTypeLarge");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource P2LabelSizeTypeMiddle");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource P2LabelSizeTypeSmall");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource P2LabelSizeTypeCustom");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource StatusTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void ButtonSpinner_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ButtonSpinnerShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractButtonSpinnerExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractButtonSpinnerExampleItems(string source)
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
}
