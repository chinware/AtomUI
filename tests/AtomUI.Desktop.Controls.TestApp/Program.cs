using AtomUI;
using Avalonia;

namespace AtomUI.Desktop.Controls.TestApp;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<TestApplication>()
                         .UseAtomUIPlatformDetect()
                         .WithAtomUIDefaultOptions()
                         .LogToTrace();
    }
}
