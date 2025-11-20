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

/// <summary>
/// Base class for controls which decorate a icon control.
/// </summary>
[PseudoClasses(StdPseudoClass.Empty)]
public class IconPresenter : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<IconPresenter, Icon?>(nameof(Icon));

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
        AffectsRender<IconPresenter>(IconModeProperty, NormalFilledBrushProperty, 
            ActiveFilledBrushProperty, 
            SelectedFilledBrushProperty, 
            DisabledFilledBrushProperty,
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
            if (e.NewValue is Icon newIcon)
            {
                ConfigureIcon(newIcon);
            }
            else if (e.OldValue != null)
            {
                _bindingDisposables?.Dispose();
                _bindingDisposables = null;
            }
        }
    }

    private void ConfigureIcon(Icon icon)
    {
        _bindingDisposables?.Dispose();
        _bindingDisposables = new CompositeDisposable(16);
        
        _bindingDisposables.Add(BindUtils.RelayBind(this, LoadingAnimationProperty, icon, Icon.LoadingAnimationProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, LoadingAnimationDurationProperty, icon,
            Icon.LoadingAnimationDurationProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, HeightProperty, icon, HeightProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, WidthProperty, icon, WidthProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, IconModeProperty, icon, IconModeProperty, BindingMode.Default, BindingPriority.Template));
        _bindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, icon, IsMotionEnabledProperty));
        
        if (icon.ThemeType != IconThemeType.TwoTone)
        {
            _bindingDisposables.Add(BindUtils.RelayBind(this, NormalFilledBrushProperty, icon, Icon.NormalFilledBrushProperty, BindingMode.Default, BindingPriority.Template));
            _bindingDisposables.Add(BindUtils.RelayBind(this, ActiveFilledBrushProperty, icon, Icon.ActiveFilledBrushProperty, BindingMode.Default, BindingPriority.Template));
            _bindingDisposables.Add(BindUtils.RelayBind(this, SelectedFilledBrushProperty, icon, Icon.SelectedFilledBrushProperty, BindingMode.Default, BindingPriority.Template));
            _bindingDisposables.Add(BindUtils.RelayBind(this, DisabledFilledBrushProperty, icon, Icon.DisabledFilledBrushProperty, BindingMode.Default, BindingPriority.Template));
        }
        else
        {
            _bindingDisposables.Add(BindUtils.RelayBind(this, PrimaryFilledBrushProperty, icon, Icon.PrimaryFilledBrushProperty, BindingMode.Default, BindingPriority.Template));
            _bindingDisposables.Add(BindUtils.RelayBind(this, SecondaryFilledBrushProperty, icon, Icon.SecondaryFilledBrushProperty, BindingMode.Default, BindingPriority.Template));
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Empty, Icon is null);
    }
}