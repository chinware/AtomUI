using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using AtomUITabStrip = AtomUI.Desktop.Controls.TabStrip;
using AtomUITabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUI.Toolkits.GalleryBase.Controls;

public enum GalleryShowCaseTab
{
    Examples,
    SemanticParts
}

public class GalleryShowCaseHost : TemplatedControl
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<GalleryShowCaseHost, object?>(nameof(Header));

    public static readonly StyledProperty<object?> ExamplesContentProperty =
        AvaloniaProperty.Register<GalleryShowCaseHost, object?>(nameof(ExamplesContent));

    public static readonly StyledProperty<IDataTemplate?> SemanticPartsContentTemplateProperty =
        AvaloniaProperty.Register<GalleryShowCaseHost, IDataTemplate?>(nameof(SemanticPartsContentTemplate));

    public static readonly StyledProperty<GalleryShowCaseTab> SelectedTabProperty =
        AvaloniaProperty.Register<GalleryShowCaseHost, GalleryShowCaseTab>(
            nameof(SelectedTab),
            GalleryShowCaseTab.Examples);

    public static readonly StyledProperty<Thickness> StickyContentPaddingProperty =
        GalleryStickyTabsHost.StickyContentPaddingProperty.AddOwner<GalleryShowCaseHost>();

    internal static readonly DirectProperty<GalleryShowCaseHost, object?> NavigationContentProperty =
        AvaloniaProperty.RegisterDirect<GalleryShowCaseHost, object?>(
            nameof(NavigationContent),
            host => host.NavigationContent);

    internal static readonly DirectProperty<GalleryShowCaseHost, object?> ActiveContentProperty =
        AvaloniaProperty.RegisterDirect<GalleryShowCaseHost, object?>(
            nameof(ActiveContent),
            host => host.ActiveContent);

    internal static readonly DirectProperty<GalleryShowCaseHost, bool> HasSemanticPartsProperty =
        AvaloniaProperty.RegisterDirect<GalleryShowCaseHost, bool>(
            nameof(HasSemanticParts),
            host => host.HasSemanticParts);

    internal static readonly DirectProperty<GalleryShowCaseHost, bool> IsSemanticPartsContentMaterializedProperty =
        AvaloniaProperty.RegisterDirect<GalleryShowCaseHost, bool>(
            nameof(IsSemanticPartsContentMaterialized),
            host => host.IsSemanticPartsContentMaterialized);

    private AtomUITabStrip? _tabStrip;
    private Control? _semanticPartsContent;
    private IReadOnlyList<SemanticPartPreview> _semanticPartPreviews = Array.Empty<SemanticPartPreview>();
    private object? _navigationContent;
    private object? _activeContent;
    private bool _hasSemanticParts;
    private bool _isSemanticPartsContentMaterialized;
    private bool _isSynchronizingSelection;

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    [Content]
    public object? ExamplesContent
    {
        get => GetValue(ExamplesContentProperty);
        set => SetValue(ExamplesContentProperty, value);
    }

    public IDataTemplate? SemanticPartsContentTemplate
    {
        get => GetValue(SemanticPartsContentTemplateProperty);
        set => SetValue(SemanticPartsContentTemplateProperty, value);
    }

    public GalleryShowCaseTab SelectedTab
    {
        get => GetValue(SelectedTabProperty);
        set => SetValue(SelectedTabProperty, value);
    }

    public Thickness StickyContentPadding
    {
        get => GetValue(StickyContentPaddingProperty);
        set => SetValue(StickyContentPaddingProperty, value);
    }

    internal object? NavigationContent
    {
        get => _navigationContent;
        private set => SetAndRaise(NavigationContentProperty, ref _navigationContent, value);
    }

    internal object? ActiveContent
    {
        get => _activeContent;
        private set => SetAndRaise(ActiveContentProperty, ref _activeContent, value);
    }

    internal bool HasSemanticParts
    {
        get => _hasSemanticParts;
        private set => SetAndRaise(HasSemanticPartsProperty, ref _hasSemanticParts, value);
    }

    internal bool IsSemanticPartsContentMaterialized
    {
        get => _isSemanticPartsContentMaterialized;
        private set => SetAndRaise(
            IsSemanticPartsContentMaterializedProperty,
            ref _isSemanticPartsContentMaterialized,
            value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SemanticPartsContentTemplateProperty)
        {
            ReleaseSemanticPartsContent();
            RebuildNavigation();
            UpdateActiveContent();
        }
        else if (change.Property == ExamplesContentProperty ||
                 change.Property == SelectedTabProperty)
        {
            SynchronizeTabStripSelection();
            UpdateActiveContent();
        }
        else if (change.Property == DataContextProperty)
        {
            SynchronizeContentDataContext();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_tabStrip is null && SemanticPartsContentTemplate is not null)
        {
            RebuildNavigation();
        }
        UpdateActiveContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ReleaseSemanticPartsContent();
        ReleaseNavigation();
        base.OnDetachedFromVisualTree(e);
    }

    private void RebuildNavigation()
    {
        ReleaseNavigation();

        HasSemanticParts = SemanticPartsContentTemplate is not null;
        if (!HasSemanticParts)
        {
            if (SelectedTab != GalleryShowCaseTab.Examples)
            {
                SetCurrentValue(SelectedTabProperty, GalleryShowCaseTab.Examples);
            }
            return;
        }

        _tabStrip = new AtomUITabStrip
        {
            SelectedIndex = SelectedTab == GalleryShowCaseTab.SemanticParts ? 1 : 0,
            Items =
            {
                CreateLocalizedTabItem(SemanticPartPreviewLangResourceKind.ExamplesTabLabel),
                CreateLocalizedTabItem(SemanticPartPreviewLangResourceKind.SemanticPartsTabLabel)
            }
        };
        _tabStrip.SelectionChanged += HandleTabSelectionChanged;
        NavigationContent = _tabStrip;
    }

    private void ReleaseNavigation()
    {
        if (_tabStrip is not null)
        {
            _tabStrip.SelectionChanged -= HandleTabSelectionChanged;
            _tabStrip = null;
        }

        NavigationContent = null;
    }

    private static AtomUITabStripItem CreateLocalizedTabItem(SemanticPartPreviewLangResourceKind resourceKind)
    {
        var item = new AtomUITabStripItem();
        item.Bind(ContentControl.ContentProperty, new DynamicResourceExtension(resourceKind));
        return item;
    }

    private void HandleTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSynchronizingSelection || _tabStrip is null)
        {
            return;
        }

        SetCurrentValue(
            SelectedTabProperty,
            _tabStrip.SelectedIndex == 1
                ? GalleryShowCaseTab.SemanticParts
                : GalleryShowCaseTab.Examples);
    }

    private void SynchronizeTabStripSelection()
    {
        if (_tabStrip is null)
        {
            return;
        }

        var selectedIndex = SelectedTab == GalleryShowCaseTab.SemanticParts ? 1 : 0;
        if (_tabStrip.SelectedIndex == selectedIndex)
        {
            return;
        }

        _isSynchronizingSelection = true;
        try
        {
            _tabStrip.SelectedIndex = selectedIndex;
        }
        finally
        {
            _isSynchronizingSelection = false;
        }
    }

    private void UpdateActiveContent()
    {
        if (SelectedTab == GalleryShowCaseTab.SemanticParts && HasSemanticParts)
        {
            ActiveContent = EnsureSemanticPartsContent();
            foreach (var preview in _semanticPartPreviews)
            {
                preview.ActivatePreview();
            }
        }
        else
        {
            foreach (var preview in _semanticPartPreviews)
            {
                preview.DeactivatePreview();
            }
            ActiveContent = ExamplesContent;
        }
    }

    private Control? EnsureSemanticPartsContent()
    {
        if (_semanticPartsContent is not null)
        {
            SynchronizeContentDataContext();
            return _semanticPartsContent;
        }

        var template = SemanticPartsContentTemplate;
        if (template is null)
        {
            return null;
        }

        var content = template.Build(DataContext);
        if (content is null)
        {
            throw new InvalidOperationException(
                $"{nameof(SemanticPartsContentTemplate)} must build a Control containing at least one " +
                $"{nameof(SemanticPartPreview)}.");
        }

        var previews = content is SemanticPartPreview preview
            ? [preview]
            : content.GetVisualDescendants().OfType<SemanticPartPreview>().ToArray();
        if (previews.Length == 0)
        {
            throw new InvalidOperationException(
                $"{nameof(SemanticPartsContentTemplate)} must build a Control containing at least one " +
                $"{nameof(SemanticPartPreview)}.");
        }

        _semanticPartsContent = content;
        _semanticPartPreviews = previews;
        _semanticPartsContent.DataContext = DataContext;
        IsSemanticPartsContentMaterialized = true;
        return _semanticPartsContent;
    }

    private void SynchronizeContentDataContext()
    {
        if (ExamplesContent is Control examplesControl)
        {
            examplesControl.DataContext = DataContext;
        }

        if (_semanticPartsContent is not null)
        {
            _semanticPartsContent.DataContext = DataContext;
        }
    }

    private void ReleaseSemanticPartsContent()
    {
        if (_semanticPartsContent is null)
        {
            return;
        }

        foreach (var preview in _semanticPartPreviews)
        {
            preview.DeactivatePreview();
        }
        if (ReferenceEquals(ActiveContent, _semanticPartsContent))
        {
            ActiveContent = ExamplesContent;
        }

        foreach (var preview in _semanticPartPreviews)
        {
            preview.Dispose();
        }
        _semanticPartsContent = null;
        _semanticPartPreviews = Array.Empty<SemanticPartPreview>();
        IsSemanticPartsContentMaterialized = false;
    }
}
