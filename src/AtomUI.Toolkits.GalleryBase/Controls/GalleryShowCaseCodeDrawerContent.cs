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
    private readonly Dictionary<DesktopTabItem, ShowCaseCodeSnippet> _pendingSnippets = new();
    private DesktopTabControl? _tabs;
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
            ContentPadding = new Thickness(1),
            HeaderStartEdgePadding = 0,
            HeaderEndEdgePadding = 0,
            TabAndContentGutter = 2,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch
        };
        foreach (var snippet in group.Snippets)
        {
            var tabItem = new DesktopTabItem
            {
                Header = snippet.TabTitle
            };
            _pendingSnippets.Add(tabItem, snippet);
            tabs.Items.Add(tabItem);
        }

        tabs.SelectionChanged += HandleSelectionChanged;
        _tabs = tabs;
        Content = tabs;

        // Materialize the initially selected tab; the rest are created on demand.
        tabs.SelectedIndex = 0;
        EnsureViewerCreated(tabs.SelectedItem as DesktopTabItem);
    }

    private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_tabs is not null)
        {
            EnsureViewerCreated(_tabs.SelectedItem as DesktopTabItem);
        }
    }

    private void EnsureViewerCreated(DesktopTabItem? tabItem)
    {
        if (tabItem is null ||
            !_pendingSnippets.TryGetValue(tabItem, out var snippet))
        {
            return;
        }

        _pendingSnippets.Remove(tabItem);
        var viewer = new GalleryCodeViewer
        {
            CodeText = snippet.Text,
            Language = snippet.Language,
            ShowLineNumbers = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        _viewers.Add(viewer);
        tabItem.Content = viewer;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        if (_tabs is not null)
        {
            _tabs.SelectionChanged -= HandleSelectionChanged;
            _tabs = null;
        }

        _pendingSnippets.Clear();
        foreach (var viewer in _viewers)
        {
            viewer.Dispose();
        }

        _viewers.Clear();
    }
}
