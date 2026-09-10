using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class DropdownButtonShowCasePageTests
{
    [Fact]
    public void DropdownButton_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/DropdownButton/Views/DropdownButtonShowCase.axaml");

        source.ShouldContain("DropdownButtonShowCaseLangResource PageSubtitle");
        source.ShouldContain("DropdownButtonShowCaseLangResource PageDescription");
        source.ShouldNotContain("DropdownButtonShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("DropdownButtonShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("DropdownButtonShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("DropdownButtonShowCaseLangResource ComponentCategory");
        source.ShouldContain("DropdownButtonShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("DropdownButtonShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("DropdownButtonShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("DropdownButtonShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:DropdownButtonShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(6);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(6);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(6);
        // 6 个示例模板 + 1 个语义部件预览模板
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:DropdownButtonViewModel\"").ShouldBe(7);
        CountOccurrences(source, "<gallery:ShowCaseItem.Styles>").ShouldBe(2);
        source.ShouldContain("DropdownButtonShowCaseLangResource BasicTitle");
        source.ShouldContain("DropdownButtonShowCaseLangResource ButtonTypesTitle");
        source.ShouldContain("DropdownButtonShowCaseLangResource SizeTypeTitle");
        source.ShouldContain("SizeType=\"Large\"");
        source.ShouldContain("SizeType=\"Middle\"");
        source.ShouldContain("SizeType=\"Small\"");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("Height=\"44\"");
        source.ShouldContain("Padding=\"18,0\"");
        source.ShouldContain("FontSize=\"15\"");
        source.ShouldContain("DropdownButtonShowCaseLangResource PlacementTitle");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticStylesTitle");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticStylesDescription");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticStylesObjectButton");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticStylesFunctionButton");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticStylesItemProfile");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticStylesItemSettings");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticStylesItemLogout");
        source.ShouldContain("<StackPanel Spacing=\"24\">");
        source.ShouldContain("ButtonType=\"Primary\"");
        source.ShouldContain("TriggerType=\"Click\"");
        // 命名对齐全局惯例（AbstractSelect/TreeSelect/Cascader/AutoComplete 的 IsPopupMatchSelectWidth）。
        source.ShouldContain("IsPopupMatchSelectWidth=\"True\"");
        source.ShouldNotContain("MatchAnchorWidth");
        source.ShouldContain("Value=\"#d9d9d9\"");
        source.ShouldContain("Value=\"4\"");
        source.ShouldContain("Value=\"#1890ff\"");
        source.ShouldContain("Value=\"8\"");
        source.ShouldContain("Kind=LogoutOutlined");
        source.ShouldContain("Kind=SettingOutlined");
        source.ShouldContain("<atom:MenuSeparator />");
        source.ShouldContain("Foreground=\"{atom:SharedTokenResource ColorError}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:DropdownButton}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #DropdownButtonSemanticOwner}\"");
        source.ShouldContain("<atom:DropdownButtonPopupRootStyle x:SetterTargetType=\"atom:ArrowDecoratedBox\">");
        source.ShouldContain("<atom:DropdownButtonItemStyle x:SetterTargetType=\"atom:MenuItem\">");
        source.ShouldContain("<atom:DropdownButtonItemIconStyle x:SetterTargetType=\"atom:IconPresenter\">");
        source.ShouldContain("<atom:DropdownButtonItemContentStyle x:SetterTargetType=\"ContentPresenter\">");
        source.ShouldContain("<atom:MenuItemGroup");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticPreviewGroupTitle");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticPreviewFirstMenuItem");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticPreviewSecondMenuItem");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticPreviewSubMenu");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticPreviewItem1");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticPreviewOption1");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticPreviewOption2");
        source.ShouldContain("Kind=SaveOutlined");
        source.ShouldContain("Kind=EditOutlined");
        source.ShouldContain("Kind=DeleteOutlined");
        source.ShouldContain("IsSubMenuOpen=\"True\"");
        source.ShouldContain("DropdownButtonShowCaseLangResource SemanticItemTitleDescription");
        source.ShouldContain("Path=\"itemTitle\"");
        source.IndexOf("Path=\"popup.root\"", StringComparison.Ordinal)
            .ShouldBeLessThan(source.IndexOf("Path=\"itemTitle\"", StringComparison.Ordinal));
        source.IndexOf("Path=\"itemTitle\"", StringComparison.Ordinal)
            .ShouldBeLessThan(source.IndexOf("Path=\"item\"", StringComparison.Ordinal));
        source.IndexOf("Path=\"item\"", StringComparison.Ordinal)
            .ShouldBeLessThan(source.IndexOf("Path=\"itemContent\"", StringComparison.Ordinal));
        source.IndexOf("Path=\"itemContent\"", StringComparison.Ordinal)
            .ShouldBeLessThan(source.IndexOf("Path=\"itemIcon\"", StringComparison.Ordinal));
        // 弹层根注册处理器（AdditionalRoots）是跨视觉根弹层语义预览的仓库既定模式
        // （同 InfoFlyout）：仅允许挂在 SemanticPartPreview 上，其余元素禁止 Loaded/Unloaded。
        source.ShouldContain("Loaded=\"HandleSemanticPreviewLoaded\"");
        source.ShouldContain("Unloaded=\"HandleSemanticPreviewUnloaded\"");
        CountOccurrences(source, "Loaded=\"").ShouldBe(1);
        CountOccurrences(source, "Unloaded=\"").ShouldBe(1);
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void DropdownButton_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/DropdownButton/Views/DropdownButtonShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/DropdownButtonShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractDropdownButtonExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    [Fact]
    public void DropdownButton_Button_Type_And_Size_Demos_Wrap_When_Content_Is_Wide()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/DropdownButton/Views/DropdownButtonShowCase.axaml");

        CountOccurrences(source, "<WrapPanel Orientation=\"Horizontal\" ItemSpacing=\"10\" LineSpacing=\"10\">").ShouldBe(2);
        CountOccurrences(source, "<StackPanel Orientation=\"Horizontal\" Spacing=\"10\">").ShouldBe(0);
    }

    private static string ExtractDropdownButtonExampleItems(string source)
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
