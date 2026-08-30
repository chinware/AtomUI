using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIOtpLineEdit = AtomUI.Desktop.Controls.OtpLineEdit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.OtpLineEdit;

public class SeparatorGlyphAlignmentTests
{
    static SeparatorGlyphAlignmentTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Presenter_Falls_Back_To_No_Transform_When_Ink_Metrics_Are_Unavailable()
    {
        // headless 测试宿主使用 BareMinimum 桩字体，字形度量退化，
        // presenter 必须回退为不平移，而不是基于无效数据位移。
        var otp = new AtomUIOtpLineEdit
        {
            Length          = 3,
            Separator       = "*",
            IsMotionEnabled = false
        };

        using var _ = Show(otp);

        var presenters = otp.GetVisualDescendants()
                            .OfType<OtpSeparatorPresenter>()
                            .ToArray();
        presenters.ShouldNotBeEmpty();
        foreach (var presenter in presenters)
        {
            presenter.ComputedInkOffsetX.ShouldBe(0);
            presenter.ComputedInkOffsetY.ShouldBe(0);
            presenter.RenderTransform.ShouldBeNull();
        }
    }

    [Fact]
    public void Separator_Container_Stays_Centered_With_Adjacent_Cells()
    {
        var otp = new AtomUIOtpLineEdit
        {
            Length          = 3,
            Separator       = "-",
            IsMotionEnabled = false
        };
        var window = new AvaloniaWindow { Width = 480, Height = 120, Content = otp };
        window.Show();
        otp.ApplyTemplate();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        try
        {
            var cells = otp.GetVisualDescendants()
                           .OfType<OtpLineEditCell>()
                           .OrderBy(static cell => cell.Bounds.X)
                           .ToArray();
            cells.Length.ShouldBe(3);

            var separators = otp.GetVisualDescendants()
                                .OfType<Border>()
                                .Where(static candidate =>
                                    candidate.Classes.Contains("semantic-separator") &&
                                    candidate.IsEffectivelyVisible)
                                .OrderBy(static border => border.Bounds.X)
                                .ToArray();
            separators.Length.ShouldBe(2);

            for (var index = 0; index < separators.Length; index++)
            {
                var leftCellCenter  = CenterInWindow(cells[index], window);
                var rightCellCenter = CenterInWindow(cells[index + 1], window);
                var separatorCenter = CenterInWindow(separators[index], window);

                separatorCenter.Y.ShouldBe(leftCellCenter.Y, 1.0);
                separatorCenter.Y.ShouldBe(rightCellCenter.Y, 1.0);

                var leftCellRight = leftCellCenter.X + cells[index].Bounds.Width / 2;
                var rightCellLeft = rightCellCenter.X - cells[index + 1].Bounds.Width / 2;
                separatorCenter.X.ShouldBe((leftCellRight + rightCellLeft) / 2, 1.0);
            }
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static Point CenterInWindow(Visual visual, Avalonia.Controls.Window window)
    {
        var topLeft = visual.TransformToVisual(window)!.Value.Transform(default);
        return new Point(topLeft.X + visual.Bounds.Width / 2, topLeft.Y + visual.Bounds.Height / 2);
    }

    private static IDisposable Show(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width   = 640,
            Height  = 260,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return new WindowLifetime(window);
    }

    private sealed class WindowLifetime(AvaloniaWindow window) : IDisposable
    {
        public void Dispose()
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
