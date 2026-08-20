using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class TreeViewShowCasePageTests
{
    [Fact]
    public void TreeView_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/TreeView/Views/TreeViewShowCase.axaml");

        source.ShouldContain("TreeViewShowCaseLangResource PageSubtitle");
        source.ShouldContain("TreeViewShowCaseLangResource PageDescription");
        source.ShouldNotContain("TreeViewShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TreeViewShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TreeViewShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TreeViewShowCaseLangResource ComponentCategory");
        source.ShouldContain("TreeViewShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TreeViewShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TreeViewShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TreeViewShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
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
        source.ShouldContain("Description=\"{gallery:TreeViewShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"TreeViewItemSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #TreeViewItemSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:TreeViewItem}\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(6);
        source.ShouldContain("TreeViewShowCaseLangResource SemanticPartStyleTitle");
        CountShowCaseItemElements(source).ShouldBe(14);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(14);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(14);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:TreeViewViewModel\"").ShouldBe(15);
        source.ShouldContain("TreeViewShowCaseLangResource BasicTitle");
        source.ShouldContain("TreeViewShowCaseLangResource GenerateByTemplateTitle");
        source.ShouldContain("TreeViewShowCaseLangResource SelectionBindingTitle");
        source.ShouldContain("TreeViewShowCaseLangResource SelectionItemsBindingTitle");
        CountOccurrences(source, "BadgeText=\"v6.0.8\"").ShouldBe(2);
        CountOccurrences(source, "Span=\"Full\"").ShouldBeGreaterThanOrEqualTo(2);
        source.ShouldContain("SelectedItem=\"{Binding BoundSelectedTreeNode, Mode=TwoWay}\"");
        source.ShouldContain("SelectedItems=\"{Binding BoundSelectedTreeNodes, Mode=TwoWay}\"");
        source.ShouldContain("Text=\"{Binding BoundSelectedTreeNodeText}\"");
        source.ShouldContain("Text=\"{Binding BoundSelectedTreeNodesText}\"");
        source.ShouldContain("Click=\"HandleSelectFirstBindingTreeNodeClick\"");
        source.ShouldContain("Click=\"HandleSelectSecondBindingTreeNodeClick\"");
        source.ShouldContain("Click=\"HandleClearBindingTreeNodeSelectionClick\"");
        source.ShouldContain("Click=\"HandleSelectFirstBindingTreeNodesClick\"");
        source.ShouldContain("Click=\"HandleSelectSecondBindingTreeNodesClick\"");
        source.ShouldContain("Click=\"HandleSelectBothBindingTreeNodesClick\"");
        source.ShouldContain("Click=\"HandleClearBindingTreeNodesSelectionClick\"");
        source.ShouldContain("TreeViewShowCaseLangResource TreeWithLineTitle");
        source.ShouldContain("TreeViewShowCaseLangResource AsyncLoadDataTitle");
        source.ShouldContain("TreeViewShowCaseLangResource SearchableTitle");
        source.ShouldContain("TreeViewShowCaseLangResource ContextMenuTitle");
        source.ShouldContain("TreeViewShowCaseLangResource EmptyIndicatorTitle");
        CountOccurrences(source, "<atom:TreeView.EmptyIndicator>").ShouldBe(1);
        source.ShouldContain("IsCheckedChanged=\"HandleHoverModeChanged\"");
        source.ShouldContain("SearchRequested=\"HandleFilterItemsSourceTreeSearchRequested\"");
        source.ShouldContain("SearchRequested=\"HandleFilterTreeSearchRequested\"");
        source.ShouldContain("ItemContextMenuRequest=\"HandleContextMenuTreeItemContextMenuRequest\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void TreeView_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/TreeView/Views/TreeViewShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TreeViewShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractTreeViewExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractTreeViewExampleItems(string source)
    {
        const string firstItemMarker     = "<gallery:ShowCaseItem";
        const string semanticItemMarker  = "SourceKey=\"tree-view-semantic-part\"";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var semanticItemStart = source.IndexOf(semanticItemMarker, firstItemStart, StringComparison.Ordinal);
        semanticItemStart.ShouldBeGreaterThan(firstItemStart);
        var semanticItemStartTag = source.LastIndexOf(firstItemMarker, semanticItemStart, StringComparison.Ordinal);
        semanticItemStartTag.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..semanticItemStartTag];
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
