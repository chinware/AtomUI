using Avalonia.Markup.Xaml;

namespace AtomUI.Desktop.Controls.TestApp;

public partial class MainWindow : AtomUI.Desktop.Controls.Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
