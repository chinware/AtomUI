using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.IconControls;

public class IconLoadingAnimationTests
{
    static IconLoadingAnimationTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Loading_Icon_Does_Not_Use_Local_Style_Or_RenderTransform_For_Spin()
    {
        var icon = new LoadingOutlined
        {
            Width            = 24,
            Height           = 24,
            LoadingAnimation = IconAnimation.Spin,
            StrokeBrush      = Brushes.Black,
            FillBrush        = Brushes.Black
        };

        ShowInWindow(icon, _ =>
        {
            icon.Styles.ShouldBeEmpty();
            icon.RenderTransform.ShouldBeNull();
        });
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 120,
            Height  = 120,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(3);
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }
}
