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

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        IconControl.LoadingAnimationDurationProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> NormalFilledBrushProperty =
        IconControl.NormalFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> ActiveFilledBrushProperty =
        IconControl.ActiveFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> SelectedFilledBrushProperty =
        IconControl.SelectedFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> DisabledFilledBrushProperty =
        IconControl.DisabledFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> PrimaryFilledBrushProperty =
        IconControl.PrimaryFilledBrushProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<IBrush?> SecondaryFilledBrushProperty =
        IconControl.SecondaryFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IconMode> IconModeProperty =
        IconControl.IconModeProperty.AddOwner<IconPresenter>();
    
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

    public TimeSpan LoadingAnimationDuration
    {
        get => GetValue(LoadingAnimationDurationProperty);
        set => SetValue(LoadingAnimationDurationProperty, value);
    }
    
    public IBrush? NormalFilledBrush
    {
        get => GetValue(NormalFilledBrushProperty);
        set => SetValue(NormalFilledBrushProperty, value);
    }

    public IBrush? ActiveFilledBrush
    {
        get => GetValue(ActiveFilledBrushProperty);
        set => SetValue(ActiveFilledBrushProperty, value);
    }

    public IBrush? SelectedFilledBrush
    {
        get => GetValue(SelectedFilledBrushProperty);
        set => SetValue(SelectedFilledBrushProperty, value);
    }

    public IBrush? DisabledFilledBrush
    {
        get => GetValue(DisabledFilledBrushProperty);
        set => SetValue(DisabledFilledBrushProperty, value);
    }
    
    public IBrush? PrimaryFilledBrush
    {
        get => GetValue(PrimaryFilledBrushProperty);
        set => SetValue(PrimaryFilledBrushProperty, value);
    }

    public IBrush? SecondaryFilledBrush
    {
        get => GetValue(SecondaryFilledBrushProperty);
        set => SetValue(SecondaryFilledBrushProperty, value);
    }
    
    public IconMode IconMode
    {
        get => GetValue(IconModeProperty);
        set => SetValue(IconModeProperty, value);
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
        AffectsMeasure<IconPresenter>(IconProperty, PaddingProperty);
        AffectsRender<IconPresenter>(ForegroundProperty,
            PrimaryFilledBrushProperty,
            SecondaryFilledBrushProperty);
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
        _bindingDisposables.Add(BindUtils.RelayBind(this, ForegroundProperty, pathIcon, ForegroundProperty, BindingMode.Default, BindingPriority.Template));
        if (pathIcon is Icon icon)
        {
            _bindingDisposables.Add(BindUtils.RelayBind(this, LoadingAnimationProperty, icon, IconControl.LoadingAnimationProperty, BindingMode.Default, BindingPriority.Template));
            _bindingDisposables.Add(BindUtils.RelayBind(this, LoadingAnimationDurationProperty, icon,
                IconControl.LoadingAnimationDurationProperty, BindingMode.Default, BindingPriority.Template));
            _bindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, icon, IsMotionEnabledProperty));
        
            if (icon.ThemeType == IconThemeType.TwoTone)
            {
                _bindingDisposables.Add(BindUtils.RelayBind(this, PrimaryFilledBrushProperty, icon, IconControl.PrimaryFilledBrushProperty, BindingMode.Default, BindingPriority.Template));
                _bindingDisposables.Add(BindUtils.RelayBind(this, SecondaryFilledBrushProperty, icon, IconControl.SecondaryFilledBrushProperty, BindingMode.Default, BindingPriority.Template));
            }
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Empty, Icon is null);
    }
}