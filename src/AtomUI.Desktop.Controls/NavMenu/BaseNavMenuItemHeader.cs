using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

[PseudoClasses(NavMenuItemPseudoClass.Icon)]
internal class BaseNavMenuItemHeader : TemplatedControl
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> HeaderProperty =
        HeaderedContentControl.HeaderProperty.AddOwner<BaseNavMenuItemHeader>();
    
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        HeaderedItemsControl.HeaderTemplateProperty.AddOwner<BaseNavMenuItemHeader>();
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<BaseNavMenuItemHeader, Icon?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BaseNavMenuItemHeader>();
    
    public static readonly DirectProperty<BaseNavMenuItemHeader, bool> HasSubMenuProperty =
        AvaloniaProperty.RegisterDirect<BaseNavMenuItemHeader, bool>(nameof(HasSubMenu),
            o => o.HasSubMenu,
            (o, v) => o.HasSubMenu = v);
    
    public static readonly StyledProperty<bool> IsSubMenuOpenProperty =
        AvaloniaProperty.Register<BaseNavMenuItemHeader, bool>(nameof(IsSubMenuOpen));
    
    public static readonly DirectProperty<BaseNavMenuItemHeader, bool> IsDarkStyleProperty =
        AvaloniaProperty.RegisterDirect<BaseNavMenuItemHeader, bool>(nameof(IsDarkStyle),
            o => o.IsDarkStyle,
            (o, v) => o.IsDarkStyle = v);
    
    public static readonly DirectProperty<BaseNavMenuItemHeader, bool> IsSelectedProperty =
        AvaloniaProperty.RegisterDirect<BaseNavMenuItemHeader, bool>(
            nameof(IsSelected), 
            o => o.IsSelected,
            (o, v) => o.IsSelected = v);
    
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    private bool _hasSubMenu;
    
    public bool HasSubMenu
    {
        get => _hasSubMenu;
        set => SetAndRaise(HasSubMenuProperty, ref _hasSubMenu, value);
    }
    
    public bool IsSubMenuOpen
    {
        get => GetValue(IsSubMenuOpenProperty);
        set => SetValue(IsSubMenuOpenProperty, value);
    }
    
    private bool _isDarkStyle;

    public bool IsDarkStyle
    {
        get => _isDarkStyle;
        set => SetAndRaise(IsDarkStyleProperty, ref _isDarkStyle, value);
    }
    
    private bool _isSelected;
    
    public bool IsSelected
    {
        get => _isSelected;
        set => SetAndRaise(IsSelectedProperty, ref _isSelected, value);
    }
    #endregion

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconProperty)
        {
            UpdatePseudoClasses();
        }
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(NavMenuItemPseudoClass.Icon, Icon is not null);
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                var transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
                };
                NotifyConfigureTransitions(transitions);
                Transitions = transitions;
            }
        }
        else
        {
            Transitions = null;
        }
    }
    
    protected virtual void NotifyConfigureTransitions(Transitions transitions)
    {}
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }

}