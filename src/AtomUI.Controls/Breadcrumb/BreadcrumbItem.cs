using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Animation;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected, BreadcrumbPseudoClass.HasIcon, BreadcrumbPseudoClass.IsLast)]
public class BreadcrumbItem : ContentControl, ISelectable
{

    #region 公共属性定义

    public static readonly StyledProperty<string?> SeparatorProperty =
        AvaloniaProperty.Register<BreadcrumbItem, string?>(nameof(Separator));
    
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<BreadcrumbItem, string?>(nameof(Value));
    
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<BreadcrumbItem>();

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<BreadcrumbItem, Icon?>(nameof(Icon));
    
    public string? Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }
    
    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<BreadcrumbItem, string> EffectiveSeparatorProperty =
        AvaloniaProperty.RegisterDirect<BreadcrumbItem, string>(
            nameof(EffectiveSeparator),
            o => o.EffectiveSeparator,
            (o, v) => o.EffectiveSeparator = v);
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BreadcrumbItem>();
    
    internal static readonly DirectProperty<BreadcrumbItem, bool> IsLastProperty =
        AvaloniaProperty.RegisterDirect<BreadcrumbItem, bool>(
            nameof(IsLast),
            o => o.IsLast,
            (o, v) => o.IsLast = v);
    
    private string _effectiveSeparator = string.Empty;
    internal string EffectiveSeparator
    {
        get => _effectiveSeparator;
        set => SetAndRaise(EffectiveSeparatorProperty, ref _effectiveSeparator, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    private bool _isLast;
    internal bool IsLast
    {
        get => _isLast;
        set => SetAndRaise(IsLastProperty, ref _isLast, value);
    }
    
    #endregion
    
    static BreadcrumbItem()
    {
        AffectsRender<BreadcrumbItem>(BackgroundProperty);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconProperty || change.Property == IsLastProperty)
        {
            UpdatePseudoClasses();
        }
        
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();    
            }
        }
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions = new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
            };
        }
        else
        {
            Transitions = null;
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(BreadcrumbPseudoClass.HasIcon, Icon is not null);
        PseudoClasses.Set(BreadcrumbPseudoClass.IsLast, IsLast);
    }
}