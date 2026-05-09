using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.MotionScene;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

public class ContextMenu : AvaloniaContextMenu,
                           ISizeTypeAware,
                           IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<BoxShadows> PopupRootShadowProperty =
        Popup.PopupRootShadowProperty.AddOwner<ContextMenu>();
    
    public static readonly StyledProperty<BoxShadows> OverlayHostShadowProperty =
        Popup.OverlayHostShadowProperty.AddOwner<ContextMenu>();
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<ContextMenu>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ContextMenu>();

    public static readonly StyledProperty<int> DisplayPageSizeProperty =
        Menu.DisplayPageSizeProperty.AddOwner<ContextMenu>();

    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        Menu.ShouldUseOverlayPopupProperty.AddOwner<ContextMenu>();
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<ContextMenu>();
    
    public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty = 
        Popup.OpenMotionProperty.AddOwner<ContextMenu>();
        
    public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty = 
        Popup.CloseMotionProperty.AddOwner<ContextMenu>();
    
    public BoxShadows PopupRootShadow
    {
        get => GetValue(PopupRootShadowProperty);
        set => SetValue(PopupRootShadowProperty, value);
    }
    
    public BoxShadows OverlayHostShadow
    {
        get => GetValue(OverlayHostShadowProperty);
        set => SetValue(OverlayHostShadowProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public int DisplayPageSize
    {
        get => GetValue(DisplayPageSizeProperty);
        set => SetValue(DisplayPageSizeProperty, value);
    }

    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }
    
    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }
    
    public AbstractMotion? OpenMotion
    {
        get => GetValue(OpenMotionProperty);
        set => SetValue(OpenMotionProperty, value);
    }

    public AbstractMotion? CloseMotion
    {
        get => GetValue(CloseMotionProperty);
        set => SetValue(CloseMotionProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<ContextMenu, double>(nameof(ItemHeight));

    internal static readonly StyledProperty<double> MaxPopupHeightProperty =
        AvaloniaProperty.Register<ContextMenu, double>(nameof(MaxPopupHeight));

    internal double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    internal double MaxPopupHeight
    {
        get => GetValue(MaxPopupHeightProperty);
        set => SetValue(MaxPopupHeightProperty, value);
    }

    #endregion

    private Popup? _popup;
    private WindowBase? _attachedWindow;

    static ContextMenu()
    {
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<ContextMenu>(false);
    }

    public ContextMenu()
    {
        this.RegisterTokenResourceScope(MenuToken.ScopeProvider);
        CreatePopup();
    }

    private Popup CreatePopup()
    {
        _popup = new Popup
        {
            IsLightDismissEnabled          = true,
            OverlayDismissEventPassThrough = true,
            TakesFocusFromNativeControl    = Popup.GetTakesFocusFromNativeControl(this),
        };
        CustomPopupPlacementCallback =  _popup.HandleCustomPlacement;
        _popup.Opened                += HandlePopupOpened;
        _popup.Closed                += this.OnPopupClosed;
        _popup.AddClosingEventHandler(HandlePopupClosing);
        _popup.KeyUp += this.OnPopupClosing;
        
        _popup[!Popup.PopupRootShadowProperty]       = this[!PopupRootShadowProperty];
        _popup[!Popup.OverlayHostShadowProperty]     = this[!OverlayHostShadowProperty];
        _popup[!Popup.MotionDurationProperty]        = this[!MotionDurationProperty];
        _popup[!Popup.OpenMotionProperty]            = this[!OpenMotionProperty];
        _popup[!Popup.CloseMotionProperty]           = this[!CloseMotionProperty];
        _popup[!Popup.IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
        _popup[!Popup.ShouldUseOverlayLayerProperty] = this[!ShouldUseOverlayPopupProperty];

        this.SetPopup(_popup);
        return _popup;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        CustomPopupPlacementCallback =  _popup!.HandleCustomPlacement;
    }

    private void HandlePopupClosing(object? sender, CancelEventArgs e)
    {
        if (!e.Cancel)
        {
            this.OnPopupClosing(sender, e);
        }
    }

    private void HandlePopupOpened(object? sender, EventArgs e)
    {
        this.OnPopupOpened(sender, e);
        if (_popup?.PlacementTarget is { } target)
        {
            var topLevel = TopLevel.GetTopLevel(target);
            if (topLevel is WindowBase window)
            {
                _attachedWindow    =  window;
                window.Deactivated += HandleWindowDeactivated;
            }
        }
    }

    private void HandleWindowDeactivated(object? sender, EventArgs e)
    {
        Close();
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is MenuSeparatorData)
        {
            return new MenuSeparator();
        }

        return new MenuItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (item is MenuItem or MenuSeparator)
        {
            recycleKey = null;
            return false;
        }

        recycleKey = DefaultRecycleKey;
        return true;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is MenuItem menuItem)
        {
            if (item != null && item is not Visual)
            {
                if (!menuItem.IsSet(MenuItem.HeaderProperty))
                {
                    menuItem.SetCurrentValue(MenuItem.HeaderProperty, item);
                }

                if (item is IMenuItemData menuItemData)
                {
                    if (!menuItem.IsSet(MenuItem.IconProperty))
                    {
                        menuItem.SetCurrentValue(MenuItem.IconProperty, menuItemData.Icon);
                    }

                    if (menuItem.ItemKey == null)
                    {
                        menuItem.ItemKey = menuItemData.ItemKey;
                    }

                    if (!menuItem.IsSet(MenuItem.IsEnabledProperty))
                    {
                        menuItem.SetCurrentValue(IsEnabledProperty, menuItemData.IsEnabled);
                    }

                    if (!menuItem.IsSet(MenuItem.InputGestureProperty))
                    {
                        menuItem.SetCurrentValue(MenuItem.InputGestureProperty, menuItemData.InputGesture);
                    }
                }
            }

            if (ItemTemplate != null)
            {
                menuItem[!MenuItem.HeaderTemplateProperty] = this[!ItemTemplateProperty];
            }

            menuItem[!IsMotionEnabledProperty]                = this[!IsMotionEnabledProperty];
            menuItem[!ItemTemplateProperty]                   = this[!ItemTemplateProperty];
            menuItem[!SizeTypeProperty]                       = this[!SizeTypeProperty];
            menuItem[!MenuItem.DisplayPageSizeProperty]       = this[!DisplayPageSizeProperty];
            menuItem[!MenuItem.ShouldUseOverlayPopupProperty] = this[!ShouldUseOverlayPopupProperty];

            PrepareMenuItem(menuItem, item, index);
        }
        else if (container is MenuSeparator menuSeparator)
        {
            menuSeparator.Orientation = Orientation.Vertical;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container),
                "The container type is incorrect, it must be type MenuItem or MenuSeparator.");
        }
    }

    protected virtual void PrepareMenuItem(MenuItem menuItem, object? item, int index)
    {
    }

    public override void Close()
    {
        if (!IsOpen)
        {
            return;
        }
        
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is MenuItem menuItem)
            {
                menuItem.Close();
            }
        }

        _popup!.IsOpen = false;

        if (_attachedWindow != null)
        {
            _attachedWindow.Deactivated -= HandleWindowDeactivated;
            _attachedWindow              =  null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DisplayPageSizeProperty ||
            change.Property == ItemHeightProperty)
        {
            ConfigureMaxPopupHeight();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureMaxPopupHeight();
    }

    private void ConfigureMaxPopupHeight()
    {
        SetCurrentValue(MaxPopupHeightProperty,
            ItemHeight * DisplayPageSize + Padding.Top + Padding.Bottom);
    }
}
