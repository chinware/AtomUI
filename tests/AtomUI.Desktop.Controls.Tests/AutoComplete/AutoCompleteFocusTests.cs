using System.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AvaloniaTextBox = Avalonia.Controls.TextBox;

namespace AtomUI.Desktop.Controls.Tests.AutoComplete;

public class AutoCompleteFocusTests
{
    static AutoCompleteFocusTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Filled_AutoComplete_Keeps_Focus_Style_After_First_Click()
    {
        var autoComplete = new AtomUI.Desktop.Controls.AutoComplete
        {
            Width           = 240,
            IsMotionEnabled = false,
            StyleVariant    = InputControlStyleVariant.Filled
        };

        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 160,
            Content = autoComplete
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var textBox = autoComplete.GetVisualDescendants()
                                      .OfType<AvaloniaTextBox>()
                                      .Single();
            var contentFrame = autoComplete.GetVisualDescendants()
                                           .OfType<Border>()
                                           .Single(x => x.Name == "PART_ContentFrame");
            var clickPoint = textBox.TranslatePoint(
                new Point(textBox.Bounds.Width / 2, textBox.Bounds.Height / 2),
                window);

            clickPoint.ShouldNotBeNull();

            window.MouseMove(clickPoint.Value);
            window.MouseDown(clickPoint.Value, MouseButton.Left);
            window.MouseUp(clickPoint.Value, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            textBox.IsFocused.ShouldBeTrue();
            autoComplete.IsKeyboardFocusWithin.ShouldBeTrue();
            GetSolidColor(contentFrame.Background).ShouldBe(
                Colors.Transparent,
                "Filled AutoComplete should keep the focus background after the first click.");
            contentFrame.BoxShadow.Count.ShouldBe(
                1,
                "Filled AutoComplete should render the token-defined focus shadow when focused.");
            contentFrame.BoxShadow[0].Spread.ShouldBeGreaterThan(0);
        }
        finally
        {
            window.Close();
        }
    }

    private static Color GetSolidColor(IBrush? brush)
    {
        if (brush is not ISolidColorBrush solidColorBrush)
        {
            throw new InvalidOperationException($"Expected a solid color brush, but got {brush?.GetType().Name ?? "null"}.");
        }

        return solidColorBrush.Color;
    }
}
