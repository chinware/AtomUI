using System.Diagnostics;
using AtomUI.Data;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum PaginationAlign
{
    Start,
    Center,
    End
}

public class Pagination : TemplatedControl,
                          ISizeTypeAware,
                          IMotionAwareControl,
                          IControlSharedTokenResourcesHost
{
    public const int DefaultPageSize = 10;
    public const int DefaultCurrentPage = 1;
    internal const int MaxNavItemCount = 11;
    
    #region 公共属性定义
    
    public static readonly StyledProperty<PaginationAlign> AlignProperty =
        AvaloniaProperty.Register<Button, PaginationAlign>(nameof(PaginationAlign));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Pagination>();
    
    public static readonly DirectProperty<Pagination, int> CurrentPageProperty =
        AvaloniaProperty.RegisterDirect<Pagination, int>(nameof(CurrentPage),
            o => o.CurrentPage,
            (o, v) => o.CurrentPage = v,
            unsetValue: DefaultCurrentPage);
    
    public static readonly DirectProperty<Pagination, int> PageSizeProperty =
        AvaloniaProperty.RegisterDirect<Pagination, int>(nameof(PageSize),
            o => o.PageSize,
            (o, v) => o.PageSize = v,
            unsetValue: DefaultPageSize);
    
    public static readonly DirectProperty<Pagination, long> TotalProperty =
        AvaloniaProperty.RegisterDirect<Pagination, long>(nameof(Total),
            o => o.Total,
            (o, v) => o.Total = v);
    
    public static readonly DirectProperty<Pagination, bool> HideOnSinglePageProperty =
        AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(HideOnSinglePage),
            o => o.HideOnSinglePage,
            (o, v) => o.HideOnSinglePage = v);
    
    public static readonly DirectProperty<Pagination, bool> ShowSizeChangerProperty =
        AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(ShowSizeChanger),
            o => o.ShowSizeChanger,
            (o, v) => o.ShowSizeChanger = v);
    
    public static readonly DirectProperty<Pagination, bool> ShowTotalInfoProperty =
        AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(ShowTotalInfo),
            o => o.ShowTotalInfo,
            (o, v) => o.ShowTotalInfo = v);
    
    public static readonly StyledProperty<IDataTemplate?> TotalInfoTemplateProperty =
        AvaloniaProperty.Register<Button, IDataTemplate?>(nameof(TotalInfoTemplate));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = WaveSpiritAwareControlProperty.IsMotionEnabledProperty.AddOwner<CheckBox>();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public PaginationAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }
    
    private int _currentPage = DefaultCurrentPage;
    public int CurrentPage
    {
        get => _currentPage;
        set => SetAndRaise(CurrentPageProperty, ref _currentPage, value);
    }
    
    private int _pageSize = DefaultPageSize;
    public int PageSize
    {
        get => _pageSize;
        set => SetAndRaise(PageSizeProperty, ref _pageSize, value);
    }
    
    private long _total;
    public long Total
    {
        get => _total;
        set => SetAndRaise(TotalProperty, ref _total, value);
    }
    
    private bool _hideOnSinglePage;
    public bool HideOnSinglePage
    {
        get => _hideOnSinglePage;
        set => SetAndRaise(HideOnSinglePageProperty, ref _hideOnSinglePage, value);
    }
    
    private bool _showSizeChanger;
    public bool ShowSizeChanger
    {
        get => _showSizeChanger;
        set => SetAndRaise(ShowSizeChangerProperty, ref _showSizeChanger, value);
    }
    
    private bool _showTotalInfo;
    public bool ShowTotalInfo
    {
        get => _showTotalInfo;
        set => SetAndRaise(ShowTotalInfoProperty, ref _showTotalInfo, value);
    }
    
    public IDataTemplate? TotalInfoTemplate
    {
        get => GetValue(TotalInfoTemplateProperty);
        set => SetValue(TotalInfoTemplateProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    #endregion
    
    #region 内部属性定义
    
    public static readonly DirectProperty<Pagination, ComboBox?> SizeChangerProperty =
        AvaloniaProperty.RegisterDirect<Pagination, ComboBox?>(nameof(SizeChanger),
            o => o.SizeChanger,
            (o, v) => o.SizeChanger = v);
    
    private ComboBox? _sizeChanger;
    public ComboBox? SizeChanger
    {
        get => _sizeChanger;
        set => SetAndRaise(SizeChangerProperty, ref _sizeChanger, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => PaginationToken.ID;
    #endregion

    private PaginationNav? _paginationNav;
    private PaginationNavItem? _previousPageItem;
    private PaginationNavItem? _nextPageItem;
    private int _nextPushItemIndex = 1;
    private int _selectedNavItemIndex = -1;

    static Pagination()
    {
        AffectsMeasure<Button>(SizeTypeProperty);
    }

    public Pagination()
    {
        this.RegisterResources();
        this.BindMotionProperties();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _paginationNav  = e.NameScope.Find<PaginationNav>(PaginationTheme.NavPart);
        Debug.Assert(_paginationNav is not null);

        _paginationNav.ContainerPrepared   += HandleContainerPrepared;
        _paginationNav.PageNavigateRequest += HandlePageNavRequest;
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
                _previousPageItem.Content  = AntDesignIconPackage.LeftOutlined();
            }
            else if (count - 1 == args.Index)
            {
                navItem.PaginationItemType = PaginationItemType.Next;
                _nextPageItem              = navItem;
                _nextPageItem.Content      = AntDesignIconPackage.RightOutlined();
            }
            else
            {
                navItem.PaginationItemType = PaginationItemType.PageIndicator;
            }
        }
        if (_paginationNav.GetRealizedContainers().Count() == count)
        {
            HandlePageConditionChanged();
        }
    }

    private void HandlePageConditionChanged()
    {
        var total       = Math.Max(0, Total);
        var pageSize    = PageSize <= 0 ? DefaultPageSize : PageSize;
        var pageCount   = (int)Math.Ceiling(total / (double)pageSize);
        var currentPage = Math.Max(1, Math.Min(CurrentPage, pageCount));
        _currentPage = currentPage;
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
    }

    private void HandlePageNavRequest(object? sender, PageNavRequestArgs args)
    {
        if (args.PageNumber != CurrentPage)
        {
            CurrentPage = args.PageNumber;
        }
    }

    private void SetupLeftButtonRange(int currentPage, int pageCount)
    {
        if (currentPage < 5) {
            for (var i = 1; i < currentPage; i++)
            {
                SetupNextIndicatorNavItem(i, false);
            }
        } else {
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
        navItem.Content            = AntDesignIconPackage.EllipsisOutlined();
        navItem.PaginationItemType = PaginationItemType.Ellipses;
        navItem.IsVisible          = true;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == TotalProperty ||
                change.Property == PageSizeProperty ||
                change.Property == CurrentPageProperty)
            {
                HandlePageConditionChanged();
            }
        }

        if (change.Property == ShowSizeChangerProperty)
        {
            SetupSizeChanger();
        }
    }

    private void SetupSizeChanger()
    {
        if (SizeChanger == null)
        {
            var sizeChanger = new ComboBox();
            BindUtils.RelayBind(this, SizeTypeProperty, sizeChanger, ComboBox.SizeTypeProperty);
            sizeChanger.Items.Add(new PageSizeComboBoxItem { Content = "10 / page", PageSize = 10});
            sizeChanger.Items.Add(new PageSizeComboBoxItem { Content = "20 / page", PageSize = 20 });
            sizeChanger.Items.Add(new PageSizeComboBoxItem { Content = "50 / page", PageSize = 50 });
            sizeChanger.Items.Add(new PageSizeComboBoxItem { Content = "100 / page", PageSize = 100 });
            sizeChanger.SelectedIndex    =  0;
            SizeChanger                  =  sizeChanger;
            SizeChanger.SelectionChanged += HandlePageSizeChanged;
        }
    }

    private void HandlePageSizeChanged(object? sender, SelectionChangedEventArgs? args)
    {
        if (args?.AddedItems.Count >= 1 && args.AddedItems[0] is PageSizeComboBoxItem comboBoxItem)
        {
            PageSize = Math.Max(comboBoxItem.PageSize, 1);
        }
    }
}