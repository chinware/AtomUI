using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUIGallery.Models;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using ScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;

namespace AtomUIGallery.Controls;

public class IconGallery : TemplatedControl
{
    private const int InitialIconLoadCount = 96;
    private const int IncrementalIconLoadCount = 96;
    private const double LoadMoreScrollThreshold = 240;
    private const string ScrollViewerPart = "PART_ScrollViewer";
    private const string SearchInputPart = "PART_SearchInput";

    public static readonly StyledProperty<IconThemeType?> IconThemeTypeProperty =
        AvaloniaProperty.Register<IconGallery, IconThemeType?>(nameof(IconThemeType));

    public IconThemeType? IconThemeType
    {
        get => GetValue(IconThemeTypeProperty);
        set => SetValue(IconThemeTypeProperty, value);
    }

    #region 内部属性定义

    internal static readonly StyledProperty<AvaloniaList<PackageIconItem>?> IconInfosProperty =
        AvaloniaProperty.Register<IconGallery, AvaloniaList<PackageIconItem>?>(nameof(IconInfos));

    internal AvaloniaList<PackageIconItem>? IconInfos
    {
        get => GetValue(IconInfosProperty);
        set => SetValue(IconInfosProperty, value);
    }

    #endregion

    private readonly List<PackageIconItem> _matchedIconInfos = new();
    private AvaloniaList<PackageIconItem> _activatedIconInfos = new();
    private ScrollViewer? _scrollViewer;
    private SearchEdit? _searchEdit;
    private int _loadedIconCount;
    private bool _templateEventsAttached;

    private bool HasMoreIconInfos => _loadedIconCount < _matchedIconInfos.Count;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DetachTemplateEvents();

        _scrollViewer = e.NameScope.Find<ScrollViewer>(ScrollViewerPart);
        _searchEdit   = e.NameScope.Find<SearchEdit>(SearchInputPart);
        AttachTemplateEvents();

        ReLoadIcons();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AttachTemplateEvents();
        if (_scrollViewer != null &&
            _matchedIconInfos.Count == 0 &&
            (IconInfos is null || IconInfos.Count == 0))
        {
            ReLoadIcons();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DetachTemplateEvents();
        ResetActivatedIconInfos();
        _matchedIconInfos.Clear();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconThemeTypeProperty)
        {
            if (VisualRoot is not null)
            {
                ReLoadIcons();
            }
        }
    }

    private void ReLoadIcons()
    {
        ResetActivatedIconInfos();
        _matchedIconInfos.Clear();

        var iconThemeType = IconThemeType;
        var targetIcons = AntDesignIconCatalog.GetIcons()
                                              .Where(info => iconThemeType is null || info.ThemeType == iconThemeType)
                                              .OrderBy(info => info.Name)
                                              .ToArray();

        var filter = _searchEdit?.Text?.Trim();
        foreach (var iconInfo in targetIcons)
        {
            if (!string.IsNullOrEmpty(filter) &&
                !iconInfo.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            _matchedIconInfos.Add(new PackageIconItem(
                iconInfo.Name,
                iconInfo.IconType,
                iconInfo.Creator));
        }

        LoadMoreIconInfos(InitialIconLoadCount);
    }

    private void HandleScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (_scrollViewer == null || !HasMoreIconInfos)
        {
            return;
        }

        var remainingHeight = _scrollViewer.Extent.Height - _scrollViewer.Viewport.Height - _scrollViewer.Offset.Y;
        if (remainingHeight <= LoadMoreScrollThreshold)
        {
            LoadMoreIconInfos(IncrementalIconLoadCount);
        }
    }

    private void HandleSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        ReLoadIcons();
    }

    private void AttachTemplateEvents()
    {
        if (_templateEventsAttached)
        {
            return;
        }

        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged += HandleScrollChanged;
        }

        if (_searchEdit != null)
        {
            _searchEdit.SearchButtonClick += HandleSearchButtonClick;
        }

        _templateEventsAttached = _scrollViewer != null || _searchEdit != null;
    }

    private void DetachTemplateEvents()
    {
        if (!_templateEventsAttached)
        {
            return;
        }

        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged -= HandleScrollChanged;
        }

        if (_searchEdit != null)
        {
            _searchEdit.SearchButtonClick -= HandleSearchButtonClick;
        }

        _templateEventsAttached = false;
    }

    private void LoadMoreIconInfos(int count)
    {
        if (!HasMoreIconInfos)
        {
            return;
        }

        var targetCount = Math.Min(_loadedIconCount + count, _matchedIconInfos.Count);
        for (var i = _loadedIconCount; i < targetCount; i++)
        {
            var iconInfo = _matchedIconInfos[i];
            iconInfo.Icon = iconInfo.Creator?.Invoke();
            _activatedIconInfos.Add(iconInfo);
        }

        _loadedIconCount = targetCount;
    }

    private void ResetActivatedIconInfos()
    {
        foreach (var iconInfo in _activatedIconInfos)
        {
            iconInfo.Icon = null;
        }

        _loadedIconCount    = 0;
        _activatedIconInfos = new AvaloniaList<PackageIconItem>();
        SetCurrentValue(IconInfosProperty, _activatedIconInfos);
        if (_scrollViewer != null)
        {
            _scrollViewer.Offset = new Vector(0, 0);
        }
    }
}
