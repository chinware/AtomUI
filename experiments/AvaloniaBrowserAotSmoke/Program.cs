using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;

[assembly: SupportedOSPlatform("browser")]

namespace AvaloniaBrowserAotSmoke;

internal static class Program
{
    public static async Task Main()
    {
        await AppBuilder.Configure<SmokeApplication>()
                        .StartBrowserAppAsync("out");
    }
}

public sealed class SmokeApplication : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ISingleViewApplicationLifetime lifetime)
        {
            lifetime.MainView = new Border
            {
                Background = Brushes.White,
                Padding    = new Thickness(32),
                Child      = new StackPanel
                {
                    Spacing              = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text       = "Avalonia Browser AOT smoke",
                            FontSize   = 28,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = Brushes.Black
                        },
                        new TextBlock
                        {
                            Text       = "Runtime initialized successfully.",
                            FontSize   = 16,
                            Foreground = Brushes.DimGray
                        }
                    }
                }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
