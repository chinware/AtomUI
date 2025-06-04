using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
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
        AvaloniaProperty.Register<IconPresenter, Thickness>(nameof(Padding));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        Icon.LoadingAnimationProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        Icon.LoadingAnimationDurationProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> NormalIconBrushProperty =
        AvaloniaProperty.Register<IconPresenter, IBrush?>(
            nameof(NormalIconColor));
    
    public static readonly StyledProperty<IBrush?> ActiveIconBrushProperty =
        AvaloniaProperty.Register<IconPresenter, IBrush?>(
            nameof(ActiveIconColor));
    
    public static readonly StyledProperty<IBrush?> SelectedIconBrushProperty =
        AvaloniaProperty.Register<IconPresenter, IBrush?>(
            nameof(SelectedIconColor));
    
    public static readonly StyledProperty<IBrush?> DisabledIconBrushProperty =
        AvaloniaProperty.Register<IconPresenter, IBrush?>(
            nameof(DisabledIconColor));

    public static readonly StyledProperty<double> IconWidthProperty
        = AvaloniaProperty.Register<IconPresenter, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty
        = AvaloniaProperty.Register<IconPresenter, double>(nameof(IconHeight));
    
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
    
    public IBrush? NormalIconColor
    {
        get => GetValue(NormalIconBrushProperty);
        set => SetValue(NormalIconBrushProperty, value);
    }

    public IBrush? ActiveIconColor
    {
        get => GetValue(ActiveIconBrushProperty);
        set => SetValue(ActiveIconBrushProperty, value);
    }

    public IBrush? SelectedIconColor
    {
        get => GetValue(SelectedIconBrushProperty);
        set => SetValue(SelectedIconBrushProperty, value);
    }

    public IBrush? DisabledIconColor
    {
        get => GetValue(DisabledIconBrushProperty);
        set => SetValue(DisabledIconBrushProperty, value);
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
        BindUtils.RelayBind(this, LoadingAnimationProperty, icon, Icon.LoadingAnimationProperty);
        BindUtils.RelayBind(this, LoadingAnimationDurationProperty, icon,
            Icon.LoadingAnimationDurationProperty);
        BindUtils.RelayBind(this, IconHeightProperty, icon, HeightProperty);
        BindUtils.RelayBind(this, IconWidthProperty, icon, WidthProperty);
        if (icon.ThemeType != IconThemeType.TwoTone)
        {
            BindUtils.RelayBind(this, NormalIconBrushProperty, icon, Icon.NormalFilledBrushProperty);
            BindUtils.RelayBind(this, ActiveIconBrushProperty, icon, Icon.ActiveFilledBrushProperty);
            BindUtils.RelayBind(this, SelectedIconBrushProperty, icon, Icon.SelectedFilledBrushProperty);
            BindUtils.RelayBind(this, DisabledIconBrushProperty, icon, Icon.DisabledFilledBrushProperty);
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Empty, Icon is null);
    }
}