using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected)]
public class TabItem : HeaderedContentControl, ISelectable
{
    #region 公共属性定义
    
    public static readonly DirectProperty<TabItem, Dock?> TabStripPlacementProperty =
        AvaloniaProperty.RegisterDirect<TabItem, Dock?>(nameof(TabStripPlacement), 
            o => o.TabStripPlacement);
    
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<TabItem>();

    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<TabItem, Icon?>(nameof(Icon));

    public static readonly StyledProperty<Icon?> CloseIconProperty =
        AvaloniaProperty.Register<TabItem, Icon?>(nameof(CloseIcon));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<TabItem, bool>(nameof(IsClosable));
    
    public static readonly StyledProperty<bool> IsAutoHideCloseButtonProperty =
        AvaloniaProperty.Register<TabItem, bool>(nameof(IsAutoHideCloseButton));
    
    public Dock? TabStripPlacement
    {
        get => _tabStripPlacement;
        internal set => SetAndRaise(TabStripPlacementProperty, ref _tabStripPlacement, value);
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

    public Icon? CloseIcon
    {
        get => GetValue(CloseIconProperty);
        set => SetValue(CloseIconProperty, value);
    }

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    public bool IsAutoHideCloseButton
    {
        get => GetValue(IsAutoHideCloseButtonProperty);
        set => SetValue(IsAutoHideCloseButtonProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<TabItem>();

    internal static readonly StyledProperty<TabSharp> ShapeProperty =
        AvaloniaProperty.Register<TabItem, TabSharp>(nameof(Shape));

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TabItem>();
    
    internal static readonly StyledProperty<double> CloseButtonOpacityProperty =
        AvaloniaProperty.Register<TabItem, double>(nameof(CloseButtonOpacity));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public TabSharp Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal double CloseButtonOpacity
    {
        get => GetValue(CloseButtonOpacityProperty);
        set => SetValue(CloseButtonOpacityProperty, value);
    }
    #endregion
    
    private Dock? _tabStripPlacement;
    private IconButton? _closeButton;
    
    static TabItem()
    {
        SelectableMixin.Attach<TabItem>(IsSelectedProperty);
        PressedMixin.Attach<TabItem>();
        FocusableProperty.OverrideDefaultValue(typeof(TabItem), true);
        DataContextProperty.Changed.AddClassHandler<TabItem>((x, e) => x.UpdateHeader(e));
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<TabItem>(AutomationControlType.TabItem);
        AutomationProperties.IsOffscreenBehaviorProperty.OverrideDefaultValue<TabItem>(IsOffscreenBehavior.FromClip);
    }
    
    private void SetupDefaultCloseIcon()
    {
        if (CloseIcon is null)
        {
            ClearValue(CloseIconProperty);
            SetValue(CloseIconProperty, AntDesignIconPackage.CloseOutlined(), BindingPriority.Template);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SetupShapeThemeBindings(false);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _closeButton   = e.NameScope.Find<IconButton>(TabItemThemeConstants.ItemCloseButtonPart);

        if (_closeButton is not null)
        {
            _closeButton.Click += HandleCloseRequest;
        }
      
        SetupDefaultCloseIcon();
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty),
                    TransitionUtils.CreateTransition<DoubleTransition>(CloseButtonOpacityProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    private void HandleCloseRequest(object? sender, RoutedEventArgs args)
    {
        if (Parent is BaseTabControl tabControl)
        {
            // 使用新的统一关闭方法
            tabControl.CloseTab(this);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToLogicalTree())
        {
            if (change.Property == ShapeProperty)
            {
                SetupShapeThemeBindings(true);
            }
        }
        
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
        
        if (change.Property == CloseIconProperty)
        {
            SetupDefaultCloseIcon();
        }
    }

    private void SetupShapeThemeBindings(bool force)
    {
        if (force || Theme == null)
        {
            string? resourceKey = null;
            if (Shape == TabSharp.Line)
            {
                resourceKey = TabItemThemeConstants.TabItemThemeId;
            }
            else
            {
                resourceKey = TabItemThemeConstants.CardTabItemThemeId;
            }
            if (Application.Current != null)
            {
                if (Application.Current.TryFindResource(resourceKey, out var resource))
                {
                    if (resource is ControlTheme theme)
                    {
                        Theme = theme;
                    }
                }
            }
        }
    }

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
    
    protected override void OnAccessKey(RoutedEventArgs e)
    {
        Focus();
        SetCurrentValue(IsSelectedProperty, true);
        e.Handled = true;
    }
    
    private void UpdateHeader(AvaloniaPropertyChangedEventArgs obj)
    {
        if (Header == null)
        {
            if (obj.NewValue is IHeadered headered)
            {
                if (Header != headered.Header)
                {
                    SetCurrentValue(HeaderProperty, headered.Header);
                }
            }
            else
            {
                if (!(obj.NewValue is Control))
                {
                    SetCurrentValue(HeaderProperty, obj.NewValue);
                }
            }
        }
        else
        {
            if (Header == obj.OldValue)
            {
                SetCurrentValue(HeaderProperty, obj.NewValue);
            }
        }
    }
}