namespace AtomUIGallery.ShowCases.Window;

internal partial class TitleBarlessDemoWindow : AtomUI.Desktop.Controls.Window
{
    public TitleBarlessDemoWindow()
    {
        InitializeComponent();
    }

    private void HandleMinimizeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = Avalonia.Controls.WindowState.Minimized;
    }

    private void HandleToggleMaximizeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState == Avalonia.Controls.WindowState.Maximized
            ? Avalonia.Controls.WindowState.Normal
            : Avalonia.Controls.WindowState.Maximized;
    }

    private void HandleCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}
