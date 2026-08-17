using System.Xml.Linq;
using AtomUI.Theme;
using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUICalendar = AtomUI.Desktop.Controls.Calendar;
using AtomUILunarCalendar = AtomUI.Desktop.Controls.LunarCalendar;
using CalendarHeaderControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarHeader;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;
using CalendarCellControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarViewCell;
using ComboBoxControl = AtomUI.Desktop.Controls.ComboBox;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarSemanticPartTests
{
    private const string HeaderClass = "semantic-header";
    private const string BodyClass = "semantic-body";
    private const string ContentClass = "semantic-content";
    private const string ItemClass = "semantic-item";
    private const string ItemContentClass = "semantic-item-content";
    private const string HeaderSelectorRoute = "/template/ .semantic-header";
    private const string BodySelectorRoute = "/template/ .semantic-body";
    private const string ContentSelectorRoute = "/template/ .semantic-content";
    private const string ItemSelectorRoute =
        "/template/ .semantic-content > .semantic-scope-body > .semantic-scope-cells > .semantic-item";
    private const string ItemContentSelectorRoute =
        "/template/ .semantic-content > .semantic-scope-body > .semantic-scope-cells > .semantic-item /template/ .semantic-item-content";

    static CalendarSemanticPartTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Registered_Descriptor_Exposes_Only_The_Approved_Parts()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;

        registry.TryGetControl(typeof(AtomUICalendar), out var descriptor).ShouldBeTrue();
        descriptor.ShouldNotBeNull();
        descriptor.Parts.Select(static part => part.Name)
                  .ShouldBe(["root", "body", "content", "header", "item", "itemContent"]);

        AssertRoot(descriptor.Parts.Single(static part => part.Name == "root"));
        AssertPart(descriptor.Parts.Single(static part => part.Name == "header"),
            HeaderClass,
            typeof(TemplatedControl),
            SemanticPartCardinality.Single,
            false,
            HeaderSelectorRoute);
        AssertPart(descriptor.Parts.Single(static part => part.Name == "body"),
            BodyClass,
            typeof(Avalonia.Controls.DockPanel),
            SemanticPartCardinality.Single,
            false,
            BodySelectorRoute);
        AssertPart(descriptor.Parts.Single(static part => part.Name == "content"),
            ContentClass,
            typeof(TemplatedControl),
            SemanticPartCardinality.Single,
            false,
            ContentSelectorRoute);
        AssertPart(descriptor.Parts.Single(static part => part.Name == "item"),
            ItemClass,
            typeof(TemplatedControl),
            SemanticPartCardinality.Multiple,
            true,
            ItemSelectorRoute);
        AssertPart(descriptor.Parts.Single(static part => part.Name == "itemContent"),
            ItemContentClass,
            typeof(ContentControl),
            SemanticPartCardinality.Multiple,
            true,
            ItemContentSelectorRoute);

        registry.TryGetControl(typeof(AtomUILunarCalendar), out var lunarDescriptor).ShouldBeTrue();
        lunarDescriptor.ShouldNotBeNull();
        lunarDescriptor.Parts.Select(static part => part.Name)
                      .ShouldBe(["root", "body", "content", "header", "item", "itemContent"]);
        lunarDescriptor.Parts.Single(static part => part.Name == "header")
                      .SelectorClass.ShouldBe(HeaderClass);
        lunarDescriptor.Parts.Single(static part => part.Name == "item")
                      .SelectorRoute.ShouldBe(ItemSelectorRoute);
        lunarDescriptor.Parts.Single(static part => part.Name == "itemContent")
                      .ContractType.ShouldBe(typeof(ContentControl));
        lunarDescriptor.Parts.ShouldAllBe(static part =>
            part.Cardinality == SemanticPartCardinality.Single ||
            part.Cardinality == SemanticPartCardinality.Multiple);

        registry.TryGetControl(typeof(CalendarHeaderControl), out _).ShouldBeFalse();
        registry.TryGetControl(typeof(CalendarViewControl), out _).ShouldBeFalse();
        registry.TryGetControl(typeof(CalendarCellControl), out _).ShouldBeFalse();
    }

    [Fact]
    public void Templates_Implement_Only_The_Approved_Static_Markers()
    {
        AssertThemeMarkers(
            "src/AtomUI.Desktop.Controls/Calendar/Themes/CalendarTheme.axaml",
            ["semantic-body:DockPanel", "semantic-content:CalendarView", "semantic-header:CalendarHeader"]);
        AssertThemeMarkers(
            "src/AtomUI.Desktop.Controls/Calendar/Themes/CalendarViewTheme.axaml",
            ["semantic-scope-body:DockPanel", "semantic-scope-cells:Grid"]);
        AssertThemeMarkers(
            "src/AtomUI.Desktop.Controls/Calendar/Themes/CalendarViewCellTheme.axaml",
            ["semantic-item-content:ContentControl"]);
        AssertThemeMarkers(
            "src/AtomUI.Desktop.Controls/Calendar/Themes/LunarCalendarViewCellTheme.axaml",
            ["semantic-item-content:ContentControl"]);
    }

    [Theory]
    [InlineData(false, CalendarMode.Month, 42)]
    [InlineData(true, CalendarMode.Month, 48)]
    [InlineData(false, CalendarMode.Year, 12)]
    public void Grid_Topology_Produces_One_Item_And_ItemContent_Marker_Per_Cell(
        bool showWeek,
        CalendarMode mode,
        int expectedCellCount)
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            Mode = mode,
            ShowWeek = showWeek
        };

        ShowInWindow(calendar, () =>
        {
            GetSemanticItemCells(calendar).Length.ShouldBe(expectedCellCount);
            GetSemanticItemContentPresenters(calendar).Length.ShouldBe(expectedCellCount);
        });
    }

    [Fact]
    public void Same_Month_Value_Change_Reuses_Cells_And_Preserves_Item_Marker()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };

        ShowInWindow(calendar, () =>
        {
            var originalCells = GetSemanticItemCells(calendar);

            calendar.Value = new DateTime(2026, 7, 20);
            Dispatcher.UIThread.RunJobs();

            var currentCells = GetSemanticItemCells(calendar);
            currentCells.Length.ShouldBe(42);
            currentCells.ShouldBe(originalCells, ignoreOrder: true);
            currentCells.ShouldAllBe(static cell => cell.Classes.Contains(ItemClass));
        });
    }

    [Fact]
    public void Mode_Switch_Converges_The_Pool_And_Keeps_Every_Cell_Marked()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };

        ShowInWindow(calendar, () =>
        {
            GetSemanticItemCells(calendar).Length.ShouldBe(42);

            calendar.Mode = CalendarMode.Year;
            Dispatcher.UIThread.RunJobs();
            GetSemanticItemCells(calendar).Length.ShouldBe(12);
            GetSemanticItemContentPresenters(calendar).Length.ShouldBe(12);

            calendar.Mode = CalendarMode.Month;
            Dispatcher.UIThread.RunJobs();
            var cells = GetSemanticItemCells(calendar);
            cells.Length.ShouldBe(42);
            cells.ShouldAllBe(static cell => cell.Classes.Contains(ItemClass));
            GetSemanticItemContentPresenters(calendar).Length.ShouldBe(42);
        });
    }

    [Fact]
    public void HeaderTemplate_Hides_Default_Header_But_Preserves_Marker_And_Excludes_Custom_Header()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };

        ShowInWindow(calendar, () =>
        {
            var header = GetSemanticHeader(calendar);
            header.IsVisible.ShouldBeTrue();

            calendar.HeaderTemplate = new FuncDataTemplate<object>(
                static (_, _) => new Avalonia.Controls.TextBlock { Text = "Custom" });
            Dispatcher.UIThread.RunJobs();

            GetSemanticHeader(calendar).ShouldBeSameAs(header);
            header.IsVisible.ShouldBeFalse();
            header.Classes.ShouldContain(HeaderClass);

            var customHeader = calendar.GetVisualDescendants()
                                       .OfType<ContentControl>()
                                       .Single(static control => control.Name == "PART_CustomHeader");
            customHeader.IsVisible.ShouldBeTrue();
            customHeader.Classes.ShouldNotContain(HeaderClass);
        });
    }

    [Fact]
    public void Fullscreen_Cells_Expose_Visible_Item_Content_Regions_Without_Templates()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };

        ShowInWindow(calendar, () =>
        {
            var presenters = GetSemanticItemContentPresenters(calendar);
            presenters.Length.ShouldBe(42);
            presenters.ShouldAllBe(static presenter => presenter.IsVisible);
            presenters.ShouldAllBe(static presenter => presenter.Bounds.Height > 0);

            calendar.CellTemplate = new FuncDataTemplate<CalendarCellContext>(
                static (_, _) => new Avalonia.Controls.TextBlock { Text = "Content" });
            Dispatcher.UIThread.RunJobs();

            var visiblePresenters = GetSemanticItemContentPresenters(calendar);
            visiblePresenters.Length.ShouldBe(42);
            visiblePresenters.ShouldAllBe(static presenter => presenter.IsVisible);
        });
    }

    [Fact]
    public void Mini_Cells_Keep_Item_Content_Hidden_Without_Templates()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15), Fullscreen = false };

        ShowInWindow(calendar, () =>
        {
            GetSemanticItemContentPresenters(calendar)
                .ShouldAllBe(static presenter => !presenter.IsVisible);
        });
    }

    [Fact]
    public void Root_Semantic_Part_Projects_The_Standard_Surface_Properties()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        calendar.Background = new SolidColorBrush(Colors.AliceBlue);
        calendar.BorderBrush = new SolidColorBrush(Color.Parse("#CDC1FF"));
        calendar.BorderThickness = new Thickness(1);
        calendar.CornerRadius = new CornerRadius(8);
        calendar.Padding = new Thickness(10);

        ShowInWindow(calendar, () =>
        {
            var root = GetRootBorder(calendar);

            root.Background.ShouldBeSameAs(calendar.Background);
            root.BorderBrush.ShouldBeSameAs(calendar.BorderBrush);
            root.BorderThickness.ShouldBe(new Thickness(1));
            root.CornerRadius.ShouldBe(new CornerRadius(8));
            root.Padding.ShouldBe(new Thickness(10));
        });
    }

    [Fact]
    public void Semantic_Selectors_Match_Their_Declared_Owner_Routes()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        calendar.Classes.Add("semantic-owner");
        calendar.Styles.Add(new Style(selector =>
            selector.OfType<AtomUICalendar>()
                    .Class("semantic-owner")
                    .Template()
                    .Class(HeaderClass))
        {
            Setters = { new Setter(Control.TagProperty, "header") }
        });
        calendar.Styles.Add(new Style(selector =>
            selector.OfType<AtomUICalendar>()
                    .Class("semantic-owner")
                    .Template()
                    .Class(BodyClass))
        {
            Setters = { new Setter(Control.TagProperty, "body") }
        });
        calendar.Styles.Add(new Style(selector =>
            selector.OfType<AtomUICalendar>()
                    .Class("semantic-owner")
                    .Template()
                    .Class(ContentClass))
        {
            Setters = { new Setter(Control.TagProperty, "content") }
        });
        calendar.Styles.Add(new Style(selector =>
            selector.OfType<AtomUICalendar>()
                    .Class("semantic-owner")
                    .Template()
                    .Class(ContentClass)
                    .Child()
                    .Class("semantic-scope-body")
                    .Child()
                    .Class("semantic-scope-cells")
                    .Child()
                    .Class(ItemClass))
        {
            Setters = { new Setter(Control.TagProperty, "item") }
        });
        calendar.Styles.Add(new Style(selector =>
            selector.OfType<AtomUICalendar>()
                    .Class("semantic-owner")
                    .Template()
                    .Class(ContentClass)
                    .Child()
                    .Class("semantic-scope-body")
                    .Child()
                    .Class("semantic-scope-cells")
                    .Child()
                    .Class(ItemClass)
                    .Template()
                    .Class(ItemContentClass))
        {
            Setters = { new Setter(Control.TagProperty, "item-content") }
        });

        ShowInWindow(calendar, () =>
        {
            GetSemanticHeader(calendar).Tag.ShouldBe("header");
            GetSemanticBody(calendar).Tag.ShouldBe("body");
            GetSemanticContent(calendar).Tag.ShouldBe("content");
            GetSemanticItemCells(calendar).ShouldAllBe(static cell => Equals(cell.Tag, "item"));
            GetSemanticItemContentPresenters(calendar)
                .ShouldAllBe(static presenter => Equals(presenter.Tag, "item-content"));
        });
    }

    [Fact]
    public void Item_Semantic_Background_Is_Projected_To_The_Cell_Item_Border()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        calendar.Classes.Add("semantic-owner");
        calendar.Styles.Add(new Style(selector =>
            selector.OfType<AtomUICalendar>()
                    .Class("semantic-owner")
                    .Template()
                    .Class(ContentClass)
                    .Child()
                    .Class("semantic-scope-body")
                    .Child()
                    .Class("semantic-scope-cells")
                    .Child()
                    .Class(ItemClass))
        {
            Setters = { new Setter(TemplatedControl.BackgroundProperty, Brushes.Silver) }
        });

        ShowInWindow(calendar, () =>
        {
            GetSemanticItemCells(calendar).ShouldAllBe(static cell =>
                ReferenceEquals(GetItemBorder(cell).Background, Brushes.Silver));
        });
    }

    [Fact]
    public void Content_Semantic_Background_Overrides_The_Fullscreen_Panel_Background()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        calendar.Classes.Add("semantic-owner");
        calendar.Styles.Add(new Style(selector =>
            selector.OfType<AtomUICalendar>()
                    .Class("semantic-owner")
                    .Template()
                    .Class(ContentClass))
        {
            Setters = { new Setter(TemplatedControl.BackgroundProperty, Brushes.Gold) }
        });

        ShowInWindow(calendar, () =>
        {
            GetSemanticContent(calendar).Background.ShouldBeSameAs(Brushes.Gold);
            GetContentViewBody(calendar).Background.ShouldBeSameAs(Brushes.Gold);
        });
    }

    [Fact]
    public void Mini_Panel_Keeps_Its_Own_Background_When_The_Root_Surface_Is_Customized()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            Fullscreen = false,
            Background = new SolidColorBrush(Colors.AliceBlue)
        };

        ShowInWindow(calendar, () =>
        {
            var content = GetSemanticContent(calendar);
            content.Background.ShouldNotBeSameAs(calendar.Background);
            HasWhiteContainerColor(content.Background).ShouldBeTrue();
            GetContentViewBody(calendar).Background.ShouldBeSameAs(content.Background);
        });
    }

    [Fact]
    public void Fullscreen_Panel_Keeps_The_Full_Bg_Default()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            Background = new SolidColorBrush(Colors.AliceBlue)
        };

        ShowInWindow(calendar, () =>
        {
            var content = GetSemanticContent(calendar);
            content.Background.ShouldNotBeSameAs(calendar.Background);
            HasWhiteContainerColor(content.Background).ShouldBeTrue();
            HasWhiteContainerColor(GetContentViewBody(calendar).Background).ShouldBeTrue();
        });
    }

    [Fact]
    public void Header_Year_Month_Selects_And_Mode_Switch_Use_White_Container_Backgrounds()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };

        ShowInWindow(calendar, () =>
        {
            var header = GetSemanticHeader(calendar);

            header.GetVisualDescendants()
                  .OfType<ComboBoxControl>()
                  .Count()
                  .ShouldBe(2);

            var selectBoxes = header.GetVisualDescendants()
                                    .OfType<AddOnDecoratedBox>()
                                    .ToArray();
            selectBoxes.Length.ShouldBe(2);
            selectBoxes.ShouldAllBe(static box => HasWhiteContainerColor(box.Background));

            var modeSwitch = header.GetVisualDescendants()
                                   .OfType<OptionButtonGroup>()
                                   .Single();
            HasWhiteContainerColor(modeSwitch.Background).ShouldBeTrue();
            HasBorderColor(modeSwitch.BorderBrush).ShouldBeTrue();
        });
    }

    [Fact]
    public void LunarCalendar_Produces_The_Same_Marker_Topology_As_Calendar()
    {
        var calendar = new AtomUILunarCalendar { Value = new DateTime(2026, 7, 15) };

        ShowInWindow(calendar, () =>
        {
            GetSemanticHeader(calendar).Classes.ShouldContain(HeaderClass);
            GetSemanticBody(calendar).Classes.ShouldContain(BodyClass);
            GetSemanticContent(calendar).Classes.ShouldContain(ContentClass);
            GetSemanticItemCells(calendar).Length.ShouldBe(42);
            GetSemanticItemContentPresenters(calendar).Length.ShouldBe(42);
            GetSemanticItemCells(calendar).ShouldAllBe(static cell =>
                cell.Classes.Contains(ItemClass) &&
                cell.GetType() == typeof(AtomUI.Desktop.Controls.Internal.Calendar.LunarCalendarViewCell));
        });
    }

    private static bool HasWhiteContainerColor(IBrush? background)
    {
        return background is ISolidColorBrush brush && brush.Color == Color.Parse("#FFFFFF");
    }

    private static bool HasBorderColor(IBrush? borderBrush)
    {
        return borderBrush is ISolidColorBrush brush && brush.Color == Color.Parse("#D9D9D9");
    }

    private static CalendarHeaderControl GetSemanticHeader(AtomUICalendar calendar)
    {
        return calendar.GetVisualDescendants()
                       .OfType<CalendarHeaderControl>()
                       .Single(static header => header.Classes.Contains(HeaderClass));
    }

    private static Avalonia.Controls.DockPanel GetSemanticBody(AtomUICalendar calendar)
    {
        return calendar.GetVisualDescendants()
                       .OfType<Avalonia.Controls.DockPanel>()
                       .Single(static panel => panel.Classes.Contains(BodyClass));
    }

    private static CalendarViewControl GetSemanticContent(AtomUICalendar calendar)
    {
        return calendar.GetVisualDescendants()
                       .OfType<CalendarViewControl>()
                       .Single(static view => view.Classes.Contains(ContentClass));
    }

    private static CalendarCellControl[] GetSemanticItemCells(AtomUICalendar calendar)
    {
        return calendar.GetVisualDescendants()
                       .OfType<CalendarCellControl>()
                       .Where(static cell => cell.Classes.Contains(ItemClass))
                       .ToArray();
    }

    private static ContentControl[] GetSemanticItemContentPresenters(AtomUICalendar calendar)
    {
        return calendar.GetVisualDescendants()
                       .OfType<ContentControl>()
                       .Where(static presenter =>
                           presenter.Name == "PART_ItemContent" &&
                           presenter.Classes.Contains(ItemContentClass))
                       .ToArray();
    }

    private static Avalonia.Controls.Border GetRootBorder(AtomUICalendar calendar)
    {
        return calendar.GetVisualDescendants()
                       .OfType<Avalonia.Controls.Border>()
                       .Single(static border => border.Name == "PART_Root");
    }

    private static Avalonia.Controls.Border GetItemBorder(CalendarCellControl cell)
    {
        return cell.GetVisualDescendants()
                   .OfType<Avalonia.Controls.Border>()
                   .Single(static border => border.Name == "PART_Item");
    }

    private static Avalonia.Controls.DockPanel GetContentViewBody(AtomUICalendar calendar)
    {
        return calendar.GetVisualDescendants()
                       .OfType<CalendarViewControl>()
                       .Single()
                       .GetVisualDescendants()
                       .OfType<Avalonia.Controls.DockPanel>()
                       .Single(static panel => panel.Name == "PART_Body");
    }

    private static void AssertThemeMarkers(string relativePath, string[] expectedMarkers)
    {
        var document = XDocument.Load(GetRepoFile(relativePath), LoadOptions.SetLineInfo);
        var literalSemanticMarkers = document.Descendants()
                                             .Attributes()
                                             .Where(static attribute =>
                                                 attribute.Name.LocalName == "Classes" &&
                                                 attribute.Value.Split(
                                                             (char[]?)null,
                                                             StringSplitOptions.RemoveEmptyEntries)
                                                         .Any(static value => value.StartsWith(
                                                             "semantic-",
                                                             StringComparison.Ordinal)))
                                             .ToArray();
        var classPropertyMarkers = document.Descendants()
                                           .SelectMany(static element => element.Attributes()
                                               .Where(static attribute => attribute.Name.LocalName.StartsWith(
                                                   "Classes.semantic-",
                                                   StringComparison.Ordinal))
                                               .Select(attribute => (Element: element, Attribute: attribute)))
                                           .ToArray();

        literalSemanticMarkers.ShouldBeEmpty();
        classPropertyMarkers.ShouldAllBe(static marker =>
            string.Equals(marker.Attribute.Value, "true", StringComparison.OrdinalIgnoreCase));
        classPropertyMarkers.Select(static marker =>
                                $"{marker.Attribute.Name.LocalName["Classes.".Length..]}:{marker.Element.Name.LocalName}")
                            .OrderBy(static value => value, StringComparer.Ordinal)
                            .ShouldBe(expectedMarkers);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width = 800,
            Height = 600,
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

    private static void AssertRoot(SemanticPartDescriptor part)
    {
        part.Path.ShouldBe("root");
        part.SelectorClass.ShouldBeNull();
        part.ContractType.ShouldBe(typeof(AtomUICalendar));
        part.Cardinality.ShouldBe(SemanticPartCardinality.Single);
        part.Customization.ShouldBe(SemanticPartCustomization.Root);
        part.CrossVisualRoot.ShouldBeFalse();
        part.RuntimeCreated.ShouldBeFalse();
    }

    private static void AssertPart(
        SemanticPartDescriptor part,
        string selectorClass,
        Type contractType,
        SemanticPartCardinality cardinality,
        bool runtimeCreated,
        string selectorRoute)
    {
        part.Path.ShouldBe(part.Name);
        part.SelectorClass.ShouldBe(selectorClass);
        part.SelectorRoute.ShouldBe(selectorRoute);
        part.ContractType.ShouldBe(contractType);
        part.Cardinality.ShouldBe(cardinality);
        part.Customization.ShouldBe(SemanticPartCustomization.Selector);
        part.CrossVisualRoot.ShouldBeFalse();
        part.RuntimeCreated.ShouldBe(runtimeCreated);
        part.Since.ShouldBe("6.0");
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
