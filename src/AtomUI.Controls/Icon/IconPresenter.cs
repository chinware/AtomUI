using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

using IconControl = Icon;

public class IconPresenter : Control, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<IconPresenter, PathIcon?>(nameof(Icon));
    
    public static readonly StyledProperty<IBrush?> IconBrushProperty =
        AvaloniaProperty.Register<IconPresenter, IBrush?>(nameof(IconBrush));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconPresenter>();
    
    [Content]
    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public IBrush? IconBrush
    {
        get => GetValue(IconBrushProperty);
        set => SetValue(IconBrushProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion
    
    private CompositeDisposable? _disposables;
    
    static IconPresenter()
    {
        AffectsMeasure<IconPresenter>(IconProperty);
        AffectsRender<IconPresenter>(IconBrushProperty);
        IconProperty.Changed.AddClassHandler<IconPresenter>((x, e) => x.HandleIconChanged(e));
    }
    
    private void HandleIconChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var oldChild = (Control?)change.OldValue;
        var newChild = (Control?)change.NewValue;
        if (oldChild != null)
        {
            _disposables?.Dispose();
            _disposables = null;
            ((ISetLogicalParent)oldChild).SetParent(null);
            LogicalChildren.Remove(oldChild);
            VisualChildren.Remove(oldChild);
        }

        if (newChild is PathIcon pathIcon)
        {
            AddIcon(pathIcon);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (Icon != null)
        {
            ConfigureIcon(Icon);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _disposables?.Dispose();
        _disposables = null;
    }

    private void AddIcon(PathIcon pathIcon)
    {
        ConfigureIcon(pathIcon);
        ((ISetLogicalParent)pathIcon).SetParent(null);
        pathIcon.SetVisualParent(null);
        VisualChildren.Add(pathIcon);
        LogicalChildren.Add(pathIcon);
    }

    private void ConfigureIcon(PathIcon pathIcon)
    {
        _disposables?.Dispose();
        _disposables = new CompositeDisposable(4);
        _disposables.Add(BindUtils.RelayBind(this, WidthProperty, pathIcon, WidthProperty));
        _disposables.Add(BindUtils.RelayBind(this, HeightProperty, pathIcon, HeightProperty));
        if (pathIcon is Icon icon)
        {
            _disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, icon, IconControl.IsMotionEnabledProperty));
            _disposables.Add(BindUtils.RelayBind(this, IconBrushProperty, icon, IconControl.StrokeBrushProperty, BindingMode.Default, BindingPriority.Template));
            _disposables.Add(BindUtils.RelayBind(this, IconBrushProperty, icon, IconControl.FillBrushProperty, BindingMode.Default, BindingPriority.Template));
        }
        else
        {
            _disposables.Add(BindUtils.RelayBind(this, IconBrushProperty, pathIcon, PathIcon.ForegroundProperty, BindingMode.Default, BindingPriority.Template));
        }
    }
}