using System;
using System.Linq;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUI.Controls;
using AtomUINumericUpDown = AtomUI.Desktop.Controls.NumericUpDown;
using AtomUIScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.NumericUpDown;

public class NumericUpDownHandleTests
{
    static NumericUpDownHandleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Disabled_NumericUpDown_Hides_Floatable_Spinner_Handle()
    {
        var numericUpDown = new AtomUINumericUpDown
        {
            Width           = 160,
            Value           = 3,
            IsEnabled       = false,
            IsMotionEnabled = false
        };

        ShowInWindow(numericUpDown, () =>
        {
            var spinnerHandle = numericUpDown.GetVisualDescendants()
                                             .OfType<ContentPresenter>()
                                             .Single(item => item.Name == "PART_SpinnerHandle");

            spinnerHandle.Opacity.ShouldBe(0.0);
        });
    }

    [Fact]
    public void Disabled_NumericUpDown_Uses_Disabled_Text_Foreground()
    {
        var numericUpDown = new AtomUINumericUpDown
        {
            Width           = 160,
            Value           = 3,
            IsEnabled       = false,
            IsMotionEnabled = false
        };

        ShowInWindow(numericUpDown, () =>
        {
            var scrollViewer = numericUpDown.GetVisualDescendants()
                                            .OfType<AtomUIScrollViewer>()
                                            .Single(item => item.Name == "ScrollViewer");

            BrushShouldHaveSameColor(
                scrollViewer.Foreground,
                GetThemeResource<IBrush>(SharedTokenKind.ColorTextDisabled));
        });
    }

    [Fact]
    public void Filled_NumericUpDown_Uses_Filled_Spinner_Handle_Background()
    {
        var numericUpDown = new AtomUINumericUpDown
        {
            Width           = 160,
            Value           = 3,
            StyleVariant    = InputControlStyleVariant.Filled,
            IsMotionEnabled = false
        };

        ShowInWindow(numericUpDown, () =>
        {
            var spinnerHandle = numericUpDown.GetVisualDescendants()
                                             .OfType<TemplatedControl>()
                                             .Single(item => item.GetType().Name == "ButtonSpinnerHandle");

            BrushShouldHaveSameColor(
                spinnerHandle.Background,
                GetThemeResource<IBrush>(ButtonSpinnerTokenKind.FilledHandleBg));
        });
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static void BrushShouldHaveSameColor(IBrush? actual, IBrush? expected)
    {
        GetSolidBrushColor(actual).ShouldBe(GetSolidBrushColor(expected));
    }

    private static Color GetSolidBrushColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        brush.ShouldBeAssignableTo<ISolidColorBrush>();
        return ((ISolidColorBrush)brush!).Color;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 120,
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
