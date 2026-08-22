using System.Diagnostics;
using AtomUI.Data;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public enum PaginationAlign
{
    Start,
    Center,
    End
}

public partial class Pagination : AbstractPagination
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsShowSizeChangerProperty =
        AvaloniaProperty.Register<Pagination, bool>(nameof(IsShowSizeChanger));

    public static readonly StyledProperty<IReadOnlyList<int>?> PageSizeOptionsProperty =
        AvaloniaProperty.Register<Pagination, IReadOnlyList<int>?>(
            nameof(PageSizeOptions),
            validate: ValidatePageSizeOptions);
    
    public static readonly StyledProperty<bool> IsShowQuickJumperProperty =
        AvaloniaProperty.Register<Pagination, bool>(nameof(IsShowQuickJumper));
    
    public static readonly StyledProperty<bool> IsShowTotalInfoProperty =
        AvaloniaProperty.Register<Pagination, bool>(nameof(IsShowTotalInfo));

    public static readonly StyledProperty<string?> TotalInfoTemplateProperty =
        AvaloniaProperty.Register<Pagination, string?>(nameof(TotalInfoTemplate));
    
    public bool IsShowSizeChanger
    {
        get => GetValue(IsShowSizeChangerProperty);
        set => SetValue(IsShowSizeChangerProperty, value);
    }

    public IReadOnlyList<int>? PageSizeOptions
    {
        get => GetValue(PageSizeOptionsProperty);
        set => SetValue(PageSizeOptionsProperty, value);
    }
    
    public bool IsShowQuickJumper
    {
        get => GetValue(IsShowQuickJumperProperty);
        set => SetValue(IsShowQuickJumperProperty, value);
    }
    
    public bool IsShowTotalInfo
    {
        get => GetValue(IsShowTotalInfoProperty);
        set => SetValue(IsShowTotalInfoProperty, value);
    }
    
    public string? TotalInfoTemplate
    {
        get => GetValue(TotalInfoTemplateProperty);
        set => SetValue(TotalInfoTemplateProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<Pagination, ComboBox?> SizeChangerProperty =
        AvaloniaProperty.RegisterDirect<Pagination, ComboBox?>(nameof(SizeChanger),
            o => o.SizeChanger,
            (o, v) => o.SizeChanger = v);

    internal static readonly DirectProperty<Pagination, QuickJumperBar?> QuickJumperBarProperty =
        AvaloniaProperty.RegisterDirect<Pagination, QuickJumperBar?>(nameof(QuickJumperBar),
            o => o.QuickJumperBar,
            (o, v) => o.QuickJumperBar = v);

    internal static readonly DirectProperty<Pagination, string?> PageTextProperty =
        AvaloniaProperty.RegisterDirect<Pagination, string?>(nameof(PageText),
            o => o.PageText,
            (o, v) => o.PageText = v);

    internal static readonly DirectProperty<Pagination, string?> TotalInfoTextProperty =
        AvaloniaProperty.RegisterDirect<Pagination, string?>(nameof(TotalInfoText),
            o => o.TotalInfoText,
            (o, v) => o.TotalInfoText = v);

    private ComboBox? _sizeChanger;

    internal ComboBox? SizeChanger
    {
        get => _sizeChanger;
        set => SetAndRaise(SizeChangerProperty, ref _sizeChanger, value);
    }

    private QuickJumperBar? _quickJumperBar;

    internal QuickJumperBar? QuickJumperBar
    {
        get => _quickJumperBar;
        set => SetAndRaise(QuickJumperBarProperty, ref _quickJumperBar, value);
    }

    private string? _pageText;

    public string? PageText
    {
        get => _pageText;
        set => SetAndRaise(PageTextProperty, ref _pageText, value);
    }

    private string? _totalInfoText;

    public string? TotalInfoText
    {
        get => _totalInfoText;
        set => SetAndRaise(TotalInfoTextProperty, ref _totalInfoText, value);
    }

    #endregion

    #region 内部协作 API

    internal const int MaxNavItemCount = 11;

    #endregion

    private static readonly int[] DefaultPageSizeOptions = [10, 20, 50, 100];

    private PaginationNav? _paginationNav;
    private PaginationNavItem? _previousPageItem;
    private PaginationNavItem? _nextPageItem;
    private int _nextPushItemIndex = 1;
    private int _selectedNavItemIndex = -1;
    private IDisposable? _sizeChangerDisposable;
    private IDisposable? _quickJumperDisposable;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_paginationNav is not null)
        {
            _paginationNav.ContainerPrepared   -= HandleContainerPrepared;
            _paginationNav.PageNavigateRequest -= HandlePageNavRequest;
        }

        _paginationNav = e.NameScope.Find<PaginationNav>("PART_Nav");
        Debug.Assert(_paginationNav is not null);
        _paginationNav.ContainerPrepared   -= HandleContainerPrepared;
        _paginationNav.PageNavigateRequest -= HandlePageNavRequest;
        _paginationNav.ContainerPrepared   += HandleContainerPrepared;
        _paginationNav.PageNavigateRequest += HandlePageNavRequest;
        if (IsShowQuickJumper)
        {
            SetupQuickJumper();
        }

        if (IsShowSizeChanger)
        {
            SetupSizeChanger();
        }
    }

    private void HandleContainerPrepared(object? sender, ContainerPreparedEventArgs args)
    {
        Debug.Assert(_paginationNav is not null);
        var count = _paginationNav.ItemCount;
        if (args.Container is PaginationNavItem navItem)
        {
            if (0 == args.Index)
            {
                navItem.PaginationItemType = PaginationItemType.Previous;
                _previousPageItem          = navItem;
                _previousPageItem.Icon  = new LeftOutlined();
            }
            else if (count - 1 == args.Index)
            {
                navItem.PaginationItemType = PaginationItemType.Next;
                _nextPageItem              = navItem;
                _nextPageItem.Icon         = new RightOutlined();
            }
            else
            {
                navItem.PaginationItemType = PaginationItemType.PageIndicator;
            }
        }
        TemplateConfigured = true;
        if (HasRealizedContainerCount(_paginationNav, count))
        {
            HandlePageConditionChanged();
        }
    }

    private static bool HasRealizedContainerCount(ItemsControl itemsControl, int expectedCount)
    {
        var realizedCount = 0;
        foreach (var _ in itemsControl.GetRealizedContainers())
        {
            ++realizedCount;
            if (realizedCount > expectedCount)
            {
                return false;
            }
        }

        return realizedCount == expectedCount;
    }

    protected override void NotifyPageConditionChanged(int currentPage, int pageCount, int pageSize, long total)
    {
        if (TemplateConfigured)
        {
            Debug.Assert(_paginationNav != null);
            Debug.Assert(_previousPageItem != null);
            Debug.Assert(_nextPageItem != null);
            var count = _paginationNav.ItemCount;
            // 清空状态 clear state
            _paginationNav.SelectedIndex = -1;
            _selectedNavItemIndex        = -1;
            for (int i = 1; i < count - 1; i++)
            {
                var container = _paginationNav.ContainerFromIndex(i);
                if (container is PaginationNavItem navItem)
                {
                    navItem.PaginationItemType = PaginationItemType.PageIndicator;
                    navItem.IsVisible          = false;
                    navItem.Content            = null;
                }
            }

            _previousPageItem.IsEnabled  = currentPage > 1;
            _previousPageItem.PageNumber = Math.Max(1, CurrentPage - 1);
            _nextPageItem.IsEnabled      = currentPage < pageCount;
            _nextPageItem.PageNumber     = Math.Min(pageCount, CurrentPage + 1);
            _nextPushItemIndex           = 1;

            SetupLeftButtonRange(currentPage, pageCount);
            SetupNextIndicatorNavItem(currentPage, true);
            SetupRightButtonRange(currentPage, pageCount);
            _paginationNav.SelectedIndex = _selectedNavItemIndex;
            SetupTotalInfoText();
        }
        base.NotifyPageConditionChanged(currentPage, pageCount, pageSize, total);
    }

    private void HandlePageNavRequest(object? sender, PageNavRequestArgs args)
    {
        if (args.PageNumber != CurrentPage)
        {
            SetCurrentValue(CurrentPageProperty, args.PageNumber);
        }
    }

    private void SetupLeftButtonRange(int currentPage, int pageCount)
    {
        if (currentPage < 5)
        {
            for (var i = 1; i < currentPage; i++)
            {
                SetupNextIndicatorNavItem(i, false);
            }
        }
        else
        {
            var leftDelta = Math.Max(2, 4 - (pageCount - currentPage));
            var i         = currentPage - leftDelta;
            if (i > 1)
            {
                SetupNextIndicatorNavItem(1, false);
                SetupEllipsisNavItem();
            }

            for (; i < currentPage; i++)
            {
                SetupNextIndicatorNavItem(i, false);
            }
        }
    }

    private void SetupRightButtonRange(int currentPage, int pageCount)
    {
        if (pageCount - currentPage < 4)
        {
            for (var i = currentPage + 1; i <= pageCount; i++)
            {
                SetupNextIndicatorNavItem(i, false);
            }
        }
        else
        {
            var rightDelta = Math.Max(2, 5 - currentPage);
            var i          = currentPage + 1;
            for (; i <= currentPage + rightDelta; i++)
            {
                SetupNextIndicatorNavItem(i, false);
            }

            if (i < pageCount)
            {
                SetupEllipsisNavItem();
                SetupNextIndicatorNavItem(pageCount, false);
            }
        }
    }

    private void SetupNextIndicatorNavItem(int pageIndex, bool isActive)
    {
        if (_nextPushItemIndex == 0 || _nextPushItemIndex == MaxNavItemCount)
        {
            throw new ArgumentException("Invalid next push item index");
        }

        Debug.Assert(_paginationNav != null);
        var navItem = _paginationNav.ContainerFromIndex(_nextPushItemIndex++) as PaginationNavItem;

        if (isActive)
        {
            _selectedNavItemIndex = _nextPushItemIndex - 1;
        }

        Debug.Assert(navItem != null);
        navItem.PageNumber = pageIndex;
        navItem.Content    = $"{pageIndex}";
        navItem.IsVisible  = true;
    }

    private void SetupEllipsisNavItem()
    {
        if (_nextPushItemIndex == 0 || _nextPushItemIndex == MaxNavItemCount)
        {
            throw new ArgumentException("Invalid next push item index");
        }

        Debug.Assert(_paginationNav != null);
        var navItem = _paginationNav.ContainerFromIndex(_nextPushItemIndex++) as PaginationNavItem;
        Debug.Assert(navItem != null);
        navItem.Icon               = new EllipsisOutlined();
        navItem.PaginationItemType = PaginationItemType.Ellipses;
        navItem.IsVisible          = true;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsShowSizeChangerProperty)
            {
                SetupSizeChanger();
            }
            else if (change.Property == IsShowQuickJumperProperty)
            {
                SetupQuickJumper();
            }
        }

        if (change.Property == PageTextProperty ||
            change.Property == PageSizeOptionsProperty)
        {
            SyncSizeChangerItems();
        }
        else if (change.Property == PageSizeProperty)
        {
            SyncSizeChangerSelection();
        }
    }

    private void SyncSizeChangerItems()
    {
        if (SizeChanger != null)
        {
            SizeChanger.SelectionChanged -= HandlePageSizeChanged;
            try
            {
                SizeChanger.SelectedIndex = -1;
                SizeChanger.Items.Clear();
                foreach (var pageSize in GetEffectivePageSizeOptions())
                {
                    SizeChanger.Items.Add(new PageSizeComboBoxItem
                    {
                        Content  = $"{pageSize} / {PageText}",
                        PageSize = pageSize
                    });
                }

                var selectedPageSize = PageSize <= 0 ? DefaultPageSize : PageSize;
                SizeChanger.SelectedIndex = -1;
                if (TryFindSizeChangerItemIndex(selectedPageSize, out var index))
                {
                    SizeChanger.SelectedIndex = index;
                }
            }
            finally
            {
                SizeChanger.SelectionChanged += HandlePageSizeChanged;
            }
        }
    }

    private void SyncSizeChangerSelection()
    {
        if (SizeChanger == null)
        {
            return;
        }

        var selectedPageSize = PageSize <= 0 ? DefaultPageSize : PageSize;
        if (TryFindSizeChangerItemIndex(selectedPageSize, out var index))
        {
            SetSizeChangerSelectedIndex(index);
        }
        else
        {
            SyncSizeChangerItems();
        }
    }

    private bool TryFindSizeChangerItemIndex(int pageSize, out int index)
    {
        index = -1;
        if (SizeChanger == null)
        {
            return false;
        }

        for (var i = 0; i < SizeChanger.Items.Count; i++)
        {
            if (SizeChanger.Items.GetAt(i) is PageSizeComboBoxItem pageSizeItem &&
                pageSizeItem.PageSize == pageSize)
            {
                index = i;
                return true;
            }
        }

        return false;
    }

    private void SetSizeChangerSelectedIndex(int index)
    {
        Debug.Assert(SizeChanger != null);
        if (SizeChanger.SelectedIndex == index)
        {
            return;
        }

        SizeChanger.SelectionChanged -= HandlePageSizeChanged;
        try
        {
            SizeChanger.SelectedIndex = index;
        }
        finally
        {
            SizeChanger.SelectionChanged += HandlePageSizeChanged;
        }
    }

    private IEnumerable<int> GetEffectivePageSizeOptions()
    {
        var selectedPageSize = PageSize <= 0 ? DefaultPageSize : PageSize;
        var pageSizeOptions  = PageSizeOptions ?? DefaultPageSizeOptions;
        var emittedPageSizes = new HashSet<int>();

        if (selectedPageSize > 0 && !pageSizeOptions.Contains(selectedPageSize))
        {
            emittedPageSizes.Add(selectedPageSize);
            yield return selectedPageSize;
        }

        foreach (var pageSize in pageSizeOptions)
        {
            if (emittedPageSizes.Add(pageSize))
            {
                yield return pageSize;
            }
        }
    }

    private void SetupTotalInfoText()
    {
        if (IsShowTotalInfo && TotalInfoTemplate != null)
        {
            TotalInfoText = TotalInfoTemplate.Replace("${Total}", $"{Total}")
                                             .Replace("${RangeStart}", $"{(CurrentPage - 1) * PageSize}")
                                             .Replace("${RangeEnd}", $"{Math.Min(CurrentPage * PageSize, Total)}");
        }
    }

    private void SetupSizeChanger()
    {
        if (!IsShowSizeChanger)
        {
            ClearSizeChanger();
            return;
        }

        if (SizeChanger == null)
        {
            var sizeChanger = new ComboBox();
            sizeChanger.VerticalAlignment = VerticalAlignment.Center;
            _sizeChangerDisposable?.Dispose();
            _sizeChangerDisposable = BindUtils.RelayBind(this, SizeTypeProperty, sizeChanger, ComboBox.SizeTypeProperty);
            SizeChanger                  =  sizeChanger;
            SyncSizeChangerItems();
        }
    }

    private void ClearSizeChanger()
    {
        if (SizeChanger is not null)
        {
            SizeChanger.SelectionChanged -= HandlePageSizeChanged;
            SizeChanger = null;
        }

        _sizeChangerDisposable?.Dispose();
        _sizeChangerDisposable = null;
    }

    private void SetupQuickJumper()
    {
        if (!IsShowQuickJumper)
        {
            ClearQuickJumper();
            return;
        }

        if (QuickJumperBar == null)
        {
            QuickJumperBar = new QuickJumperBar();
            QuickJumperBar.JumpRequest += HandleQuickJumpRequested;
            _quickJumperDisposable?.Dispose();
            _quickJumperDisposable = BindUtils.RelayBind(this, SizeTypeProperty, QuickJumperBar, QuickJumperBar.SizeTypeProperty);
        }
    }

    private void ClearQuickJumper()
    {
        if (QuickJumperBar is not null)
        {
            QuickJumperBar.JumpRequest -= HandleQuickJumpRequested;
            QuickJumperBar = null;
        }

        _quickJumperDisposable?.Dispose();
        _quickJumperDisposable = null;
    }

    private void HandleQuickJumpRequested(object? sender, QuickJumpArgs args)
    {
        var total     = Math.Max(0, Total);
        var pageSize  = PageSize <= 0 ? DefaultPageSize : PageSize;
        var pageCount = (int)Math.Ceiling(total / (double)pageSize);
        SetCurrentValue(CurrentPageProperty, Math.Max(1, Math.Min(pageCount, args.PageNumber)));
    }

    private void HandlePageSizeChanged(object? sender, SelectionChangedEventArgs? args)
    {
        if (args?.AddedItems.Count >= 1 && args.AddedItems[0] is PageSizeComboBoxItem comboBoxItem)
        {
            SetCurrentValue(PageSizeProperty, Math.Max(comboBoxItem.PageSize, 1));
        }
    }

    private static bool ValidatePageSizeOptions(IReadOnlyList<int>? pageSizeOptions)
    {
        return pageSizeOptions is null || pageSizeOptions.All(pageSize => pageSize > 0);
    }
}
