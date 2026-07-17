using System;
using System.Linq;
using AtomUI.Controls.Primitives;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsConnectorTests
{
    static StepsConnectorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(StepsItemStatus.Wait)]
    [InlineData(StepsItemStatus.Process)]
    [InlineData(StepsItemStatus.Finish)]
    [InlineData(StepsItemStatus.Error)]
    public void Default_Horizontal_Connector_Is_Visible_And_Colored(StepsItemStatus status)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width          = 760,
            Style          = StepsStyle.Default,
            Orientation    = Orientation.Horizontal,
            LabelPlacement = Orientation.Horizontal
        };
        steps.Items.Add(new StepsItem
        {
            Header = "Step 1",
            Status = status
        });
        steps.Items.Add(new StepsItem
        {
            Header = "Step 2"
        });
        steps.Items.Add(new StepsItem
        {
            Header = "Step 3"
        });

        ShowInWindow(steps, () =>
        {
            var items = steps.GetVisualDescendants()
                             .OfType<StepsItem>()
                             .ToList();
            var connector = FindConnector(items[0]);
            var lastConnector = FindConnector(items[^1]);

            connector.IsVisible.ShouldBeTrue();
            connector.Bounds.Width.ShouldBeGreaterThan(0);
            connector.BorderThickness.Bottom.ShouldBeGreaterThan(0);
            connector.BorderBrush.ShouldNotBeNull(
                $"the {status} connector should resolve its status-specific Steps tail color.");
            if (status is not StepsItemStatus.Finish)
            {
                GetSolidBrushColor(connector.BorderBrush).ShouldBe(
                    GetSolidBrushColor(GetThemeResource<IBrush>(SharedTokenKind.ColorTextDisabled)),
                    $"the non-finish {status} tail should use Ant Design's disabled-text gray.");
            }
            lastConnector.IsVisible.ShouldBeFalse();
        });
    }

    private static PixelAlignedBorder FindConnector(StepsItem item)
    {
        return item.GetVisualDescendants()
                   .OfType<PixelAlignedBorder>()
                   .Single(control => control.Name == "SeparatorLine");
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static Color GetSolidBrushColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        return brush.ShouldBeAssignableTo<ISolidColorBrush>().Color;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 900,
            Height  = 220,
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
}
