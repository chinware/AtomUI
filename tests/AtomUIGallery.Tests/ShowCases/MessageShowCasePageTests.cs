using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class MessageShowCasePageTests
{
    [Fact]
    public void Message_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Message/Views/MessageShowCase.axaml");

        source.ShouldContain("MessageShowCaseLangResource PageSubtitle");
        source.ShouldContain("MessageShowCaseLangResource PageDescription");
        source.ShouldNotContain("MessageShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("MessageShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("MessageShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("MessageShowCaseLangResource ComponentCategory");
        source.ShouldContain("MessageShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("MessageShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("MessageShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("MessageShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
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
        CountShowCaseItemElements(source).ShouldBe(5);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(5);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(5);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:MessageViewModel\"").ShouldBe(6);
        source.ShouldContain("MessageShowCaseLangResource BasicTitle");
        source.ShouldContain("MessageShowCaseLangResource OtherTypesTitle");
        source.ShouldContain("MessageShowCaseLangResource LoadingIndicatorTitle");
        source.ShouldContain("MessageShowCaseLangResource CallbackTitle");
        source.ShouldContain("MessageShowCaseLangResource SemanticPartStyleTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Message_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Message/Views/MessageShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/MessageShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractMessageExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    [Fact]
    public void Message_ShowCase_Semantic_Parts_Follow_The_Two_Owner_Pattern()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Message/Views/MessageShowCase.axaml");
        var codeBehind = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Message/Views/MessageShowCase.axaml.cs");

        source.ShouldContain("GalleryShowCaseHost.SemanticPartsContentTemplate");
        CountOccurrences(source, "<gallery:SemanticPartPreview ").ShouldBe(2);
        source.ShouldContain("<gallery:SemanticPartPreview Name=\"MessageManagerSemanticPreview\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:WindowMessageManager}\"");
        source.ShouldContain("<gallery:SemanticPartPreview Name=\"MessageCardSemanticPreview\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:MessageCard}\"");

        // manager 有两个 public 构造（含无参），因此预览可声明式实例化；manager 没有声明式消息入口，
        // 示例数据由 owner 的 Loaded 事件调用 Show() 播种。
        source.ShouldContain("<atom:WindowMessageManager Name=\"MessageManagerSemanticOwner\"");
        source.ShouldContain("Loaded=\"HandleSemanticPreviewOwnerLoaded\"");
        codeBehind.ShouldContain("HandleSemanticPreviewOwnerLoaded");
        // 舞台根是 Border 而非 owner 类型，必须显式绑定 SemanticOwner，否则 owner 兼容性校验会拒绝。
        source.ShouldContain("SemanticOwner=\"{Binding #MessageManagerSemanticOwner}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #MessageCardSemanticOwner}\"");

        // 两个 owner 各自独立 descriptor，PartDescriptions 顺序与 semantic-part.md / descriptor 一致。
        var descriptions = Regex.Matches(source, "SemanticPartDescription Path=\"([^\"]+)\"")
                                .Select(static match => match.Groups[1].Value)
                                .ToArray();
        descriptions.ShouldBe(["root", "listContent", "root", "wrapper", "icon", "title"]);

        // SemanticStyles 示例必须用生成的专用 Style 类定制（专用 Style 唯一定制入口）。
        var semanticStylesItem = ExtractShowCaseItem(source, "SemanticPartStyleTitle");
        semanticStylesItem.ShouldContain("atom:MessageCardWrapperStyle");
        semanticStylesItem.ShouldContain("atom:MessageCardIconStyle");
        semanticStylesItem.ShouldContain("atom:MessageCardTitleStyle");
        semanticStylesItem.ShouldContain("x:SetterTargetType=\"DockPanel\"");
        semanticStylesItem.ShouldContain("x:SetterTargetType=\"atom:IconPresenter\"");
        semanticStylesItem.ShouldContain("x:SetterTargetType=\"SelectableTextBlock\"");
        // 部件样式必须是声明式专用 Style，不得以事件处理器做代码回退。
        semanticStylesItem.ShouldNotContain(".Loaded=");
        semanticStylesItem.ShouldNotContain(".Unloaded=");
        semanticStylesItem.ShouldNotContain("/template/ .semantic-");

        // 语义预览不得用代码回退定制部件：不得按 Name 查找模板节点后设置部件属性，也不得注册
        // AdditionalRoots（manager 内联在舞台视觉树内，无需跨根注册）。
        source.ShouldNotContain("/template/ .semantic-");
        codeBehind.ShouldNotContain("AdditionalRoots");
        codeBehind.ShouldNotContain("semantic-wrapper");
        codeBehind.ShouldNotContain("semantic-icon");
        codeBehind.ShouldNotContain("semantic-title");
        codeBehind.ShouldNotContain("semantic-list-content");
    }

    private static string ExtractShowCaseItem(string source, string titleResourceName)
    {
        var titleMarker = $"MessageShowCaseLangResource {titleResourceName}";
        var titleIndex  = source.IndexOf(titleMarker, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);

        var itemStart = source.LastIndexOf("<gallery:ShowCaseItem", titleIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        var itemEnd = source.IndexOf("</gallery:ShowCaseItem>", titleIndex, StringComparison.Ordinal);
        itemEnd.ShouldBeGreaterThan(titleIndex);

        return source[itemStart..(itemEnd + "</gallery:ShowCaseItem>".Length)];
    }

    private static string ExtractMessageExampleItems(string source)
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
