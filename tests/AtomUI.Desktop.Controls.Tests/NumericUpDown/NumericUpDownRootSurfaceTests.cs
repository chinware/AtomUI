using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUINumericUpDown = AtomUI.Desktop.Controls.NumericUpDown;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.NumericUpDown;

public class NumericUpDownRootSurfaceTests
{
    static NumericUpDownRootSurfaceTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Root_BorderBrush_Relves_Onto_The_Input_Frame()
    {
        var borderBrush = new SolidColorBrush(Colors.MediumPurple);
        var numericUpDown = new AtomUINumericUpDown
        {
            Width = 320,
            Value = 66m,
            BorderBrush = borderBrush
        };

        var window = Show(numericUpDown);
        try
        {
            FrameOf(numericUpDown).BorderBrush.ShouldBe(borderBrush);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Owner_Scoped_BorderBrush_Style_Reaches_The_Input_Frame()
    {
        var borderBrush = new SolidColorBrush(Colors.RoyalBlue);
        var numericUpDown = new AtomUINumericUpDown
        {
            Width = 320,
            Value = 66m,
            Classes = { "semantic-object" }
        };
        var ownerStyle = new Style(selector => selector.OfType<AtomUINumericUpDown>().Class("semantic-object"));
        ownerStyle.Setters.Add(new Setter(AtomUINumericUpDown.BorderBrushProperty, borderBrush));
        numericUpDown.Styles.Add(ownerStyle);

        var window = Show(numericUpDown);
        try
        {
            FrameOf(numericUpDown).BorderBrush.ShouldBe(borderBrush);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Clearing_The_Root_BorderBrush_Restores_The_Frame_State_Machine()
    {
        var borderBrush = new SolidColorBrush(Colors.MediumPurple);
        var numericUpDown = new AtomUINumericUpDown
        {
            Width = 320,
            Value = 66m,
            BorderBrush = borderBrush
        };

        var window = Show(numericUpDown);
        try
        {
            var frame = FrameOf(numericUpDown);
            frame.BorderBrush.ShouldBe(borderBrush);

            numericUpDown.BorderBrush = null;
            Dispatcher.UIThread.RunJobs();

            frame.BorderBrush.ShouldNotBe(borderBrush);
            frame.BorderBrush.ShouldNotBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    private static ButtonSpinnerDecoratedBox FrameOf(AtomUINumericUpDown numericUpDown)
    {
        return numericUpDown.GetVisualDescendants()
                            .OfType<ButtonSpinnerDecoratedBox>()
                            .Single();
    }

    private static AvaloniaWindow Show(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width = 480,
            Height = 160,
            Content = content
        };
        window.Show();
        content.ApplyTemplate();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}
