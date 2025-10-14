using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Controls;

public class MenuFlyoutPresenter : MenuBase,
                                   IArrowAwareShadowMaskInfoProvider,
                                   IMotionAwareControl,
                                   IControlSharedTokenResourcesHost
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
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MenuFlyoutPresenter>();

    /// <summary>
    /// 是否显示指示箭头
    /// </summary>
    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    /// <summary>
    /// 箭头渲染的位置
    /// </summary>
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
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MenuToken.ID;

    #endregion
    
    private ArrowDecoratedBox? _arrowDecoratedBox;
    private readonly Dictionary<MenuItem, CompositeDisposable> _itemsBindingDisposables = new();

    public MenuFlyoutPresenter()
        : base(new DefaultMenuInteractionHandler(true))
    {
        this.RegisterResources();
        Items.CollectionChanged += HandleCollectionChanged;
    }

    public MenuFlyoutPresenter(IMenuInteractionHandler menuInteractionHandler)
        : base(menuInteractionHandler)
    {
        this.RegisterResources();
        Items.CollectionChanged += HandleCollectionChanged;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is MenuItem menuItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(menuItem, out var disposable))
                        {
                            disposable.Dispose();
                            _itemsBindingDisposables.Remove(menuItem);
                        }
                    }
                }
            }
        }
    }

    public override void Close()
    {
        // DefaultMenuInteractionHandler calls this
        if (MenuFlyout is not null)
        {
            SelectedIndex = -1;
            MenuFlyout.Hide();
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
    
    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is MenuItem menuItem)
        {
            BindMenuItemClickedRecursive(menuItem);
        }
    }

    private void BindMenuItemClickedRecursive(MenuItem menuItem)
    {
        foreach (var childItem in menuItem.Items)
        {
            if (childItem is MenuItem childMenuItem)
            {
                BindMenuItemClickedRecursive(childMenuItem);
            }
        }

        // 绑定自己
        menuItem.Click += HandleMenuItemClicked;
    }

    private void ClearMenuItemClickedRecursive(MenuItem menuItem)
    {
        foreach (var childItem in menuItem.Items)
        {
            if (childItem is MenuItem childMenuItem)
            {
                ClearMenuItemClickedRecursive(childMenuItem);
            }
        }

        // 绑定自己
        menuItem.Click -= HandleMenuItemClicked;
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        base.ClearContainerForItemOverride(container);
        if (container is MenuItem menuItem)
        {
            ClearMenuItemClickedRecursive(menuItem);
        }
    }

    private void HandleMenuItemClicked(object? sender, RoutedEventArgs args)
    {
        if (sender is MenuItem menuItem)
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
        _arrowDecoratedBox = e.NameScope.Find<ArrowDecoratedBox>(MenuFlyoutThemeConstants.ArrowDecoratorPart);
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
    
    ArrowDecoratedBox IArrowAwareShadowMaskInfoProvider.GetArrowDecoratedBox()
    {
        Debug.Assert(_arrowDecoratedBox != null);
        return _arrowDecoratedBox;
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            var disposables = new CompositeDisposable(1);
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty));
            if (_itemsBindingDisposables.TryGetValue(menuItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(menuItem);
            }
            _itemsBindingDisposables.Add(menuItem, disposables);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }
    
    private void ConfigureMaxPopupHeight()
    {
        SetCurrentValue(MaxPopupHeightProperty, ItemHeight * DisplayPageSize + Padding.Top + Padding.Bottom);
    }
}

public class FlyoutMenuItemClickedEventArgs : RoutedEventArgs
{
   /// <summary>
   /// 当前鼠标点击的菜单项
   /// </summary>
   public MenuItem Item { get; }

    public FlyoutMenuItemClickedEventArgs(RoutedEvent routedEvent, MenuItem menuItem)
        : base(routedEvent)
    {
        Item = menuItem;
    }
}