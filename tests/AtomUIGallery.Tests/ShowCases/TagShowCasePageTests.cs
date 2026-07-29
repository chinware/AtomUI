using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class TagShowCasePageTests
{
    [Fact]
    public void Tag_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag/Views/TagShowCase.axaml");

        source.ShouldContain("TagShowCaseLangResource PageSubtitle");
        source.ShouldContain("x:CompileBindings=\"True\"");
        source.ShouldContain("TagShowCaseLangResource PageDescription");
        source.ShouldNotContain("TagShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TagShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TagShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TagShowCaseLangResource ComponentCategory");
        source.ShouldContain("TagShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TagShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TagShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TagShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        source.ShouldContain("Selector=\"atom|Tag\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:TagShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("TagShowCaseLangResource BasicTitle");
        source.ShouldContain("TagShowCaseLangResource CheckableTagTitle");
        source.ShouldContain("TagShowCaseLangResource ColorfulTagTitle");
        source.ShouldContain("TagShowCaseLangResource P2TextPresetsFilled");
        source.ShouldContain("TagShowCaseLangResource P2TextPresetsSolid");
        source.ShouldContain("TagShowCaseLangResource P2TextPresetsOutlined");
        source.ShouldContain("TagShowCaseLangResource P2TextCustomFilled");
        source.ShouldContain("TagShowCaseLangResource P2TextCustomSolid");
        source.ShouldContain("TagShowCaseLangResource P2TextCustomOutlined");
        CountOccurrences(source, "TagColor=\"magenta\"").ShouldBe(3);
        CountOccurrences(source, "TagColor=\"#f50\"").ShouldBe(3);
        source.ShouldNotContain("TagColor=\"#2db7f5\" IsClosable");
        source.ShouldContain("TagShowCaseLangResource StatusTagTitle");
        source.ShouldContain("TagShowCaseLangResource IconTitle");
        source.ShouldContain("TagShowCaseLangResource VariantTitle");
        source.ShouldContain("TagShowCaseLangResource P2TextStatusFilled");
        source.ShouldContain("TagShowCaseLangResource P2TextStatusSolid");
        source.ShouldContain("TagShowCaseLangResource P2TextStatusOutlined");
        source.ShouldContain("TitlePosition=\"Left\"");
        source.ShouldContain("LoadingAnimation=\"Spin\"");
        CountOccurrences(source, "TagColor=\"processing\"").ShouldBe(3);
        source.ShouldContain("Variant=\"Filled\"");
        source.ShouldContain("Variant=\"Solid\"");
        source.ShouldContain("Variant=\"Outlined\"");
        source.ShouldNotContain("IsBordered");
        source.ShouldNotContain("BorderlessTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Tag_ShowCase_Checkable_Example_Matches_Ant_Design_Demo_State()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag/Views/TagShowCase.axaml");
        var viewModelSource = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag/ViewModels/TagViewModel.cs");

        CountOccurrences(source, "<atom:CheckableTagGroup").ShouldBe(2);
        source.ShouldContain("<atom:CheckableTag Content=\"{gallery:TagShowCaseLangResource P2ContentYes}\"");
        source.ShouldContain("IsChecked=\"{Binding IsCheckableTagChecked, Mode=TwoWay}\"");
        source.ShouldContain("CheckedItem=\"{Binding SingleCheckedTag, Mode=TwoWay}\"");
        source.ShouldContain("CheckedItems=\"{Binding MultipleCheckedTags, Mode=TwoWay}\"");
        source.ShouldContain("IsMultiple=\"True\"");

        viewModelSource.ShouldContain("IsCheckableTagChecked = true");
        viewModelSource.ShouldContain("SingleCheckedTag = \"Books\"");
        viewModelSource.ShouldContain("new ObservableCollection<object> { \"Movies\", \"Music\" }");
        viewModelSource.ShouldContain("\"Movies\", \"Books\", \"Music\", \"Sports\"");
    }

    [Fact]
    public void Tag_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag/Views/TagShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TagShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractTagExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractTagExampleItems(string source)
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
