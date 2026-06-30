using System;
using System.Linq;
using System.Reflection;
using AtomUI;
using AtomUI.Controls.Commons;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUISpinIndicator = AtomUI.Desktop.Controls.SpinIndicator;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Spin;

public class SpinIndicatorCustomSizeTests
{
    static SpinIndicatorCustomSizeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Custom_SizeType_Defaults_To_Middle_Metrics()
    {
        var middle = new AtomUISpinIndicator
        {
            SizeType = CustomizableSizeType.Middle
        };
        var custom = new AtomUISpinIndicator
        {
            SizeType = CustomizableSizeType.Custom
        };
        var host = new StackPanel
        {
            Children =
            {
                middle,
                custom
            }
        };

        ShowInWindow(host, () =>
        {
            custom.IndicatorSize.ShouldBe(middle.IndicatorSize);
            custom.DotSize.ShouldBe(middle.DotSize);
            FindBuiltInLayout(custom).Width.ShouldBe(FindBuiltInLayout(middle).Width);
            FindDots(custom).First().Width.ShouldBe(FindDots(middle).First().Width);
        });
    }

    [Fact]
    public void Custom_SizeType_Local_Metrics_Override_Defaults()
    {
        var indicator = new AtomUISpinIndicator
        {
            SizeType      = CustomizableSizeType.Custom,
            IndicatorSize = 44,
            DotSize       = 14
        };

        ShowInWindow(indicator, () =>
        {
            indicator.IndicatorSize.ShouldBe(44);
            indicator.DotSize.ShouldBe(14);

            var layout = FindBuiltInLayout(indicator);
            layout.Width.ShouldBe(44);
            layout.Height.ShouldBe(44);

            foreach (var dot in FindDots(indicator))
            {
                dot.Width.ShouldBe(14);
                dot.Height.ShouldBe(14);
            }
        });
    }

    [Fact]
    public void BuiltIn_Indicator_Template_Parts_Survive_ContentHost_Detach_And_Reattach()
    {
        var indicator = new AtomUISpinIndicator();
        var host = new ContentControl
        {
            Content = indicator
        };
        var replacement = new Border();

        ShowInWindow(host, () =>
        {
            var initialLayout = GetBuiltInLayoutField(indicator);
            initialLayout.ShouldNotBeNull();

            host.Content = replacement;
            Dispatcher.UIThread.RunJobs();

            host.Content = indicator;
            Dispatcher.UIThread.RunJobs();

            GetBuiltInLayoutField(indicator).ShouldBeSameAs(initialLayout);
        });
    }

    private static Control FindBuiltInLayout(Control control)
    {
        var layout = control.GetVisualDescendants()
                            .OfType<Control>()
                            .SingleOrDefault(item => item.Name == "BuiltInIndicatorLayout");
        layout.ShouldNotBeNull();
        return layout!;
    }

    private static Ellipse[] FindDots(Control control)
    {
        var dots = control.GetVisualDescendants().OfType<Ellipse>().ToArray();
        dots.Length.ShouldBe(4);
        return dots;
    }

    private static Control? GetBuiltInLayoutField(AtomUISpinIndicator indicator)
    {
        var field = typeof(AbstractSpinIndicator).GetField(
            "_builtInIndicatorLayout",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return (Control?)field!.GetValue(indicator);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 320,
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
