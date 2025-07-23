using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

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
    
    public static readonly StyledProperty<int> CurrentPageProperty =
        AvaloniaProperty.Register<AbstractPagination, int>(nameof(CurrentPage), DefaultCurrentPage,
            validate:v => v > 0);
    
    public static readonly StyledProperty<int> PageSizeProperty =
        AvaloniaProperty.Register<AbstractPagination, int>(nameof(PageSize), DefaultPageSize,
            validate:PageSizeValidator);
    
    public static readonly StyledProperty<int> TotalProperty =
        AvaloniaProperty.Register<AbstractPagination, int>(nameof(Total), validate:v => v >= 0);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractPagination>();
    
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
    
    public int CurrentPage
    {
        get => GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }
    
    public int PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }
    
    public int Total
    {
        get => GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
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

    private static bool PageSizeValidator(int pageSize)
    {
        int[] allowPageSizes = [10, 20, 50, 100];
        return allowPageSizes.Contains(pageSize);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    private CompositeDisposable? _resourceBindingsDisposable;
    
    #endregion
    
    protected bool TemplateApplied = false;
    
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
        if (TemplateApplied)
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
        CurrentPage = currentPage;
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