using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.IconPkg;

/// <summary>
/// Base class for controls which decorate a icon control.
/// </summary>
[PseudoClasses(StdPseudoClass.Empty)]
public class IconPresenter : Control
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

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<IconPresenter, double>(nameof(IconWidth), double.NaN);

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<IconPresenter, double>(nameof(IconHeight), double.NaN);
    
    public static readonly StyledProperty<IconMode> IconModeProperty =
        Icon.IconModeProperty.AddOwner<IconPresenter>();
    
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
    
    public IconMode IconMode
    {
        get => GetValue(IconModeProperty);
        set => SetValue(IconModeProperty, value);
    }

    #endregion
    
    /// <summary>
    /// Initializes static members of the <see cref="IconPresenter"/> class.
    /// </summary>
    static IconPresenter()
    {
        AffectsMeasure<IconPresenter>(IconProperty, PaddingProperty);
        IconProperty.Changed.AddClassHandler<IconPresenter>((x, e) => x.ChildChanged(e));
    }
    
    public IconPresenter()
    {
        UpdatePseudoClasses();
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        return LayoutHelper.MeasureChild(Icon, availableSize, Padding);
    }

    /// <inheritdoc/>
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
        // SetValue(LoadingAnimationProperty, icon.LoadingAnimation, BindingPriority.Template);
        // SetValue(LoadingAnimationDurationProperty, icon.LoadingAnimationDuration, BindingPriority.Template);
        if (icon.ThemeType != IconThemeType.TwoTone)
        {
            if (icon.NormalFilledBrush != null)
            {
                SetValue(NormalFilledBrushProperty, icon.NormalFilledBrush, BindingPriority.Template);
            }
            if (icon.ActiveFilledBrush != null)
            {
                SetValue(ActiveFilledBrushProperty, icon.ActiveFilledBrush, BindingPriority.Template);
            }
            if (icon.SecondaryFilledBrush != null)
            {
                SetValue(SelectedFilledBrushProperty, icon.SecondaryFilledBrush, BindingPriority.Template);
            }
            if (icon.SecondaryFilledBrush != null)
            {
                SetValue(DisabledFilledBrushProperty, icon.DisabledFilledBrush, BindingPriority.Template);
            }
        }
        
        BindUtils.RelayBind(this, LoadingAnimationProperty, icon, Icon.LoadingAnimationProperty);
        BindUtils.RelayBind(this, LoadingAnimationDurationProperty, icon,
            Icon.LoadingAnimationDurationProperty);
        BindUtils.RelayBind(this, IconHeightProperty, icon, HeightProperty);
        BindUtils.RelayBind(this, IconWidthProperty, icon, WidthProperty);
        BindUtils.RelayBind(this, IconModeProperty, icon, IconModeProperty);
        if (icon.ThemeType != IconThemeType.TwoTone)
        {
            BindUtils.RelayBind(this, NormalFilledBrushProperty, icon, Icon.NormalFilledBrushProperty);
            BindUtils.RelayBind(this, ActiveFilledBrushProperty, icon, Icon.ActiveFilledBrushProperty);
            BindUtils.RelayBind(this, SelectedFilledBrushProperty, icon, Icon.SelectedFilledBrushProperty);
            BindUtils.RelayBind(this, DisabledFilledBrushProperty, icon, Icon.DisabledFilledBrushProperty);
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Empty, Icon is null);
    }
}