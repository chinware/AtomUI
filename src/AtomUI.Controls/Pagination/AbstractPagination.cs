using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public abstract class AbstractPagination : TemplatedControl,
                                           ISizeTypeAware,
                                           IMotionAwareControl,
                                           IResourceBindingManager
{
    public const int DefaultPageSize = 10;
    public const int DefaultCurrentPage = 1;
    
    #region 公共属性定义
    
    public static readonly StyledProperty<PaginationAlign> AlignProperty =
        AvaloniaProperty.Register<AbstractPagination, PaginationAlign>(nameof(PaginationAlign));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<AbstractPagination>();
    
    public static readonly DirectProperty<AbstractPagination, int> CurrentPageProperty =
        AvaloniaProperty.RegisterDirect<AbstractPagination, int>(nameof(CurrentPage),
            o => o.CurrentPage,
            (o, v) => o.CurrentPage = v,
            unsetValue: DefaultCurrentPage);
    
    public static readonly DirectProperty<AbstractPagination, int> PageSizeProperty =
        AvaloniaProperty.RegisterDirect<AbstractPagination, int>(nameof(PageSize),
            o => o.PageSize,
            (o, v) => o.PageSize = v,
            unsetValue: DefaultPageSize,
            enableDataValidation:true);
    
    public static readonly DirectProperty<AbstractPagination, long> TotalProperty =
        AvaloniaProperty.RegisterDirect<AbstractPagination, long>(nameof(Total),
            o => o.Total,
            (o, v) => o.Total = v);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractPagination>();
    
    public PaginationAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    private protected int _currentPage = DefaultCurrentPage;
    public int CurrentPage
    {
        get => _currentPage;
        set => SetAndRaise(CurrentPageProperty, ref _currentPage, value);
    }
    
    private protected int _pageSize = DefaultPageSize;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (!new[] { 10, 20, 50, 100 }.Contains(value))
            {
                throw new ArgumentException("PageSize only allow: 10, 20, 50, 100");
            }
            SetAndRaise(PageSizeProperty, ref _pageSize, value);
        }
    }
    
    private protected long _total;
    public long Total
    {
        get => _total;
        set => SetAndRaise(TotalProperty, ref _total, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    #region 公共事件定义
    public event EventHandler<PageChangedArgs>? CurrentPageChanged;
    #endregion
    
    #region 内部属性定义
    Control IMotionAwareControl.PropertyBindTarget => this;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;
    private CompositeDisposable? _resourceBindingsDisposable;
    #endregion
    
    static AbstractPagination()
    {
        AffectsMeasure<AbstractPagination>(SizeTypeProperty);
    }
    
    public AbstractPagination()
    {
        this.BindMotionProperties();
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
    }

    protected void HandlePageConditionChanged()
    {
        var total       = Math.Max(0, Total);
        var pageSize    = PageSize <= 0 ? DefaultPageSize : PageSize;
        var pageCount   = (int)Math.Ceiling(total / (double)pageSize);
        var currentPage = Math.Max(1, Math.Min(CurrentPage, pageCount));
        _currentPage = currentPage;
        NotifyPageConditionChanged(currentPage, pageCount, pageSize, total);
    }

    protected virtual void NotifyPageConditionChanged(int currentPage, int pageCount, int pageSize, long total)
    {
    }

    protected void EmitCurrentPageChanged(int currentPage, int pageCount, int pageSize)
    {
        CurrentPageChanged?.Invoke(this, new PageChangedArgs(currentPage, pageCount, pageSize));
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }
}