using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

using IconControl = Icon;

/// <summary>
/// Base class for controls which decorate a icon control.
/// </summary>
[PseudoClasses(StdPseudoClass.Empty)]
public class IconPresenter : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<IconPresenter, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        IconControl.LoadingAnimationProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> StrokeBrushProperty =
        IconControl.StrokeBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> FillBrushProperty =
        IconControl.FillBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> SecondaryStrokeBrushProperty =
        IconControl.SecondaryStrokeBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> SecondaryFillBrushProperty =
        IconControl.SecondaryFillBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> FallbackBrushProperty =
        IconControl.FallbackBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<double> StrokeWidthProperty =
        AvaloniaProperty.Register<IconPresenter, double>(
            nameof(StrokeWidth), 4);

    public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty =
        AvaloniaProperty.Register<IconPresenter, PenLineCap>(
            nameof(StrokeLineCap), PenLineCap.Round);

    public static readonly StyledProperty<PenLineJoin> StrokeLineJoinProperty =
        AvaloniaProperty.Register<IconPresenter, PenLineJoin>(
            nameof(StrokeLineJoin), PenLineJoin.Round);

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        IconControl.LoadingAnimationDurationProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconPresenter>();
    
    [Content]
    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public IconAnimation LoadingAnimation
    {
        get => GetValue(LoadingAnimationProperty);
        set => SetValue(LoadingAnimationProperty, value);
    }
    
    public IBrush? StrokeBrush
    {
        get => GetValue(StrokeBrushProperty);
        set => SetValue(StrokeBrushProperty, value);
    }
    
    public IBrush? FillBrush
    {
        get => GetValue(FillBrushProperty);
        set => SetValue(FillBrushProperty, value);
    }
    
    public IBrush? SecondaryStrokeBrush
    {
        get => GetValue(SecondaryStrokeBrushProperty);
        set => SetValue(SecondaryStrokeBrushProperty, value);
    }
    
    public IBrush? SecondaryFillBrush
    {
        get => GetValue(SecondaryFillBrushProperty);
        set => SetValue(SecondaryFillBrushProperty, value);
    }
    
    public IBrush? FallbackBrush
    {
        get => GetValue(FallbackBrushProperty);
        set => SetValue(FallbackBrushProperty, value);
    }
    
    public double StrokeWidth
    {
        get => GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }

    public PenLineCap StrokeLineCap
    {
        get => GetValue(StrokeLineCapProperty);
        set => SetValue(StrokeLineCapProperty, value);
    }

    public PenLineJoin StrokeLineJoin
    {
        get => GetValue(StrokeLineJoinProperty);
        set => SetValue(StrokeLineJoinProperty, value);
    }

    public TimeSpan LoadingAnimationDuration
    {
        get => GetValue(LoadingAnimationDurationProperty);
        set => SetValue(LoadingAnimationDurationProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    
    private CompositeDisposable? _bindingDisposables;
    
    static IconPresenter()
    {
        AffectsMeasure<IconPresenter>(IconProperty);
        AffectsRender<IconPresenter>(StrokeBrushProperty,
            FillBrushProperty,
            SecondaryStrokeBrushProperty,
            SecondaryFillBrushProperty,
            FallbackBrushProperty);
        IconProperty.Changed.AddClassHandler<IconPresenter>((x, e) => x.ChildChanged(e));
    }
    
    public IconPresenter()
    {
        UpdatePseudoClasses();
    }
    
    private void ChildChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldChild = (Control?)e.OldValue;
        var newChild = (Control?)e.NewValue;

        if (oldChild != null)
        {
            ((ISetLogicalParent)oldChild).SetParent(null);
            LogicalChildren.Clear();
            VisualChildren.Remove(oldChild);
        }

        if (newChild != null)
        {
            ((ISetLogicalParent)newChild).SetParent(this);
            VisualChildren.Add(newChild);
            LogicalChildren.Add(newChild);
        }

        UpdatePseudoClasses();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IconProperty)
        {
            if (e.OldValue != null)
            {
                _bindingDisposables?.Dispose();
                _bindingDisposables = null;
            }
            if (e.NewValue is PathIcon newIcon)
            {
                ConfigureIcon(newIcon);
            }
        }
    }

    private void ConfigureIcon(PathIcon pathIcon)
    {
        _bindingDisposables?.Dispose();
        _bindingDisposables = new CompositeDisposable(16);
        
        _bindingDisposables.Add(BindUtils.RelayBind(this, HeightProperty, pathIcon, HeightProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, WidthProperty, pathIcon, WidthProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, StrokeBrushProperty, pathIcon, StrokeBrushProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, FillBrushProperty, pathIcon, FillBrushProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, SecondaryFillBrushProperty, pathIcon, SecondaryFillBrushProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, SecondaryStrokeBrushProperty, pathIcon, SecondaryStrokeBrushProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, FallbackBrushProperty, pathIcon, FallbackBrushProperty, BindingMode.Default, BindingPriority.Template));
        if (pathIcon is Icon icon)
        {
            _bindingDisposables.Add(BindUtils.RelayBind(this, LoadingAnimationProperty, icon, IconControl.LoadingAnimationProperty, BindingMode.Default, BindingPriority.Template));
            _bindingDisposables.Add(BindUtils.RelayBind(this, LoadingAnimationDurationProperty, icon,
                IconControl.LoadingAnimationDurationProperty, BindingMode.Default, BindingPriority.Template));
            _bindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, icon, IsMotionEnabledProperty));
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Empty, Icon is null);
    }
}