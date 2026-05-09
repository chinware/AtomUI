using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class MenuFlyoutPresenter : MenuBase,
                                   IArrowAwareShadowMaskInfoProvider,
                                   IMotionAwareControl,
                                   ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<MenuFlyoutPresenter>();

    public static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
        ArrowDecoratedBox.ArrowPositionProperty.AddOwner<MenuFlyoutPresenter>();

    public static readonly StyledProperty<int> DisplayPageSizeProperty =
        Menu.DisplayPageSizeProperty.AddOwner<MenuFlyoutPresenter>();

    public static readonly RoutedEvent<FlyoutMenuItemClickedEventArgs> MenuItemClickedEvent =
        RoutedEvent.Register<MenuFlyoutPresenter, FlyoutMenuItemClickedEventArgs>(
            nameof(MenuItemClicked),
            RoutingStrategies.Bubble);

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<MenuFlyoutPresenter>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MenuFlyoutPresenter>();

    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        Menu.ShouldUseOverlayPopupProperty.AddOwner<MenuFlyoutPresenter>();

    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    public ArrowPosition ArrowPosition
    {
        get => GetValue(ArrowPositionProperty);
        set => SetValue(ArrowPositionProperty, value);
    }

    public int DisplayPageSize
    {
        get => GetValue(DisplayPageSizeProperty);
        set => SetValue(DisplayPageSizeProperty, value);
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

    public event EventHandler<FlyoutMenuItemClickedEventArgs>? MenuItemClicked
    {
        add => AddHandler(MenuItemClickedEvent, value);
        remove => RemoveHandler(MenuItemClickedEvent, value);
    }

    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }

    public MenuFlyout? MenuFlyout { get; set; }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<MenuFlyoutPresenter, double>(nameof(ItemHeight));

    internal static readonly StyledProperty<double> MaxPopupHeightProperty =
        AvaloniaProperty.Register<MenuFlyoutPresenter, double>(nameof(MaxPopupHeight));

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

    private ArrowDecoratedBox? _arrowDecoratedBox;

    static MenuFlyoutPresenter()
    {
        Avalonia.Controls.MenuItem.ClickEvent.AddClassHandler<MenuFlyoutPresenter>(
            (presenter, args) => presenter.HandleMenuItemClicked(args));
    }

    public MenuFlyoutPresenter()
        : base(new DefaultMenuInteractionHandler(true))
    {
        this.RegisterTokenResourceScope(MenuToken.ScopeProvider);
    }

    public MenuFlyoutPresenter(IMenuInteractionHandler menuInteractionHandler)
        : base(menuInteractionHandler)
    {
        this.RegisterTokenResourceScope(MenuToken.ScopeProvider);
    }

    public override void Close()
    {
        // DefaultMenuInteractionHandler calls this
        var host = this.FindLogicalAncestorOfType<Popup>();
        if (host != null)
        {
            SelectedIndex = -1;
            host.IsOpen   = false;
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

            menuItem[!MenuItem.IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
            menuItem[!MenuItem.ItemTemplateProperty]          = this[!ItemTemplateProperty];
            menuItem[!MenuItem.SizeTypeProperty]              = this[!SizeTypeProperty];
            menuItem[!MenuItem.DisplayPageSizeProperty]       = this[!DisplayPageSizeProperty];
            menuItem[!MenuItem.ShouldUseOverlayPopupProperty] = this[!ShouldUseOverlayPopupProperty];

            PrepareMenuItem(menuItem, item, index);
        }
        else if (container is MenuSeparator menuSeparator)
        {
            menuSeparator.Orientation = Orientation.Horizontal;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container),
                "The container type is incorrect, it must be type MenuItem or MenuSeparator.");
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }

    protected virtual void PrepareMenuItem(MenuItem menuItem, object? item, int index)
    {
    }

    private void HandleMenuItemClicked(RoutedEventArgs args)
    {
        if (args.Source is MenuItem menuItem)
        {
            var ev = new FlyoutMenuItemClickedEventArgs(MenuItemClickedEvent, menuItem);
            RaiseEvent(ev);
        }
    }

    public override void Open()
    {
        throw new NotSupportedException("Use MenuFlyout.ShowAt(Control) instead");
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _arrowDecoratedBox =
            e.NameScope.Find<ArrowDecoratedBox>(AbstractArrowDecoratedBox.ArrowDecoratorPart);
        ConfigureMaxPopupHeight();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        foreach (var i in LogicalChildren)
        {
            if (i is MenuItem menuItem)
            {
                menuItem.IsSubMenuOpen = false;
            }
        }
    }

    public CornerRadius GetMaskCornerRadius()
    {
        if (_arrowDecoratedBox is not null)
        {
            return _arrowDecoratedBox.CornerRadius;
        }

        return new CornerRadius(0);
    }

    public Rect GetMaskBounds()
    {
        if (_arrowDecoratedBox is not null)
        {
            var contentRect = _arrowDecoratedBox.GetMaskBounds();
            var adjustedPos = _arrowDecoratedBox.TranslatePoint(contentRect.Position, this) ?? default;
            return new Rect(adjustedPos, contentRect.Size);
        }

        return Bounds;
    }

    public IBrush? GetMaskBackground()
    {
        return _arrowDecoratedBox?.Background;
    }

    ArrowPosition IArrowAwareShadowMaskInfoProvider.GetArrowPosition()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.ArrowPosition;
    }

    bool IArrowAwareShadowMaskInfoProvider.IsShowArrow()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.IsShowArrow;
    }

    void IArrowAwareShadowMaskInfoProvider.SetArrowOpacity(double opacity)
    {
        Debug.Assert(_arrowDecoratedBox != null);
        _arrowDecoratedBox.ArrowOpacity = opacity;
    }

    Rect IArrowAwareShadowMaskInfoProvider.GetArrowIndicatorBounds()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.ArrowIndicatorBounds;
    }

    Rect IArrowAwareShadowMaskInfoProvider.GetArrowIndicatorLayoutBounds()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox.ArrowIndicatorLayoutBounds;
    }

    AbstractArrowDecoratedBox IArrowAwareShadowMaskInfoProvider.GetArrowDecoratedBox()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox;
    }

    private void ConfigureMaxPopupHeight()
    {
        SetCurrentValue(MaxPopupHeightProperty,
            ItemHeight * DisplayPageSize + Padding.Top + Padding.Bottom);
    }
}

public class FlyoutMenuItemClickedEventArgs : RoutedEventArgs
{
    public MenuItem Item { get; }

    public FlyoutMenuItemClickedEventArgs(RoutedEvent routedEvent, MenuItem menuItem)
        : base(routedEvent)
    {
        Item = menuItem;
    }
}