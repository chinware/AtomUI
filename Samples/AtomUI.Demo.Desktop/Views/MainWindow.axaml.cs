using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
// #if DEBUG
//         this.AttachDevTools();
// #endif
    }
}