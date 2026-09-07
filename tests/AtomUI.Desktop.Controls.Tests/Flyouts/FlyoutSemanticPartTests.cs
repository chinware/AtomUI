using System.Xml.Linq;
using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Flyouts;

public class FlyoutSemanticPartTests
{
    private const string PopupRootClass = "semantic-popup-root";
    private const string PopupContainerClass = "semantic-popup-container";
    private const string PopupContentClass = "semantic-popup-content";
    private const string PopupArrowClass = "semantic-popup-arrow";

    private const string FlyoutHostThemePath =
        "src/AtomUI.Desktop.Controls/Flyouts/Themes/FlyoutHostTheme.axaml";

    private const string ArrowDecoratedBoxThemePath =
        "src/AtomUI.Desktop.Controls/Primitives/ArrowDecoratedBox/Themes/ArrowDecoratedBoxTheme.axaml";

    private static readonly string[] ApprovedPartNames =
    [
        "root", "popup.arrow", "popup.container", "popup.content", "popup.root"
    ];

    static FlyoutSemanticPartTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Registered_Descriptor_Exposes_Only_The_Approved_InfoFlyout_Parts()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;

        registry.TryGetControl(typeof(FlyoutHost), out var descriptor).ShouldBeTrue();
        descriptor.ShouldNotBeNull();
        descriptor.Parts.Select(static part => part.Name)
                  .ShouldBe(ApprovedPartNames);

        AssertRoot(descriptor, typeof(FlyoutHost));
        AssertPart(descriptor, "popup.root", PopupRootClass, typeof(FlyoutPresenter),
            ">> .semantic-popup-root");
        AssertPart(descriptor, "popup.container", PopupContainerClass, typeof(Border),
            ">> .semantic-popup-root >> .semantic-popup-container");
        AssertPart(descriptor, "popup.content", PopupContentClass, typeof(ContentPresenter),
            ">> .semantic-popup-root >> .semantic-popup-content");
        AssertPart(descriptor, "popup.arrow", PopupArrowClass, typeof(ArrowIndicator),
            ">> .semantic-popup-root >> .semantic-popup-arrow");

        foreach (var part in descriptor.Parts.Where(static part => part.Name != "root"))
        {
            part.CrossVisualRoot.ShouldBeTrue(
                $"{part.Name} must be cross-visual-root because the popup is code-created outside the owner template");
            part.RuntimeCreated.ShouldBeTrue(
                $"{part.Name} must be runtime-created because its marker class is injected in code, not declared in shared theme AXAML");
        }
    }

    [Fact]
    public void IsPopupPinnedOpen_Is_Public_Api()
    {
        // Gallery 语义预览需要在 AXAML 中钉住打开弹层（跨程序集），因此该属性必须公开。
        // 先例：AbstractColorPicker.SemanticPartTests.IsPopupPinnedOpen_Is_Public_Api。
        var property = typeof(FlyoutHost)
            .GetProperty(nameof(FlyoutHost.IsPopupPinnedOpen));
        property.ShouldNotBeNull();
        property.GetGetMethod().ShouldNotBeNull().IsPublic.ShouldBeTrue();
        property.GetSetMethod().ShouldNotBeNull().IsPublic.ShouldBeTrue();
    }

    [Fact]
    public void Built_In_Templates_Do_Not_Declare_Semantic_Markers()
    {
        AssertNoSemanticMarkers(FlyoutHostThemePath);
        AssertNoSemanticMarkers(ArrowDecoratedBoxThemePath);
    }

    [Fact]
    public void Default_Themes_Do_Not_Consume_Semantic_Selectors()
    {
        AssertNoSemanticSelectors(FlyoutHostThemePath);
        AssertNoSemanticSelectors(ArrowDecoratedBoxThemePath);
    }

    [Fact]
    public void Popup_Parts_Expose_Markers_When_Opened()
    {
        var anchor = new Border { Width = 100, Height = 30 };
        var flyout = new Flyout
        {
            Content               = new Border { Width = 120, Height = 40 },
            IsMotionEnabled       = false,
            ShouldUseOverlayPopup = true
        };
        var host = new FlyoutHost
        {
            Content         = anchor,
            Flyout          = flyout,
            IsMotionEnabled = false
        };
        var window = new AtomUIWindow
        {
            Width   = 320,
            Height  = 240,
            Content = host
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            host.IsPopupPinnedOpen = true;
            Dispatcher.UIThread.RunJobs();

            var popup = flyout.Popup.ShouldBeOfType<Popup>();
            popup.IsOpen.ShouldBeTrue();
            var presenter = popup.Child.ShouldBeOfType<FlyoutPresenter>();
            presenter.Classes.Contains(PopupRootClass).ShouldBeTrue();
            presenter.ApplyTemplate();

            presenter.GetVisualDescendants()
                     .OfType<Border>()
                     .Single(control => control.Classes.Contains(PopupContainerClass))
                     .ShouldNotBeNull();
            presenter.GetVisualDescendants()
                     .OfType<ContentPresenter>()
                     .Single(control => control.Classes.Contains(PopupContentClass))
                     .ShouldNotBeNull();
            presenter.GetVisualDescendants()
                     .OfType<ArrowIndicator>()
                     .Single(control => control.Classes.Contains(PopupArrowClass))
                     .ShouldNotBeNull();
        }
        finally
        {
            host.IsPopupPinnedOpen = false;
            flyout.Hide();
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Generated_Semantic_Styles_Are_Instantiable_For_Every_Part()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;
        registry.TryGetControl(typeof(FlyoutHost), out var descriptor).ShouldBeTrue();
        descriptor.ShouldNotBeNull();

        foreach (var part in descriptor.Parts.Where(static part => part.Name != "root"))
        {
            var styleType = part.StyleType.ShouldNotBeNull();
            Activator.CreateInstance(styleType).ShouldBeAssignableTo<Style>();
        }
    }

    private static void AssertRoot(ControlSemanticDescriptor descriptor, Type controlType)
    {
        var root = descriptor.Parts.Single(static part => part.Name == "root");
        root.Path.ShouldBe("root");
        root.SelectorClass.ShouldBeNull();
        root.SelectorRoute.ShouldBeNull();
        root.ContractType.ShouldBe(controlType);
        root.Cardinality.ShouldBe(SemanticPartCardinality.Single);
        root.Customization.ShouldBe(SemanticPartCustomization.Root);
        root.StyleType.ShouldBeNull();
    }

    private static void AssertPart(
        ControlSemanticDescriptor descriptor,
        string name,
        string selectorClass,
        Type contractType,
        string selectorRoute)
    {
        var part = descriptor.Parts.Single(candidate => candidate.Name == name);
        part.Path.ShouldBe(name);
        part.SelectorClass.ShouldBe(selectorClass);
        part.SelectorRoute.ShouldBe(selectorRoute);
        part.ContractType.ShouldBe(contractType);
        part.Cardinality.ShouldBe(SemanticPartCardinality.Single);
        part.Customization.ShouldBe(SemanticPartCustomization.Selector);
        part.Since.ShouldBe("6.0");
        part.StyleType.ShouldNotBeNull();
    }

    private static void AssertNoSemanticMarkers(string themePath)
    {
        var document = XDocument.Load(GetRepoFile(themePath), LoadOptions.SetLineInfo);
        document.Descendants()
                .Any(static element => element.Attributes()
                    .Any(static attribute => attribute.Name.LocalName.StartsWith(
                        "Classes.semantic-", StringComparison.Ordinal)))
                .ShouldBeFalse($"{themePath} must not declare semantic marker classes");
    }

    private static void AssertNoSemanticSelectors(string themePath)
    {
        var document = XDocument.Load(GetRepoFile(themePath), LoadOptions.SetLineInfo);
        var selectors = document.Descendants()
                                .Where(static element => element.Name.LocalName == "Style")
                                .Attributes("Selector")
                                .Select(static attribute => attribute.Value)
                                .ToArray();

        selectors.ShouldAllBe(static selector =>
            !selector.Contains("semantic-", StringComparison.Ordinal));
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

        throw new FileNotFoundException($"Could not locate repository file '{relativePath}'.");
    }
}
