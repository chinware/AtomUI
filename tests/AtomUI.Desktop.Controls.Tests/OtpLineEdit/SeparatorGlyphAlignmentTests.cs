using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIOtpLineEdit = AtomUI.Desktop.Controls.OtpLineEdit;

namespace AtomUI.Desktop.Controls.Tests.OtpLineEdit;

public class SeparatorGlyphAlignmentTests
{
    static SeparatorGlyphAlignmentTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Compensation_Scales_With_Font_Size()
    {
        var host = new ContentControl { FontSize = 14 };
        OtpSeparatorGlyphAlignment.SetX(host, 0.10);
        OtpSeparatorGlyphAlignment.SetY(host, 0.21);

        var transform = host.RenderTransform.ShouldBeOfType<TranslateTransform>();
        transform.X.ShouldBe(1.4, 0.001);
        transform.Y.ShouldBe(2.94, 0.001);

        host.FontSize = 16;
        Dispatcher.UIThread.RunJobs();
        transform.X.ShouldBe(1.6, 0.001);
        transform.Y.ShouldBe(3.36, 0.001);
    }

    [Fact]
    public void Zero_Compensation_Removes_Transform()
    {
        var host = new ContentControl { FontSize = 14 };
        OtpSeparatorGlyphAlignment.SetX(host, 0.0);
        OtpSeparatorGlyphAlignment.SetY(host, 0.0);

        host.RenderTransform.ShouldBeNull();
    }

    [Fact]
    public void Gallery_Separator_Receives_Token_Driven_Compensation()
    {
        var otp = new AtomUIOtpLineEdit
        {
            Length = 2,
            Separator = "*",
            IsMotionEnabled = false
        };
        var window = new Avalonia.Controls.Window { Width = 480, Height = 120, Content = otp };
        window.Show();
        otp.ApplyTemplate();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        try
        {
            var separator = otp.GetVisualDescendants()
                               .OfType<ContentControl>()
                               .First(control => control.Name == "PART_SeparatorPresenter");
            var transform = separator.RenderTransform.ShouldBeOfType<TranslateTransform>();
            transform.Y.ShouldBeGreaterThan(0);
            var token = OtpSeparatorGlyphAlignment.GetX(separator);
            transform.X.ShouldBe(separator.FontSize * token, 0.001);
        }
        finally
        {
            window.Close();
        }
    }
}
