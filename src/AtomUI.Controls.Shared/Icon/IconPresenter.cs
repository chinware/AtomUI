using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace AtomUI.Controls;

/// <summary>
/// Base class for controls which decorate a icon control.
/// </summary>
[PseudoClasses(StdPseudoClass.Empty)]
public class IconPresenter : Control, IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<IconPresenter, Icon?>(nameof(Icon));

    public static readonly StyledProperty<Thickness> PaddingProperty =
        Decorator.PaddingProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        Icon.LoadingAnimationProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        Icon.LoadingAnimationDurationProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> NormalFilledBrushProperty =
        Icon.NormalFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> ActiveFilledBrushProperty =
        Icon.ActiveFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> SelectedFilledBrushProperty =
        Icon.SelectedFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> DisabledFilledBrushProperty =
        Icon.DisabledFilledBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> PrimaryFilledBrushProperty =
        Icon.PrimaryFilledBrushProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<IBrush?> SecondaryFilledBrushProperty =
        Icon.SecondaryFilledBrushProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<IconPresenter, double>(nameof(IconWidth), double.NaN);

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<IconPresenter, double>(nameof(IconHeight), double.NaN);
    
    public static readonly StyledProperty<IconMode> IconModeProperty =
        Icon.IconModeProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconPresenter>();
    
    [Content]
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public Thickness Padding
    {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
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

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
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
    
    /// <summary>
    /// Initializes static members of the <see cref="IconPresenter"/> class.
    /// </summary>
    static IconPresenter()
    {
        AffectsMeasure<IconPresenter>(IconProperty, PaddingProperty, IconHeightProperty, IconWidthProperty);
        AffectsRender<IconPresenter>(IconModeProperty, NormalFilledBrushProperty, ActiveFilledBrushProperty, SelectedFilledBrushProperty, DisabledFilledBrushProperty);
        IconProperty.Changed.AddClassHandler<IconPresenter>((x, e) => x.ChildChanged(e));
    }
    
    public IconPresenter()
    {
        UpdatePseudoClasses();
        this.ConfigureMotionBindingStyle();
        ConfigureInstanceStyles();
    }

    private void ConfigureInstanceStyles()
    {
        var style = new Style();
        style.Add(NormalFilledBrushProperty, SharedTokenKey.ColorTextTertiary);
        style.Add(PrimaryFilledBrushProperty, SharedTokenKey.ColorPrimary);
        style.Add(SecondaryFilledBrushProperty, SharedTokenKey.ColorInfoBg);
        Styles.Add(style);
    }
        
    protected override Size MeasureOverride(Size availableSize)
    {
        return LayoutHelper.MeasureChild(Icon, availableSize, Padding);
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        return LayoutHelper.ArrangeChild(Icon, finalSize, Padding);
    }

    /// <summary>
    /// Called when the <see cref="IconPresenter"/> property changes.
    /// </summary>
    /// <param name="e">The event args.</param>
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
            if (Icon != null)
            {
                ConfigureIcon(Icon);
            }
        }
    }

    private void ConfigureIcon(Icon icon)
    {
        _bindingDisposables?.Dispose();
        _bindingDisposables = new CompositeDisposable(10);
        SetCurrentValue(LoadingAnimationProperty, icon.LoadingAnimation);
        SetCurrentValue(LoadingAnimationDurationProperty, icon.LoadingAnimationDuration);
        if (icon.ThemeType != IconThemeType.TwoTone)
        {
            if (icon.NormalFilledBrush != null)
            {
                SetCurrentValue(NormalFilledBrushProperty, icon.NormalFilledBrush);
            }
            if (icon.ActiveFilledBrush != null)
            {
                SetCurrentValue(ActiveFilledBrushProperty, icon.ActiveFilledBrush);
            }
            if (icon.SecondaryFilledBrush != null)
            {
                SetCurrentValue(SelectedFilledBrushProperty, icon.SecondaryFilledBrush);
            }
            if (icon.SecondaryFilledBrush != null)
            {
                SetCurrentValue(DisabledFilledBrushProperty, icon.DisabledFilledBrush);
            }
        }
        else
        {
            if (icon.PrimaryFilledBrush != null)
            {
                SetCurrentValue(PrimaryFilledBrushProperty, icon.PrimaryFilledBrush);
            }
            if (icon.SecondaryFilledBrush != null)
            {
                SetCurrentValue(SecondaryFilledBrushProperty, icon.SecondaryFilledBrush);
            }
        }
        
        _bindingDisposables.Add(BindUtils.RelayBind(this, LoadingAnimationProperty, icon, Icon.LoadingAnimationProperty));
        _bindingDisposables.Add(BindUtils.RelayBind(this, LoadingAnimationDurationProperty, icon,
            Icon.LoadingAnimationDurationProperty));
        _bindingDisposables.Add(BindUtils.RelayBind(this, IconHeightProperty, icon, HeightProperty));
        _bindingDisposables.Add(BindUtils.RelayBind(this, IconWidthProperty, icon, WidthProperty));
        _bindingDisposables.Add(BindUtils.RelayBind(this, IconModeProperty, icon, IconModeProperty));
        _bindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, icon, IsMotionEnabledProperty));
        
        if (icon.ThemeType != IconThemeType.TwoTone)
        {
            _bindingDisposables.Add(BindUtils.RelayBind(this, NormalFilledBrushProperty, icon, Icon.NormalFilledBrushProperty));
            _bindingDisposables.Add(BindUtils.RelayBind(this, ActiveFilledBrushProperty, icon, Icon.ActiveFilledBrushProperty));
            _bindingDisposables.Add(BindUtils.RelayBind(this, SelectedFilledBrushProperty, icon, Icon.SelectedFilledBrushProperty));
            _bindingDisposables.Add(BindUtils.RelayBind(this, DisabledFilledBrushProperty, icon, Icon.DisabledFilledBrushProperty));
        }
        else
        {
            _bindingDisposables.Add(BindUtils.RelayBind(this, PrimaryFilledBrushProperty, icon, Icon.PrimaryFilledBrushProperty));
            _bindingDisposables.Add(BindUtils.RelayBind(this, SecondaryFilledBrushProperty, icon, Icon.SecondaryFilledBrushProperty));
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Empty, Icon is null);
    }
}