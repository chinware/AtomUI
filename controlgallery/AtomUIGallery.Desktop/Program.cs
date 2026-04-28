using Avalonia;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.Desktop;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            LogException(ex);
            throw;
        }
    }

    private static void LogException(Exception ex)
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            
        var logDirectory = Path.Combine(homeDirectory, Path.Combine("AtomUIGallery", "AppCrashLogs"));
        Directory.CreateDirectory(logDirectory);
            
        var logFileName = $"CrashLog_{DateTime.Now:yyyyMMdd_HHmmss}.log";
        var logFilePath = Path.Combine(logDirectory, logFileName);
            
        File.WriteAllText(logFilePath, 
            $"CrashTime: {DateTime.Now}\r\n" +
            $"Exception Type: {ex.GetType().Name}\r\n" +
            $"Exception Message: {ex.Message}\r\n" +
            $"Stack Info: \r\n{ex.StackTrace}");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<GalleryApplication>()
            .UseReactiveUI(build =>
                {})
            // On X11 (Linux) drawn decorations are off by default. Enable them so
            // the WM stops drawing its own title bar / borders and lets our
            // WindowDrawnDecorations theme own the chrome. Win32 and macOS do
            // this automatically when ExtendClientAreaToDecorationsHint=True.
            .With(new X11PlatformOptions
            {
                EnableDrawnDecorations = true
            })
            .UsePlatformDetect()
            .WithDeveloperTools()
            .LogToTrace();
    }
}