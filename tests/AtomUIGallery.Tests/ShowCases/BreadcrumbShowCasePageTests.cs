using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUIGallery.ShowCases.Breadcrumb;
using AtomUIGallery.Tests.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Shouldly;
using Xunit;
using AtomUIBreadcrumb = AtomUI.Desktop.Controls.Breadcrumb;
using AtomUIBreadcrumbItem = AtomUI.Desktop.Controls.BreadcrumbItem;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUIGallery.Tests.ShowCases;

public class BreadcrumbShowCasePageTests
{
    [Fact]
    public void Breadcrumb_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Views/BreadcrumbShowCase.axaml");

        source.ShouldContain("BreadcrumbShowCaseLangResource PageSubtitle");
        source.ShouldContain("BreadcrumbShowCaseLangResource PageDescription");
        source.ShouldNotContain("BreadcrumbShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("BreadcrumbShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("BreadcrumbShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("BreadcrumbShowCaseLangResource ComponentCategory");
        source.ShouldContain("BreadcrumbShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("BreadcrumbShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("BreadcrumbShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("BreadcrumbShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:BreadcrumbShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("BreadcrumbShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("BreadcrumbShowCaseLangResource WithIconTitle");
        source.ShouldContain("BreadcrumbShowCaseLangResource WithParamsTitle");
        source.ShouldContain("BreadcrumbShowCaseLangResource ConfiguringSeparatorTitle");
        source.ShouldContain("BreadcrumbShowCaseLangResource ConfiguringSeparatorIndependentlyTitle");
        source.ShouldContain("BreadcrumbShowCaseLangResource GenerateByTemplateTitle");
        source.ShouldContain("BreadcrumbShowCaseLangResource SemanticPartStyleTitle");
        CountOccurrences(source, "<gallery:ShowCaseItem\n").ShouldBe(7);
        CountOccurrences(source, "Span=\"Full\"").ShouldBe(7);
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("<gallery:SemanticPartPreview");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Breadcrumb}\"");
        source.ShouldContain("SourceKey=\"breadcrumb-semantic-part\"");
        source.ShouldContain("<atom:BreadcrumbItemStyle x:SetterTargetType=\"atom:BreadcrumbItem\">");
        source.ShouldContain("<atom:BreadcrumbSeparatorStyle x:SetterTargetType=\"ContentPresenter\">");
        source.ShouldContain("Classes=\"semantic-style-demo-object\"");
        source.ShouldContain("Classes=\"semantic-style-demo-function\"");
        source.ShouldContain("BadgeText=\"{x:Static gallery:GalleryVersionInfo.DisplayVersion}\"");
        CountOccurrences(source, "<atom:Breadcrumb Classes=").ShouldBe(2);
    }

    [Fact]
    public void Breadcrumb_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Views/BreadcrumbShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/BreadcrumbShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractBreadcrumbExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    [Fact]
    public void Breadcrumb_ShowCase_Declares_A_Deferred_Semantic_Part_Preview()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Views/BreadcrumbShowCase.axaml");

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("<gallery:SemanticPartPreview");
        source.ShouldContain("SemanticOwner=\"{Binding #BreadcrumbSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Breadcrumb}\"");
        // 预览画布默认居中内容（SemanticPartPreview.PreviewContentAlignment=Center），
        // 面包屑宿主自身再以对齐属性显式居中。
        source.ShouldContain("HorizontalAlignment=\"Center\"");
        source.ShouldContain("VerticalAlignment=\"Center\"");
        source.ShouldContain("Kind=HomeOutlined");
        source.ShouldContain("Kind=UserOutlined");
        source.ShouldContain("SourceKey=\"breadcrumb-semantic-part\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(3);
        source.ShouldContain("Path=\"root\"");
        source.ShouldContain("Path=\"item\"");
        source.ShouldContain("Path=\"separator\"");
    }

    [Fact]
    public void Breadcrumb_Semantic_Preview_Is_Materialized_Only_After_The_Tab_Is_Selected()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new BreadcrumbShowCase
        {
            DataContext = new BreadcrumbViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 800, () =>
        {
            page.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
            page.GetVisualDescendants()
                .OfType<AtomUIBreadcrumb>()
                .ShouldNotContain(static breadcrumb => breadcrumb.Name == "BreadcrumbSemanticOwner");

            var host = page.GetVisualDescendants().OfType<GalleryShowCaseHost>().Single();
            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            page.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(1);
            page.GetVisualDescendants()
                .OfType<AtomUIBreadcrumb>()
                .Count(static breadcrumb => breadcrumb.Name == "BreadcrumbSemanticOwner")
                .ShouldBe(1);
        });
    }

    [Fact]
    public void Breadcrumb_Semantic_Style_Example_Applies_The_Official_Style_Values()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new BreadcrumbShowCase
        {
            DataContext = new BreadcrumbViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 900, () =>
        {
            var panel = page.GetVisualDescendants().OfType<ShowCasePanel>().Single();
            var item = panel.Children
                            .OfType<ShowCaseItem>()
                            .Single(static candidate => candidate.SourceKey == "breadcrumb-semantic-part");
            item.BadgeText.ShouldBe(GalleryVersionInfo.DisplayVersion);
            item.MaterializeDeferredContent();
            Dispatcher.UIThread.RunJobs();

            var demos = page.GetVisualDescendants()
                            .OfType<AtomUIBreadcrumb>()
                            .Where(static breadcrumb => breadcrumb.Classes.Contains("semantic-style-demo-object") ||
                                                        breadcrumb.Classes.Contains("semantic-style-demo-function"))
                            .ToArray();
            demos.Length.ShouldBe(2);

            var objectDemo = demos.Single(static breadcrumb => breadcrumb.Classes.Contains("semantic-style-demo-object"));
            AssertSolidColor(objectDemo.BorderBrush, "#F0F0F0");
            objectDemo.BorderThickness.ShouldBe(new Thickness(1));
            objectDemo.CornerRadius.ShouldBe(new CornerRadius(4));
            objectDemo.Padding.ShouldBe(new Thickness(8));

            var objectItems = objectDemo.GetVisualDescendants()
                                        .OfType<AtomUIBreadcrumbItem>()
                                        .Where(static candidate => candidate.Classes.Contains("semantic-item"))
                                        .ToArray();
            objectItems.Length.ShouldBe(2);
            // The plain item takes the object demo item color; the linked item keeps the token
            // link color on its content presenter (Ant Design `.ant-breadcrumb-item a` semantics).
            AssertContentForeground(objectItems[0], "#1890FF");
            AssertContentForeground(objectItems[1], "#72000000");

            var objectSeparators = objectDemo.GetVisualDescendants()
                                             .OfType<ContentPresenter>()
                                             .Where(static candidate => candidate.Classes.Contains("semantic-separator"))
                                             .ToArray();
            objectSeparators.Length.ShouldBe(1);
            foreach (var separator in objectSeparators)
            {
                AssertSolidColor(separator.Foreground, "#73000000");
            }

            var functionDemo = demos.Single(static breadcrumb => breadcrumb.Classes.Contains("semantic-style-demo-function"));
            AssertSolidColor(functionDemo.BorderBrush, "#F5EFFF");
            functionDemo.BorderThickness.ShouldBe(new Thickness(1));
            functionDemo.CornerRadius.ShouldBe(new CornerRadius(4));
            functionDemo.Padding.ShouldBe(new Thickness(8));

            var functionItems = functionDemo.GetVisualDescendants()
                                            .OfType<AtomUIBreadcrumbItem>()
                                            .Where(static candidate => candidate.Classes.Contains("semantic-item"))
                                            .ToArray();
            functionItems.Length.ShouldBe(3);
            AssertContentForeground(functionItems[0], "#8F87F1");
            AssertContentForeground(functionItems[1], "#72000000");
            AssertContentForeground(functionItems[2], "#8F87F1");

            var functionSeparators = functionDemo.GetVisualDescendants()
                                                 .OfType<ContentPresenter>()
                                                 .Where(static candidate => candidate.Classes.Contains("semantic-separator"))
                                                 .ToArray();
            functionSeparators.Length.ShouldBe(2);
            foreach (var separator in functionSeparators)
            {
                AssertSolidColor(separator.Foreground, "#73000000");
            }
        });
    }

    [Fact]
    public void Semantic_Part_Localization_Uses_Approved_Copy()
    {
        var zhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Localization/zh-CN.xlf");
        zhCn["SemanticPartRootDescription"].ShouldBe("根元素，包含文字颜色、字体大小、图标尺寸等基础样式，内部使用 flex 布局的有序列表");
        zhCn["SemanticPartItemDescription"].ShouldBe("Item 元素，包含文字颜色、链接的颜色变化、悬浮效果、内边距、圆角、高度、外边距等样式");
        zhCn["SemanticPartSeparatorDescription"].ShouldBe("分隔符元素，包含分隔符的外边距和颜色样式");
        zhCn["SemanticPartStyleTitle"].ShouldBe("自定义语义结构的样式");
        zhCn["SemanticPartStyleDescription"].ShouldBe(
            "对齐 Ant Design 的 style-class demo：通过生成的 Semantic Part 样式定制根边框、条目颜色与分隔符颜色。");
        zhCn["P2StyleItemAntDesign"].ShouldBe("Ant Design");
        zhCn["P2StyleItemBreadcrumb"].ShouldBe("面包屑");
        zhCn["P2StyleItemComponent"].ShouldBe("组件");

        var zhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Localization/zh-TW.xlf");
        zhTw["SemanticPartRootDescription"].ShouldBe("根元素，包含文字顏色、字體大小、圖示尺寸等基礎樣式，內部使用 flex 佈局的有序列表");
        zhTw["SemanticPartItemDescription"].ShouldBe("Item 元素，包含文字顏色、連結的顏色變化、懸浮效果、內邊距、圓角、高度、外邊距等樣式");
        zhTw["SemanticPartSeparatorDescription"].ShouldBe("分隔符元素，包含分隔符的外邊距和顏色樣式");
        zhTw["SemanticPartStyleTitle"].ShouldBe("自訂語義結構的樣式");
        zhTw["SemanticPartStyleDescription"].ShouldBe(
            "對齊 Ant Design 的 style-class demo：透過生成的 Semantic Part 樣式自訂根邊框、項目顏色與分隔符顏色。");
        zhTw["P2StyleItemAntDesign"].ShouldBe("Ant Design");
        zhTw["P2StyleItemBreadcrumb"].ShouldBe("麵包屑");
        zhTw["P2StyleItemComponent"].ShouldBe("元件");

        var enUs = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Localization/en-US.xlf");
        enUs["SemanticPartRootDescription"].ShouldBe(
            "Root element with text color, font size, icon size and other basic styles, using flex layout with ordered list");
        enUs["SemanticPartItemDescription"].ShouldBe(
            "Item element with text color, link color transitions, hover effects, padding, border-radius, height, and margin styles");
        enUs["SemanticPartSeparatorDescription"].ShouldBe("Separator element with margin and color styles for the divider");
        enUs["SemanticPartStyleTitle"].ShouldBe("Custom Semantic Part styling");
        enUs["SemanticPartStyleDescription"].ShouldBe(
            "Mirrors the Ant Design style-class demo: customize the root border, item color and separator color through generated Semantic Part styles.");
        enUs["P2StyleItemAntDesign"].ShouldBe("Ant Design");
        enUs["P2StyleItemBreadcrumb"].ShouldBe("Breadcrumb");
        enUs["P2StyleItemComponent"].ShouldBe("Component");

        var ptBr = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb/Localization/pt-BR.xlf");
        ptBr["SemanticPartRootDescription"].ShouldBe(
            "Elemento raiz com cor de texto, tamanho da fonte, tamanho do ícone e outros estilos básicos, usando layout flex com lista ordenada");
        ptBr["SemanticPartItemDescription"].ShouldBe(
            "Elemento de item com cor de texto, transições de cor do link, efeitos de hover, preenchimento, raio da borda, altura e estilos de margem");
        ptBr["SemanticPartSeparatorDescription"].ShouldBe("Elemento separador com estilos de margem e cor para o divisor");
        ptBr["SemanticPartStyleTitle"].ShouldBe("Estilo personalizado de Semantic Part");
        ptBr["SemanticPartStyleDescription"].ShouldBe(
            "Espelha o demo style-class do Ant Design: personalize a borda da raiz, a cor do item e a cor do separador por meio dos estilos Semantic Part gerados.");
        ptBr["P2StyleItemAntDesign"].ShouldBe("Ant Design");
        ptBr["P2StyleItemBreadcrumb"].ShouldBe("Breadcrumb");
        ptBr["P2StyleItemComponent"].ShouldBe("Componente");
    }

    private static void AssertSolidColor(IBrush? actual, string expected)
    {
        actual.ShouldNotBeNull()
              .ShouldBeAssignableTo<ISolidColorBrush>()
              .Color.ShouldBe(Color.Parse(expected));
    }

    private static void AssertContentForeground(AtomUIBreadcrumbItem item, string expected)
    {
        var content = item.GetVisualDescendants()
                          .OfType<ContentPresenter>()
                          .Single(static presenter => presenter.Name == "Content");
        AssertSolidColor(content.Foreground, expected);
    }

    private static string ExtractBreadcrumbExampleItems(string source)
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

    private static void ShowInWindow(Control content, int width, int height, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = width,
            Height  = height,
            Content = content
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
        }
    }

    private sealed class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }
}
