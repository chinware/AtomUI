using System.Collections.ObjectModel;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.DesignTokens;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsSemanticStyleTests
{
    static StepsSemanticStyleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Semantic_Style_Properties_Are_Nullable_Root_APIs_With_Null_Defaults()
    {
        var steps = new Desktop.Controls.Steps();

        steps.ItemHeaderForeground.ShouldBeNull();
        steps.ItemSubHeaderForeground.ShouldBeNull();
        steps.ItemRailBackground.ShouldBeNull();

        typeof(Desktop.Controls.StepsItem).GetProperty(nameof(Desktop.Controls.Steps.ItemHeaderForeground))
                                          .ShouldBeNull();
        typeof(Desktop.Controls.StepsItem).GetProperty(nameof(Desktop.Controls.Steps.ItemSubHeaderForeground))
                                          .ShouldBeNull();
        typeof(Desktop.Controls.StepsItem).GetProperty(nameof(Desktop.Controls.Steps.ItemRailBackground))
                                          .ShouldBeNull();
    }

    [Fact]
    public void Direct_Items_Receive_Runtime_Semantic_Style_Updates_And_Null_Restores_Tokens()
    {
        var firstHeaderBrush    = new SolidColorBrush(Colors.Crimson);
        var secondHeaderBrush   = new SolidColorBrush(Colors.DarkRed);
        var subHeaderBrush      = new SolidColorBrush(Colors.SeaGreen);
        var railBrush           = new SolidColorBrush(Colors.Goldenrod);
        var steps = CreateDirectSteps(firstHeaderBrush, subHeaderBrush, railBrush);

        ShowInWindow(steps, () =>
        {
            var items = steps.Items.Cast<Desktop.Controls.StepsItem>().ToArray();
            items.ShouldAllBe(item => ReferenceEquals(item.ItemHeaderForeground, firstHeaderBrush));
            items.ShouldAllBe(item => ReferenceEquals(item.ItemSubHeaderForeground, subHeaderBrush));
            items.ShouldAllBe(item => ReferenceEquals(item.ItemRailBackground, railBrush));

            var first = items[0];
            FindPresenter(first, "HeaderPresenter").Foreground.ShouldBeSameAs(firstHeaderBrush);
            FindPresenter(first, "SubHeaderPresenter").Foreground.ShouldBeSameAs(subHeaderBrush);
            FindConnector(first).Background.ShouldBeSameAs(railBrush);
            FindPresenter(first, "ContentPresenter").Foreground.ShouldNotBeSameAs(subHeaderBrush);

            steps.ItemHeaderForeground = secondHeaderBrush;
            Dispatcher.UIThread.RunJobs();

            items.ShouldAllBe(item => ReferenceEquals(item.ItemHeaderForeground, secondHeaderBrush));
            FindPresenter(first, "HeaderPresenter").Foreground.ShouldBeSameAs(secondHeaderBrush);

            steps.ItemHeaderForeground    = null;
            steps.ItemSubHeaderForeground = null;
            steps.ItemRailBackground      = null;
            Dispatcher.UIThread.RunJobs();

            items.ShouldAllBe(item => item.ItemHeaderForeground == null);
            items.ShouldAllBe(item => item.ItemSubHeaderForeground == null);
            items.ShouldAllBe(item => item.ItemRailBackground == null);
            GetColor(FindPresenter(first, "HeaderPresenter").Foreground)
                .ShouldBe(GetColor(GetThemeResource<IBrush>(StepsTokenKind.ProcessTitleColor)));
            GetColor(FindPresenter(first, "SubHeaderPresenter").Foreground)
                .ShouldBe(GetColor(GetThemeResource<IBrush>(StepsTokenKind.ProcessDescriptionColor)));
            GetColor(FindConnector(first).Background)
                .ShouldBe(GetColor(GetThemeResource<IBrush>(StepsTokenKind.WaitTailColor)));
        });
    }

    [Fact]
    public void Direct_Item_Removal_Clears_Semantic_Style_Projection()
    {
        var item = new Desktop.Controls.StepsItem { Header = "A" };
        var source = new ObservableCollection<Desktop.Controls.StepsItem>([item]);
        var steps = new Desktop.Controls.Steps
        {
            Width                   = 640,
            ItemsSource             = source,
            ItemHeaderForeground    = Brushes.Red,
            ItemSubHeaderForeground = Brushes.Green,
            ItemRailBackground      = Brushes.Blue
        };

        ShowInWindow(steps, () =>
        {
            item.ItemHeaderForeground.ShouldBeSameAs(Brushes.Red);
            item.ItemSubHeaderForeground.ShouldBeSameAs(Brushes.Green);
            item.ItemRailBackground.ShouldBeSameAs(Brushes.Blue);

            source.Remove(item);
            Dispatcher.UIThread.RunJobs();

            item.ItemHeaderForeground.ShouldBeNull();
            item.ItemSubHeaderForeground.ShouldBeNull();
            item.ItemRailBackground.ShouldBeNull();
        });
    }

    [Fact]
    public void Generated_Container_Receives_And_Releases_Semantic_Style_Projection()
    {
        var source = new ObservableCollection<string>(["A", "B"]);
        var steps = new Desktop.Controls.Steps
        {
            Width                   = 640,
            ItemsSource             = source,
            ItemHeaderForeground    = Brushes.Red,
            ItemSubHeaderForeground = Brushes.Green,
            ItemRailBackground      = Brushes.Blue
        };

        ShowInWindow(steps, () =>
        {
            var container = steps.ContainerFromIndex(0).ShouldBeOfType<Desktop.Controls.StepsItem>();
            container.ItemHeaderForeground.ShouldBeSameAs(Brushes.Red);
            container.ItemSubHeaderForeground.ShouldBeSameAs(Brushes.Green);
            container.ItemRailBackground.ShouldBeSameAs(Brushes.Blue);

            source.RemoveAt(0);
            Dispatcher.UIThread.RunJobs();

            container.ItemHeaderForeground.ShouldBeNull();
            container.ItemSubHeaderForeground.ShouldBeNull();
            container.ItemRailBackground.ShouldBeNull();
        });
    }

    private static Desktop.Controls.Steps CreateDirectSteps(
        IBrush headerBrush,
        IBrush subHeaderBrush,
        IBrush railBrush)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width                   = 640,
            Current                 = 0,
            ItemHeaderForeground    = headerBrush,
            ItemSubHeaderForeground = subHeaderBrush,
            ItemRailBackground      = railBrush
        };
        steps.Items.Add(new Desktop.Controls.StepsItem
        {
            Header    = "First",
            SubHeader = "Sub first",
            Content   = "Content first"
        });
        steps.Items.Add(new Desktop.Controls.StepsItem
        {
            Header    = "Second",
            SubHeader = "Sub second",
            Content   = "Content second"
        });
        return steps;
    }

    private static ContentPresenter FindPresenter(Desktop.Controls.StepsItem item, string name)
    {
        return item.GetVisualDescendants()
                   .OfType<ContentPresenter>()
                   .Single(control => control.Name == name);
    }

    private static PixelAlignedBorder FindConnector(Desktop.Controls.StepsItem item)
    {
        return item.GetVisualDescendants()
                   .OfType<PixelAlignedBorder>()
                   .Single(control => control.Name == "Connector");
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current.ShouldNotBeNull();
        application.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static Color GetColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        return ((ISolidColorBrush)brush!).Color;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 760,
            Height  = 240,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
