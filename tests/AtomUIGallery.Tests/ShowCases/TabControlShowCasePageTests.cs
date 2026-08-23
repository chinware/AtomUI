using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUIGallery.ShowCases.TabControl;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AtomUICardTabControl = AtomUI.Desktop.Controls.CardTabControl;
using AtomUITabControl = AtomUI.Desktop.Controls.TabControl;
using AtomUITabItem = AtomUI.Desktop.Controls.TabItem;

namespace AtomUIGallery.Tests.ShowCases;

public class TabControlShowCasePageTests
{
    [Fact]
    public void TabControl_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Views/TabControlShowCase.axaml");

        source.ShouldContain("TabControlShowCaseLangResource PageSubtitle");
        source.ShouldContain("TabControlShowCaseLangResource PageDescription");
        source.ShouldNotContain("TabControlShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TabControlShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TabControlShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TabControlShowCaseLangResource ComponentCategory");
        source.ShouldContain("TabControlShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TabControlShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TabControlShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TabControlShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryShowCaseHost StickyContentPadding=\"28,0,28,0\">");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldContain("Name=\"TabControlSemanticPreview\"");
        source.ShouldContain("Name=\"CardTabControlSemanticPreview\"");
        source.ShouldContain("Name=\"TabItemSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #TabControlSemanticOwner}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #CardTabControlSemanticOwner}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #TabItemSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:TabControl}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:CardTabControl}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:TabItem}\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:TabControlShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(16);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(16);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(16);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:TabControlViewModel\"").ShouldBe(17);
        CountOccurrences(source, "BadgeText=\"v6.0.8\"").ShouldBe(2);
        source.ShouldContain("TabControlShowCaseLangResource TabControlBasicTitle");
        source.ShouldContain("TabControlShowCaseLangResource TabControlItemsSourceTitle");
        source.ShouldContain("TabControlShowCaseLangResource TabControlReorderTitle");
        source.ShouldContain("TabControlShowCaseLangResource TabControlReorderPlacementTitle");
        source.ShouldContain("TabControlShowCaseLangResource TabControlAddCloseTitle");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void TabControl_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Views/TabControlShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TabControlShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractTabControlExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    [Fact]
    public void TabControl_Semantic_Previews_Are_Materialized_On_Tab_Selection()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new TabControlShowCase
        {
            DataContext = new TabControlViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 900, () =>
        {
            page.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();

            var host = page.GetVisualDescendants().OfType<GalleryShowCaseHost>().Single();
            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            page.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(3);

            var tabControlPreview = page.GetVisualDescendants()
                                        .OfType<SemanticPartPreview>()
                                        .Single(static candidate => candidate.Name == "TabControlSemanticPreview");
            var semanticTabControl = tabControlPreview.SemanticOwner.ShouldBeOfType<AtomUITabControl>();
            semanticTabControl.Name.ShouldBe("TabControlSemanticOwner");
            semanticTabControl.GetVisualDescendants()
                              .Count(static item => item is AtomUITabItem && item.Classes.Contains("semantic-item"))
                              .ShouldBe(3);
            semanticTabControl.GetVisualDescendants()
                              .Count(static item => item is ContentPresenter && item.Classes.Contains("semantic-content"))
                              .ShouldBe(1);

            var cardPreview = page.GetVisualDescendants()
                                  .OfType<SemanticPartPreview>()
                                  .Single(static candidate => candidate.Name == "CardTabControlSemanticPreview");
            var semanticCardTabControl = cardPreview.SemanticOwner.ShouldBeOfType<AtomUICardTabControl>();
            semanticCardTabControl.Name.ShouldBe("CardTabControlSemanticOwner");
            semanticCardTabControl.GetVisualDescendants()
                                  .Count(static item => item is AtomUITabItem && item.Classes.Contains("semantic-item"))
                                  .ShouldBe(3);
            semanticCardTabControl.GetVisualDescendants()
                                  .Count(static item => item.Classes.Contains("semantic-add"))
                                  .ShouldBe(1);
            semanticCardTabControl.GetVisualDescendants()
                                  .Count(static item => item is ContentPresenter && item.Classes.Contains("semantic-content"))
                                  .ShouldBe(1);

            var itemPreview = page.GetVisualDescendants()
                                  .OfType<SemanticPartPreview>()
                                  .Single(static candidate => candidate.Name == "TabItemSemanticPreview");
            var semanticTabItem = itemPreview.SemanticOwner.ShouldBeOfType<AtomUITabItem>();
            semanticTabItem.Name.ShouldBe("TabItemSemanticOwner");
            semanticTabItem.GetVisualDescendants()
                           .Count(static item => item.Classes.Contains("semantic-icon"))
                           .ShouldBe(1);
            semanticTabItem.GetVisualDescendants()
                           .Count(static item => item.Classes.Contains("semantic-label"))
                           .ShouldBe(1);
            semanticTabItem.GetVisualDescendants()
                           .Count(static item => item.Classes.Contains("semantic-close"))
                           .ShouldBe(1);

            host.SelectedTab = GalleryShowCaseTab.Examples;
            Dispatcher.UIThread.RunJobs();
            page.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
        });
    }

    [Fact]
    public void TabControl_ShowCase_Declares_The_Semantic_Previews_And_Style_Example()
    {
        var source = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Views/TabControlShowCase.axaml");
        var english = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Localization/en-US.xlf");
        var semanticSource = ExtractShowCaseItemBySourceKey(source, "tabcontrol-semantic-part");

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"TabControlSemanticPreview\"");
        source.ShouldContain("Name=\"CardTabControlSemanticPreview\"");
        source.ShouldContain("Name=\"TabItemSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #TabControlSemanticOwner}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #CardTabControlSemanticOwner}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #TabItemSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:TabControl}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:CardTabControl}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:TabItem}\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(11);
        foreach (var path in new[] { "root", "add", "content", "item", "close", "icon", "label" })
        {
            source.ShouldContain($"Path=\"{path}\"");
        }

        semanticSource.ShouldContain("SourceKey=\"tabcontrol-semantic-part\"");
        semanticSource.ShouldContain("BadgeText=\"{x:Static gallery:GalleryVersionInfo.DisplayVersion}\"");
        semanticSource.ShouldContain("TabControlShowCaseLangResource SemanticPartStyleTitle");
        semanticSource.ShouldContain("TabControlShowCaseLangResource SemanticPartStyleDescription");
        semanticSource.ShouldContain("Selector=\"atom|TabControl.semantic-style-demo\"");
        semanticSource.ShouldContain("Selector=\"atom|TabItem.semantic-style-demo\"");
        semanticSource.ShouldContain("Selector=\"atom|CardTabControl.semantic-style-demo\"");
        semanticSource.ShouldContain("Selector=\"^:selected\"");
        semanticSource.ShouldContain("Property=\"Padding\"");
        semanticSource.ShouldContain("Value=\"16\"");
        semanticSource.ShouldContain("Property=\"Background\"");
        semanticSource.ShouldContain("Value=\"#0F1677FF\"");
        semanticSource.ShouldContain("Value=\"#1A1677FF\"");
        semanticSource.ShouldContain("Property=\"Margin\"");
        semanticSource.ShouldContain("Value=\"0,0,4,0\"");
        semanticSource.ShouldContain("Property=\"FontWeight\"");
        semanticSource.ShouldContain("Value=\"SemiBold\"");
        semanticSource.ShouldContain("Property=\"CornerRadius\"");
        semanticSource.ShouldContain("Value=\"4\"");
        CountOccurrences(semanticSource, "<atom:TabControlContentStyle ").ShouldBe(1);
        CountOccurrences(semanticSource, "<atom:TabControlItemStyle ").ShouldBe(1);
        CountOccurrences(semanticSource, "<atom:TabItemIconStyle ").ShouldBe(1);
        CountOccurrences(semanticSource, "<atom:TabItemLabelStyle ").ShouldBe(1);
        CountOccurrences(semanticSource, "<atom:CardTabControlAddStyle ").ShouldBe(1);
        CountOccurrences(semanticSource, "x:SetterTargetType=\"atom:TabItem\"").ShouldBe(1);
        CountOccurrences(semanticSource, "x:SetterTargetType=\"ContentPresenter\"").ShouldBe(2);
        CountOccurrences(semanticSource, "x:SetterTargetType=\"atom:IconPresenter\"").ShouldBe(1);
        CountOccurrences(semanticSource, "x:SetterTargetType=\"atom:IconButton\"").ShouldBe(1);
        CountOccurrences(semanticSource, "<atom:TabControl ").ShouldBe(1);
        CountOccurrences(semanticSource, "<atom:CardTabControl ").ShouldBe(1);
        semanticSource.ShouldNotContain("/template/");
        semanticSource.ShouldNotContain("semantic dom", Case.Insensitive);
        english.ShouldContain("Custom Semantic Part styling");
        english.ShouldNotContain("semantic dom", Case.Insensitive);
    }

    [Fact]
    public void TabControl_Semantic_Style_Example_Applies_The_Declared_Values()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new TabControlShowCase
        {
            DataContext = new TabControlViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 900, () =>
        {
            var panel = page.GetVisualDescendants().OfType<ShowCasePanel>().Single();
            var item = panel.Children
                            .OfType<ShowCaseItem>()
                            .Single(static candidate => candidate.SourceKey == "tabcontrol-semantic-part");
            item.BadgeText.ShouldBe(GalleryVersionInfo.DisplayVersion);
            item.MaterializeDeferredContent();
            Dispatcher.UIThread.RunJobs();

            var demoControls = page.GetVisualDescendants()
                                   .Where(static control =>
                                       (control is AtomUITabControl or AtomUICardTabControl) &&
                                       control.Classes.Contains("semantic-style-demo"))
                                   .ToArray();
            demoControls.Length.ShouldBe(2);

            var lineDemo = demoControls.OfType<AtomUITabControl>().Single();
            var cardDemo = demoControls.OfType<AtomUICardTabControl>().Single();
            lineDemo.GetVisualParent().ShouldBeSameAs(cardDemo.GetVisualParent());

            var contentPresenter = lineDemo.GetVisualDescendants()
                                           .OfType<ContentPresenter>()
                                           .Single(static candidate => candidate.Classes.Contains("semantic-content"));
            contentPresenter.Padding.ShouldBe(new Thickness(16));
            AssertSolidColor(contentPresenter.Background, "#0F1677FF");

            var lineItems = lineDemo.GetVisualDescendants()
                                    .OfType<AtomUITabItem>()
                                    .Where(static item => item.Classes.Contains("semantic-item"))
                                    .ToArray();
            lineItems.Length.ShouldBe(3);
            var lineSelected = lineItems.Single(static item => item is ISelectable { IsSelected: true });
            AssertSolidColor(lineSelected.Background, "#1A1677FF");
            foreach (var lineItem in lineItems.Where(static item => item is not ISelectable { IsSelected: true }))
            {
                lineItem.Margin.ShouldBe(new Thickness(0, 0, 4, 0));
            }

            var cardItems = cardDemo.GetVisualDescendants()
                                    .OfType<AtomUITabItem>()
                                    .Where(static item => item.Classes.Contains("semantic-item"))
                                    .ToArray();
            cardItems.Length.ShouldBe(2);
            cardItems[0].GetVisualDescendants()
                        .Count(static candidate => candidate.Classes.Contains("semantic-icon"))
                        .ShouldBe(1);
            cardItems[0].GetVisualDescendants()
                        .OfType<ContentPresenter>()
                        .Single(static candidate => candidate.Classes.Contains("semantic-label"))
                        .FontWeight.ShouldBe(FontWeight.SemiBold);

            var addButton = cardDemo.GetVisualDescendants()
                                    .OfType<IconButton>()
                                    .Single(static candidate => candidate.Classes.Contains("semantic-add"));
            addButton.CornerRadius.ShouldBe(new CornerRadius(4));
        });
    }

    [Fact]
    public void Semantic_Part_Localization_Uses_Approved_Copy()
    {
        var zhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Localization/zh-CN.xlf");
        zhCn["SemanticRootDescription"].ShouldBe("根元素，设置整体尺寸、内边距、边框与背景样式");
        zhCn["SemanticContentDescription"].ShouldBe("内容元素，设置内容面板的内边距、背景与对齐样式");
        zhCn["SemanticItemDescription"].ShouldBe("页签元素，设置页签的尺寸、内边距、悬停态与选中态样式");
        zhCn["SemanticAddDescription"].ShouldBe("新增元素，设置加号按钮的尺寸、颜色与圆角样式");
        zhCn["SemanticIconDescription"].ShouldBe("图标元素，设置页签图标的尺寸与间距样式");
        zhCn["SemanticLabelDescription"].ShouldBe("标题元素，设置页签标题的字体与颜色样式");
        zhCn["SemanticCloseDescription"].ShouldBe("关闭元素，设置关闭按钮的尺寸与颜色样式");
        zhCn["SemanticPartStyleTitle"].ShouldBe("自定义 Semantic Part 样式");

        var zhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Localization/zh-TW.xlf");
        zhTw["SemanticRootDescription"].ShouldBe("根元素，設定整體尺寸、內邊距、邊框與背景樣式");
        zhTw["SemanticContentDescription"].ShouldBe("內容元素，設定內容面板的內邊距、背景與對齊樣式");
        zhTw["SemanticItemDescription"].ShouldBe("頁籤元素，設定頁籤的尺寸、內邊距、懸停態與選中態樣式");
        zhTw["SemanticAddDescription"].ShouldBe("新增元素，設定加號按鈕的尺寸、顏色與圓角樣式");
        zhTw["SemanticIconDescription"].ShouldBe("圖示元素，設定頁籤圖示的尺寸與間距樣式");
        zhTw["SemanticLabelDescription"].ShouldBe("標題元素，設定頁籤標題的字型與顏色樣式");
        zhTw["SemanticCloseDescription"].ShouldBe("關閉元素，設定關閉按鈕的尺寸與顏色樣式");
        zhTw["SemanticPartStyleTitle"].ShouldBe("自定義 Semantic Part 樣式");

        var enUs = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Localization/en-US.xlf");
        enUs["SemanticRootDescription"].ShouldBe("Root element, set overall size, padding, border and background styles");
        enUs["SemanticContentDescription"].ShouldBe(
            "Content element, set padding, background and alignment styles for the content panel");
        enUs["SemanticItemDescription"].ShouldBe(
            "Item element, set size, padding, hover state and selected state styles for each tab");
        enUs["SemanticAddDescription"].ShouldBe(
            "Add element, set size, color and corner radius styles for the plus button");
        enUs["SemanticIconDescription"].ShouldBe("Icon element, set size and margin styles for the tab icon");
        enUs["SemanticLabelDescription"].ShouldBe("Label element, set font and color styles for the tab title");
        enUs["SemanticCloseDescription"].ShouldBe("Close element, set size and color styles for the close button");
        enUs["SemanticPartStyleTitle"].ShouldBe("Custom Semantic Part styling");

        var ptBr = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl/Localization/pt-BR.xlf");
        ptBr["SemanticRootDescription"].ShouldBe(
            "Elemento raiz, define tamanho geral, preenchimento, borda e estilos de fundo");
        ptBr["SemanticContentDescription"].ShouldBe(
            "Elemento de conteúdo, define preenchimento, fundo e estilos de alinhamento para o painel de conteúdo");
        ptBr["SemanticItemDescription"].ShouldBe(
            "Elemento de item, define tamanho, preenchimento, estado de foco e estado selecionado para cada aba");
        ptBr["SemanticAddDescription"].ShouldBe(
            "Elemento de adição, define tamanho, cor e estilos de raio de canto para o botão de mais");
        ptBr["SemanticIconDescription"].ShouldBe(
            "Elemento de ícone, define tamanho e estilos de margem para o ícone da aba");
        ptBr["SemanticLabelDescription"].ShouldBe(
            "Elemento de rótulo, define estilos de fonte e cor para o título da aba");
        ptBr["SemanticCloseDescription"].ShouldBe(
            "Elemento de fechamento, define tamanho e estilos de cor para o botão de fechar");
        ptBr["SemanticPartStyleTitle"].ShouldBe("Personalizar estilo de Semantic Part");
    }

    private static void AssertSolidColor(IBrush? actual, string expected)
    {
        actual.ShouldNotBeNull()
              .ShouldBeAssignableTo<ISolidColorBrush>()
              .Color.ShouldBe(Color.Parse(expected));
    }

    private static string ExtractTabControlExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string ExtractShowCaseItemBySourceKey(string source, string sourceKey)
    {
        var sourceKeyIndex = source.IndexOf($"SourceKey=\"{sourceKey}\"", StringComparison.Ordinal);
        sourceKeyIndex.ShouldBeGreaterThanOrEqualTo(0);

        var itemStart = source.LastIndexOf("<gallery:ShowCaseItem", sourceKeyIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        const string itemEndMarker = "</gallery:ShowCaseItem>";
        var itemEnd = source.IndexOf(itemEndMarker, sourceKeyIndex, StringComparison.Ordinal);
        itemEnd.ShouldBeGreaterThan(sourceKeyIndex);

        return source[itemStart..(itemEnd + itemEndMarker.Length)];
    }

    private static string NormalizeMarkup(string source)
    {
        return ShowCaseSnapshotMarkup.Normalize(StripTabControlBehaviorMarkup(source));
    }

    private static string StripTabControlBehaviorMarkup(string source)
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

    private static void ShowInWindow(Control content, double width, double height, Action assertion)
    {
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child = content
        };

        var window = new AvaloniaWindow
        {
            Content = visualLayerManager,
            Width = width,
            Height = height
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private sealed class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }
}
