using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUIGallery.ShowCases.Splash;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class GallerySplashWindowTests
{
    static GallerySplashWindowTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Dedicated_Window_Owns_Custom_Visuals_And_Inherits_Status_Colors()
    {
        var window = new GallerySplashWindow();
        window.Splash.ShouldBeOfType<GalleryWindowSplash>();

        try
        {
            window.Show();

            window.Splash!.StyleKey.ShouldBe(typeof(AtomUI.Desktop.Controls.Splash));
            window.Splash.Theme.ShouldBeNull();

            var surface = window.GetVisualDescendants()
                                .OfType<Border>()
                                .Single(control => control.Name == "PART_SurfaceLayout");
            var gradient = surface.Background.ShouldBeOfType<LinearGradientBrush>();
            gradient.GradientStops.Select(stop => stop.Color).ShouldBe(
                [Color.Parse("#0B1026"), Color.Parse("#1D39C4"), Color.Parse("#13C2C2")]);

            var title = FindSplashTextBlock(window, "PART_TitleBlock");
            var subtitle = FindSplashTextBlock(window, "PART_SubtitleBlock");
            var message = FindSplashTextBlock(window, "PART_MessageBlock");
            var detail = FindSplashTextBlock(window, "PART_DetailBlock");
            GetSolidBrushColor(title.Foreground).ShouldBe(Colors.White);
            GetSolidBrushColor(subtitle.Foreground).ShouldBe(Color.Parse("#D6E4FF"));
            GetSolidBrushColor(message.Foreground).ShouldBe(Color.Parse("#F5F8FF"));
            GetSolidBrushColor(detail.Foreground).ShouldBe(Color.Parse("#D6E4FF"));

            window.TryFindResource(SplashTokenKind.SuccessColor, out var successResource).ShouldBeTrue();
            window.TryFindResource(SplashTokenKind.ErrorColor, out var errorResource).ShouldBeTrue();

            window.Splash!.Status = SplashStatus.Success;
            Dispatcher.UIThread.RunJobs();
            GetSolidBrushColor(message.Foreground).ShouldBe(GetSolidBrushColor((IBrush?)successResource));

            window.Splash.Status = SplashStatus.Error;
            Dispatcher.UIThread.RunJobs();
            GetSolidBrushColor(message.Foreground).ShouldBe(GetSolidBrushColor((IBrush?)errorResource));
        }
        finally
        {
            window.Close();
        }
    }

    private static AtomUI.Desktop.Controls.TextBlock FindSplashTextBlock(
        GallerySplashWindow window,
        string name)
    {
        return window.GetVisualDescendants()
                     .OfType<AtomUI.Desktop.Controls.TextBlock>()
                     .Single(control => control.Name == name);
    }

    private static Color GetSolidBrushColor(IBrush? brush)
    {
        brush.ShouldBeAssignableTo<ISolidColorBrush>();
        return ((ISolidColorBrush)brush!).Color;
    }
}
