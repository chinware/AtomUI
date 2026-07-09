using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.SourceCode;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;

namespace AtomUI.Toolkits.GalleryBase.Controls;

public sealed class GalleryShowCaseCodeDrawerHost : UserControl
{
    public static readonly StyledProperty<object?> PageContentProperty =
        AvaloniaProperty.Register<GalleryShowCaseCodeDrawerHost, object?>(nameof(PageContent));

    private readonly GalleryBaseConfiguration _configuration;
    private readonly ContentPresenter _contentPresenter;
    private readonly Drawer _drawer;
    private GalleryShowCaseCodeDrawerContent? _drawerContent;

    public GalleryShowCaseCodeDrawerHost(GalleryBaseConfiguration configuration)
    {
        _configuration = configuration;
        _contentPresenter = new ContentPresenter();
        _drawer = new Drawer
        {
            Placement = AtomUI.Desktop.Controls.DrawerPlacement.Right,
            DialogSize = new Dimension(736),
            ContentPadding = new Thickness(8, 1, 1, 1),
            IsShowCloseButton = true,
            IsCloseOnMaskClick = true,
            Title = "Source Code"
        };

        AddHandler(ShowCaseItem.SourceCodeRequestedEvent, HandleSourceCodeRequested);
        _drawer.Closed += HandleDrawerClosed;

        var layout = new Grid
        {
            Children =
            {
                _contentPresenter,
                _drawer
            }
        };

        Content = layout;
    }

    public object? PageContent
    {
        get => GetValue(PageContentProperty);
        set => SetValue(PageContentProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _drawer.OpenOn = this;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _drawer.IsOpen = false;
        ClearDrawerContent();
        _drawer.OpenOn = null;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PageContentProperty)
        {
            _contentPresenter.Content = PageContent;
        }
    }

    private void HandleSourceCodeRequested(object? sender, ShowCaseSourceCodeRequestedEventArgs e)
    {
        if (!_configuration.SourceCodeDisplay.CanShowSourceCode ||
            _configuration.SourceCodeDisplay.SnippetProvider is null)
        {
            return;
        }

        var title = e.Title;
        if (_configuration.SourceCodeDisplay.SnippetProvider.TryGetSnippetGroup(e.Key, out var group))
        {
            title = string.IsNullOrWhiteSpace(title) ? group.Title : title;
            SetDrawerContent(new GalleryShowCaseCodeDrawerContent(group, e.Key));
        }
        else
        {
            SetDrawerContent(new GalleryShowCaseCodeDrawerContent(null, e.Key));
        }

        _drawer.Title = string.IsNullOrWhiteSpace(title) ? "Source Code" : title!;
        _drawer.IsOpen = true;
        e.Handled = true;
    }

    private void HandleDrawerClosed(object? sender, EventArgs e)
    {
        ClearDrawerContent();
    }

    private void SetDrawerContent(GalleryShowCaseCodeDrawerContent content)
    {
        ClearDrawerContent();
        _drawerContent = content;
        _drawer.Content = content;
    }

    private void ClearDrawerContent()
    {
        _drawerContent?.Dispose();
        _drawerContent = null;
        _drawer.Content = null;
    }
}
