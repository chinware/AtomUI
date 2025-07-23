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
    
    public static readonly StyledProperty<bool> IsLastProperty 
        = AvaloniaProperty.Register<BreadcrumbItem, bool>(nameof(IsLast));
    
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
    
    public bool IsLast
    {
        get => GetValue(IsLastProperty);
        internal set => SetValue(IsLastProperty, value);
    }
    
    #endregion

    internal static readonly StyledProperty<string> EffectiveSeparatorProperty
        //= MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BreadcrumbItem>();
        = AvaloniaProperty.Register<BreadcrumbItem, string>(nameof(EffectiveSeparator), defaultValue: string.Empty);
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BreadcrumbItem>();
    
    //public string EffectiveSeparator => Separator ?? (TemplatedParent as Breadcrumb)?.Separator;
    internal string EffectiveSeparator
    {
        get => GetValue(EffectiveSeparatorProperty);
        set => SetValue(EffectiveSeparatorProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    static BreadcrumbItem()
    {
        SelectableMixin.Attach<BreadcrumbItem>(IsSelectedProperty);
        PressedMixin.Attach<BreadcrumbItem>();
        FocusableProperty.OverrideDefaultValue<BreadcrumbItem>(true);
        AffectsRender<BreadcrumbItem>(BackgroundProperty);
    }
    
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureTransitions();
        UpdatePseudoClasses();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconProperty)
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
            Transitions ??= new Transitions
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
        PseudoClasses.Set(SegmentedPseudoClass.HasIcon, Icon is not null);
    }
    
    public void SetPseudoClass(string pseudoClass, bool value)
    {
        PseudoClasses.Set(pseudoClass, value);
    }
}