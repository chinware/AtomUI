using AtomUI.Icon;
using AtomUI.Icon.AntDesign;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.Media;

#if DEBUG
using Nlnet.Avalonia.DevTools;
#endif


namespace AtomUI.Demo.Desktop;

class Program
{
   // Initialization code. Don't use any Avalonia, third-party APIs or any
   // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
   // yet and stuff might break.
   [STAThread]
   public static void Main(string[] args) => BuildAvaloniaApp()
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
   
   public static AppBuilder BuildAvaloniaApp()
      => AppBuilder.Configure<App>()
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