using AtomUI.Toolkits.GalleryBase.SourceCode;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using DesktopTabControl = AtomUI.Desktop.Controls.TabControl;
using DesktopTabItem = AtomUI.Desktop.Controls.TabItem;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class GalleryShowCaseCodeDrawerContent : UserControl, IDisposable
{
    private readonly List<GalleryCodeViewer> _viewers = new();
    private bool _isDisposed;

    public GalleryShowCaseCodeDrawerContent(ShowCaseCodeSnippetGroup? group, ShowCaseCodeSnippetKey key)
    {
        if (group is null || group.Snippets.Count == 0)
        {
            Content = new TextBlock
            {
                Text = $"No source snippet found for {key.ViewTypeName} / {key.PanelKey} / {key.ItemIndex}.",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };
            return;
        }

        var tabs = new DesktopTabControl
        {
            Margin = new Thickness(0),
            ContentPadding = new Thickness(0),
            HeaderStartEdgePadding = 0,
            HeaderEndEdgePadding = 0,
            TabAndContentGutter = 8,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch
        };
        foreach (var snippet in group.Snippets)
        {
            var viewer = new GalleryCodeViewer
            {
                CodeText = snippet.Text,
                Language = snippet.Language,
                ShowLineNumbers = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _viewers.Add(viewer);

            tabs.Items.Add(new DesktopTabItem
            {
                Header = snippet.TabTitle,
                Content = viewer
            });
        }

        tabs.SelectedIndex = 0;
        Content = tabs;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        foreach (var viewer in _viewers)
        {
            viewer.Dispose();
        }

        _viewers.Clear();
    }
}
