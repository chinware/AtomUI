using AtomUI.Icon;
using AtomUI.Icon.AntDesign;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.Media;
#if DEBUG
using Nlnet.Avalonia.DevTools;
#endif

namespace AtomUI.Demo.Desktop;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .With(new FontManagerOptions
                {
                    FontFallbacks = new[]
                    {
                    new FontFallback
                    {
                        FontFamily = new FontFamily("Microsoft YaHei")
                    }
                    }
                })
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.log", ex.ToString());
#if DEBUG
            throw;
#endif
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .ConfigureAtomUI()
                     .UseManagedSystemDialogs()
                     .UsePlatformDetect()
                     .UseAtomUI()
#if DEBUG
                     .UseDevToolsForAvalonia()
#endif
                     .UseIconPackage<AntDesignIconPackage>(true)
                     .With(new Win32PlatformOptions())
                     .LogToTrace();
}