using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUINumericUpDown = AtomUI.Desktop.Controls.NumericUpDown;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.NumericUpDown;

public class NumericUpDownSpinnerLayoutTests
{
    static NumericUpDownSpinnerLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(NumericUpDownMode.Spinner)]
    [InlineData(NumericUpDownMode.Input)]
    public void Text_Input_Frame_Fills_The_Embedded_Text_Box(NumericUpDownMode mode)
    {
        var numericUpDown = new AtomUINumericUpDown
        {
            Width = 150,
            Mode = mode,
            Minimum = 1,
            Maximum = 10,
            Value = 3m,
            IsMotionEnabled = false
        };

        var window = new AvaloniaWindow { Width = 320, Height = 120, Content = numericUpDown };
        window.Show();
        numericUpDown.ApplyTemplate();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        try
        {
            var textBox = numericUpDown.GetVisualDescendants()
                                       .OfType<EmbeddedTextBox>()
                                       .Single();
            var frame = numericUpDown.GetVisualDescendants()
                                     .OfType<InputControlFrame>()
                                     .Single(control => control.Name == "PART_InputControlFrame");

            frame.Bounds.Height.ShouldBe(textBox.Bounds.Height, 0.5);
            var frameCenter = frame.TransformToVisual(textBox).ShouldNotBeNull()
                                       .Transform(new Point(0, 0)).Y + frame.Bounds.Height / 2;
            frameCenter.ShouldBe(textBox.Bounds.Height / 2, 0.5);
        }
        finally
        {
            window.Close();
        }
    }
}
