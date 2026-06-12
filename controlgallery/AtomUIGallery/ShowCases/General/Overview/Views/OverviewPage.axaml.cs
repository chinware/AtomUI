using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUIGallery.ShowCases.Overview;

public partial class OverviewPage : GalleryReactiveUserControl<OverviewViewModel>
{
    public const string LanguageId = nameof(OverviewPage);

    public OverviewPage()
    {
        InitializeComponent();
    }

    private async void HandleCopyInstallCommandClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Avalonia.Controls.Button { Tag: string command } || string.IsNullOrWhiteSpace(command))
        {
            return;
        }

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null && this.IsAttachedToVisualTree())
        {
            await clipboard.SetTextAsync(command);
        }
    }
}
