using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Window;

internal partial class TitleAlignmentDemoWindow : AtomUI.Desktop.Controls.Window
{
    public TitleAlignmentDemoWindow()
    {
        InitializeComponent();
    }

    private void HandleAlignmentSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is AtomUI.Desktop.Controls.Segmented segmented)
        {
            TitleAlignment = segmented.SelectedIndex switch
            {
                1 => AtomUI.Desktop.Controls.WindowTitleBarTitleAlignment.Left,
                2 => AtomUI.Desktop.Controls.WindowTitleBarTitleAlignment.Center,
                3 => AtomUI.Desktop.Controls.WindowTitleBarTitleAlignment.WindowCenter,
                4 => AtomUI.Desktop.Controls.WindowTitleBarTitleAlignment.Right,
                _ => AtomUI.Desktop.Controls.WindowTitleBarTitleAlignment.Auto
            };
        }
    }
}
