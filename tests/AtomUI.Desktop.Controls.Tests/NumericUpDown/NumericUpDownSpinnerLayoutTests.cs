using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
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

    [Fact]
    public void Suffix_Group_Slides_Left_With_The_Floating_Handle_Shift()
    {
        var numericUpDown = new AtomUINumericUpDown
        {
            Width = 320,
            Mode = NumericUpDownMode.Input,
            Value = 3m,
            InnerRightContent = "kg",
            IsMotionEnabled = false
        };

        var window = new AvaloniaWindow { Width = 480, Height = 120, Content = numericUpDown };
        window.Show();
        numericUpDown.ApplyTemplate();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        try
        {
            var suffix = numericUpDown.GetVisualDescendants()
                                      .OfType<StackPanel>()
                                      .Single(control => control.Classes.Contains("semantic-suffix"));
            var decoratedBox = numericUpDown.GetVisualDescendants()
                                            .OfType<ButtonSpinnerDecoratedBox>()
                                            .Single();
            var originX = suffix.TransformToVisual(numericUpDown).ShouldNotBeNull()
                                 .Transform(new Point(0, 0)).X;

            // Motion is disabled, so the hover state settles the suffix shift synchronously.
            decoratedBox.IsSpinnerContentHover = true;
            Dispatcher.UIThread.RunJobs();

            var shiftedX = suffix.TransformToVisual(numericUpDown).ShouldNotBeNull()
                                  .Transform(new Point(0, 0)).X;
            (originX - shiftedX).ShouldBeGreaterThan(0d);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Suffix_Group_Keeps_A_Visible_Gap_From_The_Spinner_Action_Divider()
    {
        var numericUpDown = new AtomUINumericUpDown
        {
            Width = 320,
            Mode = NumericUpDownMode.Spinner,
            Minimum = 1,
            Maximum = 100,
            Value = 10m,
            IsAllowClear = true,
            InnerLeftContent = "$",
            InnerRightContent = "kg",
            IsMotionEnabled = false
        };

        var window = new AvaloniaWindow { Width = 480, Height = 120, Content = numericUpDown };
        window.Show();
        numericUpDown.ApplyTemplate();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        try
        {
            var suffix = numericUpDown.GetVisualDescendants()
                                      .OfType<StackPanel>()
                                      .Single(control => control.Classes.Contains("semantic-suffix"));
            var segmentFrames = numericUpDown.GetVisualDescendants()
                                             .OfType<Layoutable>()
                                             .Where(control => control.Name == "Frame")
                                             .Select(control => (Layout: control,
                                                 X: control.TransformToVisual(numericUpDown).ShouldNotBeNull()
                                                     .Transform(new Point(0, 0)).X))
                                             .ToArray();
            segmentFrames.ShouldNotBeEmpty();
            var increaseDividerX = segmentFrames.Max(static entry => entry.X);

            var suffixRight = suffix.TransformToVisual(numericUpDown).ShouldNotBeNull()
                                    .Transform(new Point(0, 0)).X + suffix.Bounds.Width;
            (increaseDividerX - suffixRight).ShouldBeGreaterThanOrEqualTo(6d);
        }
        finally
        {
            window.Close();
        }
    }
}
