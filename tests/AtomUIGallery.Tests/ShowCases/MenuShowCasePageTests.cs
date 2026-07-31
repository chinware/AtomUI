using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using AtomUIGallery.ShowCases.Menu;
using ReactiveUI;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class MenuShowCasePageTests
{
    [Fact]
    public void Menu_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml");

        source.ShouldContain("MenuShowCaseLangResource PageSubtitle");
        source.ShouldContain("MenuShowCaseLangResource PageDescription");
        source.ShouldNotContain("MenuShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("MenuShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("MenuShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("MenuShowCaseLangResource ComponentCategory");
        source.ShouldContain("MenuShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("MenuShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("MenuShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("MenuShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:MenuShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(17);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(17);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(17);
        CountOccurrences(source, "DataTemplate x:DataType=\"viewModels:MenuViewModel\"").ShouldBe(17);
        source.ShouldContain("MenuShowCaseLangResource BasicTitle");
        source.ShouldContain("MenuShowCaseLangResource IconAndSubmenuTitle");
        source.ShouldContain("MenuShowCaseLangResource MenuItemItemsSourceTitle");
        source.ShouldContain("MenuShowCaseLangResource ContextMenuTitle");
        source.ShouldContain("MenuShowCaseLangResource VerticalNavMenuTitle");
        source.ShouldContain("MenuShowCaseLangResource NavMenuNodeCommandTitle");
        source.ShouldContain("MenuShowCaseLangResource InlineCollapsedMenuTitle");
        source.ShouldContain("BadgeText=\"v6.0.6\"");
        source.ShouldContain("IsInlineCollapsed=\"{Binding IsInlineCollapsed}\"");
        source.ShouldContain("Click=\"HandleToggleInlineCollapsedClick\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Menu_ShowCase_NavMenuNode_Command_Uses_Explicit_Business_Keys_And_Displays_The_Last_Executed_Key()
    {
        var pageSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml");
        var viewModelSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/ViewModels/MenuViewModel.cs");
        var commandShowCaseSource = ExtractNavMenuNodeCommandShowCaseItem(pageSource);
        const string commandBadgePattern =
            "Title=\"\\{gallery:MenuShowCaseLangResource NavMenuNodeCommandTitle\\}\"\\s+" +
            "Description=\"\\{gallery:MenuShowCaseLangResource NavMenuNodeCommandDescription\\}\"\\s+" +
            "BadgeText=\"v6\\.1\\.0\"";

        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource NavMenuNodeCommandTitle");
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource NavMenuNodeCommandDescription");
        Regex.IsMatch(commandShowCaseSource, commandBadgePattern, RegexOptions.CultureInvariant)
            .ShouldBeTrue();
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource P2HeaderNavigationOne");
        commandShowCaseSource.ShouldContain("AntDesignIconProvider Kind=MailOutlined");
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource P2HeaderNavigationTwo");
        commandShowCaseSource.ShouldContain("AntDesignIconProvider Kind=AppstoreOutlined");
        commandShowCaseSource.ShouldContain("IsEnabled=\"False\"");
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource P2HeaderNavigationThreeSubmenu");
        commandShowCaseSource.ShouldContain("AntDesignIconProvider Kind=SettingOutlined");
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource P2HeaderItemN1");
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource P2HeaderItemN2");
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource P2HeaderOptionN1");
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource P2HeaderOptionN4");
        commandShowCaseSource.ShouldContain("MenuShowCaseLangResource P2HeaderNavigationFour");
        CountOccurrences(commandShowCaseSource, "Command=\"{Binding NavigateCommand}\"").ShouldBe(6);
        commandShowCaseSource.ShouldContain("ItemKey=\"command-navigation-one\"");
        commandShowCaseSource.ShouldContain("CommandParameter=\"navigation-one\"");
        commandShowCaseSource.ShouldContain("ItemKey=\"command-option-1\"");
        commandShowCaseSource.ShouldContain("CommandParameter=\"option-1\"");
        commandShowCaseSource.ShouldContain("ItemKey=\"command-option-4\"");
        commandShowCaseSource.ShouldContain("CommandParameter=\"option-4\"");
        commandShowCaseSource.ShouldContain("ItemKey=\"command-navigation-four\"");
        commandShowCaseSource.ShouldContain("CommandParameter=\"navigation-four\"");
        commandShowCaseSource.ShouldNotContain("CommandParameter=\"{Binding ItemKey}\"");
        commandShowCaseSource.ShouldContain("Text=\"{Binding LastCommandKey}\"");

        viewModelSource.ShouldContain("public ReactiveCommand<string, Unit> NavigateCommand { get; }");
        viewModelSource.ShouldContain("public string LastCommandKey");
        viewModelSource.ShouldContain("LastCommandKey = itemKey;");
    }

    [Fact]
    public void Menu_ViewModel_Command_Records_The_Explicit_Business_Key()
    {
        var viewModel = new MenuViewModel(new TestScreen());

        ((ICommand)viewModel.NavigateCommand).Execute("customer-overview");

        viewModel.LastCommandKey.ShouldBe("customer-overview");
    }

    [Fact]
    public void Menu_ShowCase_Scrollable_Menu_Can_Toggle_Popup_Scrolling()
    {
        var pageSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml");
        var viewModelSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/ViewModels/MenuViewModel.cs");
        var zhCN            = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Localization/zh_CN.cs");
        var zhTW            = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Localization/zh_TW.cs");
        var enUS            = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Localization/en_US.cs");
        var scrollableShowCaseSource = ExtractShowCaseItem(pageSource, "ScrollableTitle");

        scrollableShowCaseSource.ShouldContain("MenuShowCaseLangResource ScrollableTitle");
        scrollableShowCaseSource.ShouldContain("MenuShowCaseLangResource P2TextEnablePopupScroll");
        scrollableShowCaseSource.ShouldContain("IsChecked=\"{Binding IsPopupScrollEnabled}\"");
        scrollableShowCaseSource.ShouldContain("IsScrollEnabled=\"{Binding IsPopupScrollEnabled}\"");
        scrollableShowCaseSource.ShouldNotContain("IsCheckedChanged=\"");
        CountOccurrences(scrollableShowCaseSource, "Header=\"{gallery:MenuShowCaseLangResource P2HeaderMenuItem}\"")
            .ShouldBe(12);

        viewModelSource.ShouldContain("private bool _isPopupScrollEnabled = true;");
        viewModelSource.ShouldContain("public bool IsPopupScrollEnabled");

        zhCN.ShouldContain("public const string P2TextEnablePopupScroll = \"开启弹层滚动\";");
        zhTW.ShouldContain("public const string P2TextEnablePopupScroll = \"開啟彈層滾動\";");
        enUS.ShouldContain("public const string P2TextEnablePopupScroll = \"Enable popup scrolling\";");
    }

    [Fact]
    public void Menu_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/MenuShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractMenuExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractMenuExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(StripNavMenuNodeCommandShowCaseItem(StripMenuRuntimeBindingMarkup(source)));
    }

    private static string StripNavMenuNodeCommandShowCaseItem(string source)
    {
        return Regex.Replace(
            source,
            @"\s*<gallery:ShowCaseItem\s+Title=""\{gallery:MenuShowCaseLangResource NavMenuNodeCommandTitle\}"".*?</gallery:ShowCaseItem>",
            string.Empty,
            RegexOptions.CultureInvariant | RegexOptions.Singleline);
    }

    private static string ExtractNavMenuNodeCommandShowCaseItem(string source)
    {
        return ExtractShowCaseItem(source, "NavMenuNodeCommandTitle");
    }

    private static string ExtractShowCaseItem(string source, string titleResourceName)
    {
        var match = Regex.Match(
            source,
            $@"<gallery:ShowCaseItem\s+Title=""\{{gallery:MenuShowCaseLangResource {titleResourceName}\}}"".*?</gallery:ShowCaseItem>",
            RegexOptions.CultureInvariant | RegexOptions.Singleline);

        match.Success.ShouldBeTrue();
        return match.Value;
    }

    private static string StripMenuRuntimeBindingMarkup(string source)
    {
        var normalized = Regex.Replace(
            source,
            @"\s+ItemsSource=""\{Binding (MenuItems|InlineNavMenuNodes|ItemsSourceDemoNavMenuNodes|ContextMenuItems)\}""",
            string.Empty,
            RegexOptions.CultureInvariant);

        return Regex.Replace(
            normalized,
            @"\s+(IsCheckedChanged=""HandleChange(Mode|Style)CheckChanged""|Click=""HandleToggleInlineCollapsedClick"")",
            string.Empty,
            RegexOptions.CultureInvariant);
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

    private sealed class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }
}
