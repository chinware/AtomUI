using System.Reflection;
using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using AtomUIGallery.ShowCases.Button;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Shouldly;
using Xunit;
using ButtonType = AtomUI.Desktop.Controls.ButtonType;
using AtomRibbonBadge = AtomUI.Desktop.Controls.RibbonBadge;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUIGallery.Tests.ShowCases;

public class ButtonShowCasePageTests
{
    [Fact]
    public void Button_ShowCase_Uses_Document_Layout_With_Grouped_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");

        source.ShouldContain("ButtonShowCaseLangResource PageSubtitle");
        source.ShouldContain("ButtonShowCaseLangResource PageDescription");
        source.ShouldNotContain("ButtonShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("ButtonShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("ButtonShowCaseLangResource InfoBaseClassLabel");
        source.ShouldNotContain("MinHeight=\"48\"");
        source.ShouldNotContain("ItemSpacing=\"48\"");
        source.ShouldNotContain("LineSpacing=\"8\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("<Setter Property=\"Width\" Value=\"84\" />");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        source.ShouldNotContain("<Setter Property=\"Width\" Value=\"200\" />");
        source.ShouldNotContain("<Setter Property=\"TextWrapping\" Value=\"NoWrap\" />");
        source.ShouldNotContain("<Setter Property=\"TextTrimming\" Value=\"CharacterEllipsis\" />");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("Classes=\"info-pair\"");
        source.ShouldNotContain("Classes=\"info-value-frame\"");
        source.ShouldNotContain("Width=\"300\"");
        source.ShouldNotContain("ColumnDefinitions=\"Auto,*\"");
        source.ShouldNotContain("<Setter Property=\"MaxWidth\" Value=\"220\" />");
        source.ShouldNotContain("<Setter Property=\"Width\" Value=\"104\" />");
        source.ShouldNotContain("<Setter Property=\"Width\" Value=\"128\" />");
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldNotContain("ColumnDefinitions=\"Auto,*,Auto,*,Auto,*\"");
        source.ShouldNotContain("ButtonShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("ButtonShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("ButtonShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("ContentPadding=\"0,10,0,0\"");
        source.ShouldNotContain("Margin=\"28,0,12,24\"");
        source.ShouldNotContain("<atom:TabControl.Styles>");
        source.ShouldNotContain("<gallery:ShowCasePanel ContentMargin=\"0,20,16,0\">");
        source.ShouldNotContain("<gallery:ShowCasePanel ContentMargin=\"0,10,16,0\">");
        source.ShouldNotContain("<gallery:ShowCasePanel ContentMargin=\"0,0,16,0\">");
        source.ShouldNotContain("ContentPadding=\"0,10,16,0\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("ButtonShowCaseLangResource TypeTitle");
        source.ShouldContain("ButtonShowCaseLangResource ColorVariantTitle");
        source.ShouldContain("ButtonShowCaseLangResource GradientButtonTitle");
        source.ShouldContain("ButtonShowCaseLangResource ButtonShapeTitle");
        source.ShouldContain("ButtonShowCaseLangResource SizeTitle");
        source.ShouldContain("ButtonShowCaseLangResource IconPlacementTitle");
        source.ShouldContain("ButtonShowCaseLangResource LoadingTitle");
        source.ShouldNotContain("Name=\"ScenarioTabs\"");
        source.ShouldContain("Description=\"{gallery:ButtonShowCaseLangResource PageDescription}\"");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain("Classes=\"showcase-table-row\"");
        source.ShouldNotContain("showcase-table-divider");
        source.ShouldNotContain("MinWidth=\"780\"");
        source.ShouldNotContain("ColumnDefinitions=\"220,*\"");
        source.ShouldNotContain("Width=\"280\"");
        source.ShouldNotContain("UsagePrimaryActionTitle");
        source.ShouldNotContain("OverviewTitle");
        source.ShouldNotContain(">Gallery<");
        source.ShouldNotContain("ButtonShowCaseLangResource ScenarioGallery");
    }

    [Fact]
    public void Button_ShowCase_Declares_A_Deferred_Semantic_Part_Preview()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("<gallery:SemanticPartPreview");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Button}\"");
        source.ShouldContain("Name=\"SemanticPartDemoButton\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(3);
        source.ShouldContain("Path=\"root\"");
        source.ShouldContain("Path=\"icon\"");
        source.ShouldContain("Path=\"content\"");
    }

    [Fact]
    public void Button_Semantic_Preview_Is_Materialized_Only_After_The_Tab_Is_Selected()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new ButtonShowCase
        {
            DataContext = new ButtonViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 800, () =>
        {
            page.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
            page.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.Button>()
                .ShouldNotContain(static button => button.Name == "SemanticPartDemoButton");

            var host = page.GetVisualDescendants().OfType<GalleryShowCaseHost>().Single();
            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            page.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(1);
            page.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.Button>()
                .Count(static button => button.Name == "SemanticPartDemoButton")
                .ShouldBe(1);
        });
    }

    [Fact]
    public void Button_Semantic_Part_Example_Matches_Object_And_Function_Contract()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var localization =
            ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Localization/en-US.xlf");
        var examples = ExtractButtonExampleItems(source);

        var item = ExtractShowCaseItemByTitle(
            examples,
            "ButtonShowCaseLangResource SemanticPartStyleTitle");

        item.ShouldContain("SourceKey=\"button-semantic-part\"");
        item.ShouldContain("BadgeText=\"v6.1.3\"");
        item.ShouldContain("Span=\"Full\"");
        item.ShouldContain("IsDeferredContentEnabled=\"True\"");
        item.ShouldContain("<gallery:ShowCaseItem.DeferredContentTemplate>");
        item.ShouldContain("ButtonShowCaseLangResource SemanticPartStyleDescription");
        item.ShouldContain("ButtonShowCaseLangResource P2ContentSemanticObject");
        item.ShouldContain("ButtonShowCaseLangResource P2ContentSemanticFunction");
        item.ShouldContain("Selector=\"atom|Button.semantic-part-demo\"");
        item.ShouldContain(
            "Selector=\"atom|Button.semantic-part-demo /template/ .semantic-content\"");
        item.ShouldContain("Selector=\"atom|Button.semantic-part-demo.semantic-object\"");
        item.ShouldContain("Selector=\"atom|Button.semantic-part-demo[ButtonType=Primary]\"");
        item.ShouldContain(
            "Selector=\"atom|Button.semantic-part-demo[ButtonType=Primary] /template/ .semantic-content\"");
        item.ShouldContain("x:SetterTargetType=\"ContentPresenter\"");
        item.ShouldContain("<Setter Property=\"Background\" Value=\"#171717\" />");
        item.ShouldContain("Property=\"Foreground\"");
        item.ShouldContain("Value=\"{atom:SharedTokenResource ColorText}\"");
        item.ShouldContain("<Setter Property=\"Foreground\" Value=\"#FFFFFF\" />");
        CountOccurrences(item, "Classes=\"semantic-part-demo").ShouldBe(2);
        item.ShouldNotContain("Selector=\".semantic-content\"");
        localization.ShouldContain("<source>Custom Semantic Part styling</source>");
        localization.ShouldContain(
            "<source>Use owner-scoped selectors and Button state selectors to customize published Semantic Parts.</source>");
        localization.ShouldNotContain("Semantic DOM");
        localization.ShouldNotContain("semantic dom");
        localization.ShouldNotContain("classNames");
        localization.ShouldContain("<source>Object</source>");
        localization.ShouldContain("<source>Function</source>");
    }

    [Fact]
    public void Button_Semantic_Part_Example_Applies_Styles_To_Runtime_Parts()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new ButtonShowCase
        {
            DataContext = new ButtonViewModel(new TestScreen())
        };

        ShowInWindow(page, 1280, 1000, _ =>
        {
            var panel = page.GetVisualDescendants().OfType<ShowCasePanel>().Single();
            var item = panel.Children
                            .OfType<ShowCaseItem>()
                            .Single(static candidate => candidate.SourceKey == "button-semantic-part");
            item.MaterializeDeferredContent();
            Dispatcher.UIThread.RunJobs();

            var buttons = page.GetVisualDescendants()
                              .OfType<AtomUI.Desktop.Controls.Button>()
                              .Where(static button => button.Classes.Contains("semantic-part-demo"))
                              .ToArray();
            buttons.Length.ShouldBe(2);

            var objectButton = buttons.Single(static button => button.Classes.Contains("semantic-object"));
            var functionButton = buttons.Single(static button => button.ButtonType == ButtonType.Primary);

            objectButton.Effect.ShouldBeOfType<DropShadowEffect>().BlurRadius.ShouldBe(2);
            objectButton.Effect.ShouldBeOfType<DropShadowEffect>().OffsetY.ShouldBe(1);
            functionButton.Background.ShouldBeAssignableTo<ISolidColorBrush>();
            ((ISolidColorBrush)functionButton.Background!).Color.ShouldBe(Color.Parse("#171717"));

            var objectContent = GetSemanticContent(objectButton);
            var functionContent = GetSemanticContent(functionButton);
            BrushShouldHaveSameColor(
                objectContent.Foreground,
                GetThemeResource<IBrush>(SharedTokenKind.ColorText));
            BrushShouldHaveSameColor(functionContent.Foreground, Brushes.White);

            SetPseudoClass(objectButton, ":pointerover", true);
            Dispatcher.UIThread.RunJobs();

            BrushShouldHaveSameColor(
                objectContent.Foreground,
                GetThemeResource<IBrush>(SharedTokenKind.ColorText));

            SetPseudoClass(functionButton, ":pointerover", true);
            Dispatcher.UIThread.RunJobs();
            functionButton.IsMotionEnabled.ShouldBeTrue();
            functionButton.IsWaveSpiritEnabled.ShouldBeTrue();

            BrushShouldHaveSameColor(functionContent.Foreground, Brushes.White);
            BrushShouldHaveSameColor(
                functionButton.BorderBrush,
                GetThemeResource<IBrush>(SharedTokenKind.ColorBorder));

            SetPrivatePropertyValue(functionButton, "IsPressed", true);
            functionButton.IsPressed.ShouldBeTrue();
            SetPrivatePropertyValue(functionButton, "IsPressed", false);
            functionButton.IsPressed.ShouldBeFalse();
            Dispatcher.UIThread.RunJobs();

            var waveSpiritDecorator = GetPrivateFieldValue(functionButton, "_waveSpiritDecorator");
            var waveBrush = GetPublicPropertyValue<IBrush?>(waveSpiritDecorator, "WaveBrush");
            BrushShouldHaveSameColor(
                waveBrush,
                GetThemeResource<IBrush>(SharedTokenKind.ColorBorder));
        });
    }

    [Fact]
    public void Button_Color_And_Variant_Example_Precedes_The_Final_Semantic_Part_Example()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var examples = ExtractButtonExampleItems(source);

        var colorVariantIndex = examples.IndexOf("ButtonShowCaseLangResource ColorVariantTitle", StringComparison.Ordinal);
        var semanticPartIndex = examples.IndexOf(
            "ButtonShowCaseLangResource SemanticPartStyleTitle",
            StringComparison.Ordinal);
        colorVariantIndex.ShouldBeGreaterThanOrEqualTo(0);
        semanticPartIndex.ShouldBeGreaterThan(colorVariantIndex);
        colorVariantIndex.ShouldBeGreaterThan(
            examples.IndexOf("ButtonShowCaseLangResource DisabledTitle", StringComparison.Ordinal));
        colorVariantIndex.ShouldBeGreaterThan(
            examples.IndexOf("ButtonShowCaseLangResource GhostButtonTitle", StringComparison.Ordinal));
        colorVariantIndex.ShouldBeGreaterThan(
            examples.IndexOf("ButtonShowCaseLangResource GradientButtonTitle", StringComparison.Ordinal));

        var colorVariantItem = examples[colorVariantIndex..];
        colorVariantItem.ShouldContain("Span=\"Full\"");
        colorVariantItem.ShouldContain("BadgeText=\"v6.0.5\"");
        colorVariantItem.ShouldNotContain("CustomBackground");
        colorVariantItem.ShouldNotContain("LinearGradientBrush");
        colorVariantItem.ShouldNotContain("P2ColorCustom");
        colorVariantItem.ShouldNotContain("P2ContentGradient");
        colorVariantItem.ShouldNotContain("RibbonBadgeText=\"v6.0.5\"");
    }

    [Fact]
    public void Button_Gradient_Example_Is_Separate_ShowCase_With_Two_Custom_Backgrounds()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var examples = ExtractButtonExampleItems(source);

        var colorVariantIndex = examples.IndexOf("ButtonShowCaseLangResource ColorVariantTitle", StringComparison.Ordinal);
        var gradientIndex     = examples.IndexOf("ButtonShowCaseLangResource GradientButtonTitle", StringComparison.Ordinal);
        gradientIndex.ShouldBeGreaterThanOrEqualTo(0);
        gradientIndex.ShouldBeLessThan(colorVariantIndex);

        var gradientItem = ExtractShowCaseItemByTitle(examples, "ButtonShowCaseLangResource GradientButtonTitle");
        gradientItem.ShouldContain("BadgeText=\"v6.0.5\"");
        gradientItem.ShouldNotContain("Span=\"Full\"");
        gradientItem.ShouldContain("CustomBackground");
        CountOccurrences(gradientItem, "<atom:Button.CustomBackground>").ShouldBe(2);
        CountOccurrences(gradientItem, "<LinearGradientBrush").ShouldBe(2);
        gradientItem.ShouldContain("#6253E1");
        gradientItem.ShouldContain("#04BEFE");
        gradientItem.ShouldContain("#FF7A45");
        gradientItem.ShouldContain("#FFD666");
        gradientItem.ShouldNotContain("/template/");
        gradientItem.ShouldNotContain("RibbonBadgeText=\"v6.0.5\"");
    }

    [Fact]
    public void Button_Size_Example_Uses_Custom_Option_To_Resize_All_Bound_Buttons()
    {
        var source           = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml.cs");
        var examples         = ExtractButtonExampleItems(source);

        var sizeItem = ExtractShowCaseItemByTitle(examples, "ButtonShowCaseLangResource SizeTitle");
        CountOccurrences(sizeItem, "<atom:OptionButton ").ShouldBe(4);
        sizeItem.ShouldContain("ButtonShowCaseLangResource P2ContentCustom");
        CountOccurrences(sizeItem, "SizeType=\"{Binding ButtonSizeType}\"").ShouldBe(10);
        CountOccurrences(sizeItem, "Classes=\"size-demo-button\"").ShouldBe(10);
        sizeItem.ShouldContain("Selector=\"atom|Button.size-demo-button[SizeType=Custom]\"");
        sizeItem.ShouldContain("<Setter Property=\"Height\" Value=\"44\" />");
        sizeItem.ShouldContain("<Setter Property=\"Padding\" Value=\"18,0\" />");
        sizeItem.ShouldContain("<Setter Property=\"FontSize\" Value=\"15\" />");
        sizeItem.ShouldNotContain("SizeType=\"Custom\"");
        sizeItem.ShouldNotContain("Height=\"44\"");
        sizeItem.ShouldNotContain("Padding=\"18,0\"");
        sizeItem.ShouldNotContain("FontSize=\"15\"");

        codeBehindSource.ShouldContain("CustomizableSizeType.Custom");
    }

    [Fact]
    public void Button_Icon_Placement_Example_Is_Separate_ShowCase_With_V606_Badge()
    {
        var source           = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml.cs");
        var viewModelSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/ViewModels/ButtonViewModel.cs");
        var examples         = ExtractButtonExampleItems(source);

        var iconItem          = ExtractShowCaseItemByTitle(examples, "ButtonShowCaseLangResource IconTitle");
        var iconPlacementItem = ExtractShowCaseItemByTitle(examples, "ButtonShowCaseLangResource IconPlacementTitle");

        iconItem.ShouldNotContain("IconPlacement=\"End\"");
        iconPlacementItem.ShouldContain("BadgeText=\"v6.0.6\"");
        iconPlacementItem.ShouldContain("ButtonShowCaseLangResource IconPlacementDescription");
        iconPlacementItem.ShouldContain("ButtonShowCaseLangResource P2TextIconPlacement");
        iconPlacementItem.ShouldContain("OptionCheckedChanged=\"HandleButtonIconPlacementOptionCheckedChanged\"");
        iconPlacementItem.ShouldContain("IconPlacement=\"{Binding ButtonIconPlacement}\"");
        CountOccurrences(iconPlacementItem, "IconPlacement=\"{Binding ButtonIconPlacement}\"").ShouldBe(8);

        codeBehindSource.ShouldContain("HandleButtonIconPlacementOptionCheckedChanged");
        codeBehindSource.ShouldContain("ButtonIconPlacement.Start");
        codeBehindSource.ShouldContain("ButtonIconPlacement.End");
        viewModelSource.ShouldContain("ButtonIconPlacement ButtonIconPlacement");
    }

    [Fact]
    public void Button_ShowCase_Version_Ribbon_Anchors_To_Real_ShowCaseItem_Right_Edge()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new ButtonShowCase
        {
            DataContext = new ButtonViewModel(new TestScreen())
        };
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child              = page
        };

        ShowInWindow(visualLayerManager, 1440, 900, () =>
        {
            var item = page.GetVisualDescendants()
                           .OfType<ShowCaseItem>()
                           .Single(showCaseItem => showCaseItem.BadgeText == "v6.0.6");
            var ribbonBadge = item.GetVisualDescendants()
                                  .OfType<AtomRibbonBadge>()
                                  .Single();
            var label = visualLayerManager.GetVisualDescendants()
                                          .OfType<TextBlock>()
                                          .Single(textBlock => textBlock.Text == "v6.0.6");

            var labelRight = label.TranslatePoint(new Point(label.Bounds.Width, 0), visualLayerManager);
            var targetRight = ribbonBadge.TranslatePoint(new Point(ribbonBadge.Bounds.Width, 0), visualLayerManager);

            labelRight.ShouldNotBeNull();
            targetRight.ShouldNotBeNull();
            (labelRight.Value.X - targetRight.Value.X).ShouldBe(8, 1,
                "ShowCaseItem version RibbonBadge should match Ant Design's right:-badgeRibbonOffset placement.");
        });
    }

    [Fact]
    public void Button_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ButtonShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractButtonExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractButtonExampleItems(string source)
    {
        const string firstItemMarker = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string ExtractShowCaseItemByTitle(string source, string titleResource)
    {
        const string itemStartMarker = "<gallery:ShowCaseItem";
        const string itemCloseMarker = "</gallery:ShowCaseItem>";

        var titleIndex = source.IndexOf(titleResource, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);

        var itemStart = source.LastIndexOf(itemStartMarker, titleIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        var itemClose = source.IndexOf(itemCloseMarker, titleIndex, StringComparison.Ordinal);
        itemClose.ShouldBeGreaterThan(titleIndex);

        return source[itemStart..(itemClose + itemCloseMarker.Length)];
    }

    private static string NormalizeMarkup(string source)
    {
        return ShowCaseSnapshotMarkup.Normalize(source);
    }

    private static int CountOccurrences(string source, string value)
    {
        var count = 0;
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
        ShowInWindow(content, width, height, _ => assertion());
    }

    private static void ShowInWindow(
        Control content,
        double width,
        double height,
        Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Content = content,
            Width   = width,
            Height  = height
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static ContentPresenter GetSemanticContent(Control button)
    {
        var content = button.GetVisualDescendants()
                            .OfType<ContentPresenter>()
                            .SingleOrDefault(static presenter => presenter.Classes.Contains("semantic-content"));
        content.ShouldNotBeNull();
        return content!;
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current.ShouldNotBeNull();
        application.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static void BrushShouldHaveSameColor(IBrush? actual, IBrush? expected)
    {
        actual.ShouldNotBeNull();
        expected.ShouldNotBeNull();
        actual.ShouldBeAssignableTo<ISolidColorBrush>();
        expected.ShouldBeAssignableTo<ISolidColorBrush>();
        ((ISolidColorBrush)actual!).Color.ShouldBe(((ISolidColorBrush)expected!).Color);
    }

    private static object GetPrivateFieldValue(object target, string fieldName)
    {
        var field = target.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        var value = field.GetValue(target);
        value.ShouldNotBeNull();
        return value;
    }

    private static T GetPublicPropertyValue<T>(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public);
        property.ShouldNotBeNull();
        return (T)property.GetValue(target)!;
    }

    private static void SetPrivatePropertyValue<T>(object target, string propertyName, T value)
    {
        var property = typeof(Avalonia.Controls.Button).GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public);
        property.ShouldNotBeNull();
        var setter = property.GetSetMethod(true);
        setter.ShouldNotBeNull();
        setter.Invoke(target, [value]);
    }

    private static void SetPseudoClass(Control control, string pseudoClass, bool value)
    {
        ((IPseudoClasses)control.Classes).Set(pseudoClass, value);
    }

    private sealed class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }
}
