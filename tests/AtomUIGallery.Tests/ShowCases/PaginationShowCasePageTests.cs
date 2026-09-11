using System.Text.RegularExpressions;
using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUIGallery.ShowCases.Pagination;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AtomUIPagination = AtomUI.Desktop.Controls.Pagination;
using AtomUISimplePagination = AtomUI.Desktop.Controls.SimplePagination;

namespace AtomUIGallery.Tests.ShowCases;

public class PaginationShowCasePageTests
{
    [Fact]
    public void Pagination_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination/Views/PaginationShowCase.axaml");

        source.ShouldContain("PaginationShowCaseLangResource PageSubtitle");
        source.ShouldContain("PaginationShowCaseLangResource PageDescription");
        source.ShouldNotContain("PaginationShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("PaginationShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("PaginationShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("PaginationShowCaseLangResource ComponentCategory");
        source.ShouldContain("PaginationShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("PaginationShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("PaginationShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("PaginationShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldContain("Name=\"PaginationSemanticPreview\"");
        source.ShouldContain("Name=\"SimplePaginationSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #PaginationSemanticOwner}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #SimplePaginationSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Pagination}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:SimplePagination}\"");
        source.ShouldContain("Path=\"root\"");
        source.ShouldContain("Path=\"item\"");
        source.ShouldContain("Path=\"info\"");
        source.ShouldContain("PaginationShowCaseLangResource SemanticRootDescription");
        source.ShouldContain("PaginationShowCaseLangResource SemanticItemDescription");
        source.ShouldContain("PaginationShowCaseLangResource SemanticInfoDescription");
        source.ShouldContain("PaginationShowCaseLangResource SemanticPartStyleTitle");
        source.ShouldContain("PaginationShowCaseLangResource SemanticPartStyleDescription");
        source.ShouldContain("SourceKey=\"pagination-semantic-part\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:PaginationShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("PaginationShowCaseLangResource BasicTitle");
        source.ShouldContain("PaginationShowCaseLangResource BindingTitle");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("PaginationShowCaseLangResource AlignTitle");
        source.ShouldContain("PaginationShowCaseLangResource MoreTitle");
        source.ShouldContain("PaginationShowCaseLangResource MiniSizeTitle");
        source.ShouldContain("PaginationShowCaseLangResource TotalNumberTitle");
        source.ShouldContain("PaginationShowCaseLangResource SimpleModeTitle");
        CountShowCaseItemElements(source).ShouldBe(9);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(9);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(9);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:PaginationViewModel\"").ShouldBe(10);
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Pagination_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination/Views/PaginationShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/PaginationShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractPaginationExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    [Fact]
    public void Pagination_Semantic_Previews_Are_Materialized_On_Tab_Selection()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new PaginationShowCase
        {
            DataContext = new PaginationViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 900, () =>
        {
            page.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();

            var host = page.GetVisualDescendants().OfType<GalleryShowCaseHost>().Single();
            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            page.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(2);
            var paginationPreview = page.GetVisualDescendants()
                                        .OfType<SemanticPartPreview>()
                                        .Single(static candidate => candidate.Name == "PaginationSemanticPreview");
            var semanticPagination = paginationPreview.SemanticOwner.ShouldBeOfType<AtomUIPagination>();
            semanticPagination.Name.ShouldBe("PaginationSemanticOwner");
            semanticPagination.HorizontalAlignment.ShouldBe(HorizontalAlignment.Center);
            semanticPagination.VerticalAlignment.ShouldBe(VerticalAlignment.Center);
            semanticPagination.GetVisualDescendants()
                              .Count(static item => item.Classes.Contains("semantic-item") && item.IsVisible)
                              .ShouldBe(7);

            var simplePreview = page.GetVisualDescendants()
                                    .OfType<SemanticPartPreview>()
                                    .Single(static candidate => candidate.Name == "SimplePaginationSemanticPreview");
            var semanticSimplePagination = simplePreview.SemanticOwner.ShouldBeOfType<AtomUISimplePagination>();
            semanticSimplePagination.Name.ShouldBe("SimplePaginationSemanticOwner");
            semanticSimplePagination.GetVisualDescendants()
                                    .Count(static item => item.Classes.Contains("semantic-item") && item.IsVisible)
                                    .ShouldBe(2);
            semanticSimplePagination.GetVisualDescendants()
                                    .Count(static item => item.Classes.Contains("semantic-info") && item.IsVisible)
                                    .ShouldBe(1);

            host.SelectedTab = GalleryShowCaseTab.Examples;
            Dispatcher.UIThread.RunJobs();
            Assert.All(page.GetVisualDescendants().OfType<SemanticPartPreview>(), preview => Assert.False(preview.IsEffectivelyVisible));
        });
    }

    [Fact]
    public void Pagination_ShowCase_Declares_The_Semantic_Previews_And_Style_Example()
    {
        var source = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination/Views/PaginationShowCase.axaml");
        var english = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination/Localization/en-US.xlf");
        var semanticSource = ExtractShowCaseItemBySourceKey(source, "pagination-semantic-part");

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"PaginationSemanticPreview\"");
        source.ShouldContain("Name=\"SimplePaginationSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #PaginationSemanticOwner}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #SimplePaginationSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Pagination}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:SimplePagination}\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(5);
        foreach (var path in new[] { "root", "item", "info" })
        {
            source.ShouldContain($"Path=\"{path}\"");
        }

        semanticSource.ShouldContain("SourceKey=\"pagination-semantic-part\"");
        semanticSource.ShouldContain("BadgeText=\"{x:Static gallery:GalleryVersionInfo.DisplayVersion}\"");
        semanticSource.ShouldContain("PaginationShowCaseLangResource SemanticPartStyleTitle");
        semanticSource.ShouldContain("PaginationShowCaseLangResource SemanticPartStyleDescription");
        semanticSource.ShouldContain("Selector=\"atom|Pagination.semantic-style-demo-root\"");
        semanticSource.ShouldContain("Selector=\"atom|Pagination.semantic-style-demo-object\"");
        semanticSource.ShouldContain("Selector=\"atom|Pagination[SizeType=Small]\"");
        semanticSource.ShouldContain("Property=\"BorderBrush\"");
        semanticSource.ShouldContain("Value=\"#CCCCCC\"");
        semanticSource.ShouldContain("Property=\"BorderThickness\"");
        semanticSource.ShouldContain("Property=\"BorderDashArray\"");
        semanticSource.ShouldContain("Value=\"4,2\"");
        semanticSource.ShouldContain("Property=\"Padding\"");
        semanticSource.ShouldContain("Property=\"CornerRadius\"");
        semanticSource.ShouldContain("Value=\"999\"");
        semanticSource.ShouldContain("Property=\"Background\"");
        semanticSource.ShouldContain("Value=\"#4DC8C8C8\"");
        semanticSource.ShouldContain("Property=\"Margin\"");
        semanticSource.ShouldContain("Value=\"0,0,4,0\"");
        semanticSource.ShouldContain("Selector=\"^:selected\"");
        semanticSource.ShouldContain("Value=\"{atom:PaginationTokenResource ItemActiveBg}\"");
        CountOccurrences(semanticSource, "<atom:PaginationItemStyle ").ShouldBe(2);
        semanticSource.ShouldNotContain("SimplePaginationItemStyle");
        CountOccurrences(semanticSource, "x:SetterTargetType=\"ContentControl\"").ShouldBe(2);
        CountOccurrences(semanticSource, "<atom:Pagination ").ShouldBe(2);
        semanticSource.ShouldNotContain("<atom:SimplePagination ");
        CountOccurrences(semanticSource, "SizeType=\"Small\"").ShouldBe(1);
        CountOccurrences(semanticSource, "IsShowSizeChanger=\"True\"").ShouldBe(2);
        CountOccurrences(semanticSource, "Total=\"500\"").ShouldBe(2);
        semanticSource.ShouldNotContain("/template/");
        semanticSource.ShouldNotContain("semantic dom", Case.Insensitive);
        english.ShouldContain("Custom Semantic Part styling");
        english.ShouldContain("Style the root and items through generated Semantic Part styles");
        english.ShouldNotContain("Mirrors the Ant Design style-class demo");
        english.ShouldNotContain("semantic dom", Case.Insensitive);
    }

    [Fact]
    public void Pagination_Semantic_Style_Example_Applies_The_Official_Style_Values()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new PaginationShowCase
        {
            DataContext = new PaginationViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 900, () =>
        {
            var panel = page.GetVisualDescendants().OfType<ShowCasePanel>().Single();
            var item = panel.Children
                            .OfType<ShowCaseItem>()
                            .Single(static candidate => candidate.SourceKey == "pagination-semantic-part");
            item.BadgeText.ShouldBe(GalleryVersionInfo.DisplayVersion);
            item.MaterializeDeferredContent();
            Dispatcher.UIThread.RunJobs();

            var demoRows = page.GetVisualDescendants()
                               .OfType<AtomUIPagination>()
                               .Where(static pagination => pagination.Classes.Contains("semantic-style-demo-root"))
                               .ToArray();
            demoRows.Length.ShouldBe(2);

            var demoContainer = demoRows[0].GetVisualParent().ShouldBeOfType<StackPanel>();
            demoRows[1].GetVisualParent().ShouldBeSameAs(demoContainer);
            foreach (var row in demoRows)
            {
                row.Bounds.Width.ShouldBe(demoContainer.Bounds.Width, 0.5);
                AssertSolidColor(row.BorderBrush, "#CCCCCC");
                row.BorderThickness.ShouldBe(new Thickness(2));
                row.BorderDashArray.ShouldBe(new[] { 4d, 2d });
                row.Padding.ShouldBe(new Thickness(8));
            }

            var objectDemo = demoRows.Single(
                static pagination => pagination.Classes.Contains("semantic-style-demo-object"));
            objectDemo.SizeType.ShouldBe(CustomizableSizeType.Middle);

            var objectItems = objectDemo.GetVisualDescendants()
                                        .OfType<ContentControl>()
                                        .Where(static control => control.Classes.Contains("semantic-item"))
                                        .ToArray();
            objectItems.Length.ShouldBe(10);
            foreach (var objectItem in objectItems)
            {
                objectItem.CornerRadius.ShouldBe(new CornerRadius(999));
            }

            var functionDemo = demoRows.Single(
                static pagination => !pagination.Classes.Contains("semantic-style-demo-object"));
            functionDemo.SizeType.ShouldBe(CustomizableSizeType.Small);

            var functionItems = functionDemo.GetVisualDescendants()
                                            .OfType<ContentControl>()
                                            .Where(static control => control.Classes.Contains("semantic-item"))
                                            .ToArray();
            functionItems.Length.ShouldBe(10);

            var objectSelected   = objectItems.Single(static item => item is ISelectable { IsSelected: true });
            var functionSelected = functionItems.Single(static item => item is ISelectable { IsSelected: true });
            functionSelected.Background.ShouldBe(objectSelected.Background);
            functionSelected.Margin.ShouldBe(new Thickness(0, 0, 4, 0));

            foreach (var functionItem in functionItems.Where(
                         static item => item is not ISelectable { IsSelected: true }))
            {
                AssertSolidColor(functionItem.Background, "#4DC8C8C8");
                functionItem.Margin.ShouldBe(new Thickness(0, 0, 4, 0));
            }
        });
    }

    [Fact]
    public void Semantic_Part_Localization_Uses_Approved_Copy()
    {
        var zhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination/Localization/zh-CN.xlf");
        zhCn["SemanticRootDescription"].ShouldBe("根元素，设置 flex 布局、对齐方式、换行和列表样式");
        zhCn["SemanticItemDescription"].ShouldBe("页码元素，设置尺寸、内边距、边框、背景色、悬停态和激活态样式");
        zhCn["SemanticInfoDescription"].ShouldBe("信息元素，设置页码信息文本的字体、颜色和对齐样式");
        zhCn["SemanticPartStyleTitle"].ShouldBe("自定义语义结构的样式");
        zhCn["SemanticPartStyleDescription"].ShouldBe(
            "通过生成的 Semantic Part 样式定制 root 与 item：root 使用虚线描边、item 使用圆角，item 背景仅在小尺寸下生效。");

        var zhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination/Localization/zh-TW.xlf");
        zhTw["SemanticRootDescription"].ShouldBe("根元素，設定 flex 佈局、對齊方式、換行和列表樣式");
        zhTw["SemanticItemDescription"].ShouldBe("頁碼元素，設定尺寸、內邊距、邊框、背景色、懸停態和啟動態樣式");
        zhTw["SemanticInfoDescription"].ShouldBe("資訊元素，設定頁碼資訊文字的字型、顏色與對齊樣式");
        zhTw["SemanticPartStyleTitle"].ShouldBe("自訂語義結構的樣式");
        zhTw["SemanticPartStyleDescription"].ShouldBe(
            "透過生成的 Semantic Part 樣式自訂 root 與 item：root 使用虛線描邊、item 使用圓角，item 背景僅在小尺寸下生效。");

        var enUs = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination/Localization/en-US.xlf");
        enUs["SemanticRootDescription"].ShouldBe("Root element, set flex layout, alignment, flex wrap and list styles");
        enUs["SemanticItemDescription"].ShouldBe(
            "Item element, set size, padding, border, background color, hover state and active state styles");
        enUs["SemanticInfoDescription"].ShouldBe(
            "Info element, set font, color and alignment styles for the page info text");
        enUs["SemanticPartStyleTitle"].ShouldBe("Custom Semantic Part styling");
        enUs["SemanticPartStyleDescription"].ShouldBe(
            "Style the root and items through generated Semantic Part styles: a dashed outline around the root, rounded item corners, and item backgrounds that apply only at small size.");

        var ptBr = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination/Localization/pt-BR.xlf");
        ptBr["SemanticRootDescription"].ShouldBe(
            "Elemento raiz, define layout flex, alinhamento, quebra de linha e estilos de lista");
        ptBr["SemanticItemDescription"].ShouldBe(
            "Elemento de item, define tamanho, preenchimento, borda, cor de fundo, estado de foco e estado ativo");
        ptBr["SemanticInfoDescription"].ShouldBe(
            "Elemento de info, define estilos de fonte, cor e alinhamento para o texto de informação da página");
        ptBr["SemanticPartStyleTitle"].ShouldBe("Estilo personalizado de Semantic Part");
        ptBr["SemanticPartStyleDescription"].ShouldBe(
            "Personalize root e item por meio dos estilos de Semantic Part gerados: contorno tracejado no root, cantos arredondados nos itens e fundos de item aplicados somente no tamanho small.");
    }

    private static void AssertSolidColor(IBrush? actual, string expected)
    {
        actual.ShouldNotBeNull()
              .ShouldBeAssignableTo<ISolidColorBrush>()
              .Color.ShouldBe(Color.Parse(expected));
    }

    private static string ExtractPaginationExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(source);
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
