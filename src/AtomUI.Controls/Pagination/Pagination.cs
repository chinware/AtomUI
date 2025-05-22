using System.Diagnostics;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;

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
        get => _hideOnSinglePage;
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
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => PaginationToken.ID;
    #endregion

    private PaginationNav? _paginationNav;
    private PaginationNavItem? _previousPageItem;
    private PaginationNavItem? _nextPageItem;
    private PaginationNavItem? _leftEllipsesItem;
    private PaginationNavItem? _rightEllipsesItem;

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
        var count = _paginationNav.ItemCount;
        _paginationNav.ContainerPrepared += (sender, args) =>
        {
            if (args.Container is PaginationNavItem navItem)
            {
                if (0 == args.Index)
                {
                    navItem.PaginationItemType = PaginationItemType.Previous;
                    _previousPageItem          = navItem;
                    _previousPageItem.Content = AntDesignIconPackage.LeftOutlined();
                } 
                else if (2 == args.Index)
                {
                    navItem.PaginationItemType = PaginationItemType.Ellipses;
                    _leftEllipsesItem          = navItem;
                    _leftEllipsesItem.Content  = AntDesignIconPackage.EllipsisOutlined();
                }
                else if (count - 1 == args.Index)
                {
                    navItem.PaginationItemType = PaginationItemType.Next;
                    _nextPageItem              = navItem;
                    _nextPageItem.Content      = AntDesignIconPackage.RightOutlined();
                }
                else if (count - 3 == args.Index)
                {
                    navItem.PaginationItemType = PaginationItemType.Ellipses;
                    _rightEllipsesItem         = navItem;
                    _rightEllipsesItem.Content = AntDesignIconPackage.EllipsisOutlined();
                }
                else
                {
                    navItem.PaginationItemType = PaginationItemType.PageIndicator;
                }
            }
        };
    }
}