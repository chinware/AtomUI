using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Window;

internal partial class LogoVisibilityDemoWindow : AtomUI.Desktop.Controls.Window
{
    public LogoVisibilityDemoWindow()
    {
        InitializeComponent();
    }

    private void HandleLogoVisibilitySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is AtomUI.Desktop.Controls.Segmented segmented)
        {
            LogoVisibility = segmented.SelectedIndex switch
            {
                1 => AtomUI.Desktop.Controls.WindowTitleBarLogoVisibility.Always,
                2 => AtomUI.Desktop.Controls.WindowTitleBarLogoVisibility.Never,
                _ => AtomUI.Desktop.Controls.WindowTitleBarLogoVisibility.Auto
            };
        }
    }
}
