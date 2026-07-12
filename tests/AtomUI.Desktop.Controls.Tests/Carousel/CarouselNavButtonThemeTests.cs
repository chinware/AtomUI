using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using AtomUI.Theme.Styling;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Carousel;

public class CarouselNavButtonThemeTests
{
    static CarouselNavButtonThemeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Navigation_Button_Preserves_Bright_Icon_Brush_Across_Pointer_States()
    {
        var button = new CarouselNavButton
        {
            IsMotionEnabled = false
        };
        var host = new AvaloniaWindow
        {
            Width   = 200,
            Height  = 100,
            Content = button
        };

        try
        {
            host.Show();
            button.ApplyTemplate();
            host.UpdateLayout();

            var expectedBrush = GetThemeResource<IBrush>(SharedTokenKind.ColorBgContainer);

            BrushShouldHaveSameColor(button.IconBrush, expectedBrush);
            button.Opacity.ShouldBe(0.2);

            ((IPseudoClasses)button.Classes).Set(":pointerover", true);
            Dispatcher.UIThread.RunJobs();

            BrushShouldHaveSameColor(button.IconBrush, expectedBrush);
            button.Opacity.ShouldBe(1.0);

            ((IPseudoClasses)button.Classes).Set(":pressed", true);
            Dispatcher.UIThread.RunJobs();

            BrushShouldHaveSameColor(button.IconBrush, expectedBrush);
            button.Opacity.ShouldBe(1.0);
        }
        finally
        {
            host.Close();
        }
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
}
