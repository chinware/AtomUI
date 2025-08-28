using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Windows.Input;
using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Input;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Point = Avalonia.Point;

namespace AtomUI.Controls;

[PseudoClasses(NavMenuItemPseudoClass.Separator, 
    NavMenuItemPseudoClass.Icon, 
    StdPseudoClass.Open,
    StdPseudoClass.Pressed, 
    StdPseudoClass.Selected, 
    NavMenuItemPseudoClass.TopLevel)]
public class NavMenuItem : HeaderedSelectingItemsControl,
                           INavMenuItem,
                           ISelectable,
                           ICommandSource,
                           IClickableControl,
                           ICustomHitTest
{
    #region 公共属性定义
    
    public static readonly StyledProperty<ICommand?> CommandProperty =
        Button.CommandProperty.AddOwner<NavMenuItem>(new(enableDataValidation: true));
    
    public static readonly StyledProperty<KeyGesture?> HotKeyProperty =
        HotKeyManager.HotKeyProperty.AddOwner<NavMenuItem>();
    
    public static readonly StyledProperty<object?> CommandParameterProperty =
        Button.CommandParameterProperty.AddOwner<NavMenuItem>();
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<NavMenuItem, Icon?>(nameof(Icon));
    
    public static readonly StyledProperty<KeyGesture?> InputGestureProperty =
        AvaloniaProperty.Register<NavMenuItem, KeyGesture?>(nameof(InputGesture));
    
    public static readonly StyledProperty<bool> IsSubMenuOpenProperty =
        AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsSubMenuOpen));
    
    public static readonly StyledProperty<bool> StaysOpenOnClickProperty =
        AvaloniaProperty.Register<NavMenuItem, bool>(nameof(StaysOpenOnClick));
    
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsChecked));
    
    public static readonly DirectProperty<NavMenuItem, int> LevelProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, int>(
            nameof(Level), o => o.Level);
    
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    
    public KeyGesture? HotKey
    {
        get => GetValue(HotKeyProperty);
        set => SetValue(HotKeyProperty, value);
    }
    
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the input gesture that will be displayed in the menu item.
    /// </summary>
    /// <remarks>
    /// Setting this property does not cause the input gesture to be handled by the menu item,
    /// it simply displays the gesture text in the menu.
    /// </remarks>
    public KeyGesture? InputGesture
    {
        get => GetValue(InputGestureProperty);
        set => SetValue(InputGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="NavMenuItem"/> is currently selected.
    /// </summary>
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the submenu of the <see cref="NavMenuItem"/> is
    /// open.
    /// </summary>
    public bool IsSubMenuOpen
    {
        get => GetValue(IsSubMenuOpenProperty);
        set => SetValue(IsSubMenuOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the submenu that this <see cref="NavMenuItem"/> is
    /// within should not close when this item is clicked.
    /// </summary>
    public bool StaysOpenOnClick
    {
        get => GetValue(StaysOpenOnClickProperty);
        set => SetValue(StaysOpenOnClickProperty, value);
    }

    /// <inheritdoc cref="IMenuItem.IsChecked"/>
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    private bool _hasSubMenu;

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="NavMenuItem"/> has a submenu.
    /// </summary>
    public bool HasSubMenu
    {
        get => _hasSubMenu;
        set => SetAndRaise(HasSubMenuProperty, ref _hasSubMenu, value);
    }

    private int _level;

    /// <summary>
    /// Gets the level/indentation of the item.
    /// </summary>
    public int Level
    {
        get => _level;
        private set => SetAndRaise(LevelProperty, ref _level, value);
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="NavMenuItem"/> is a top-level main menu item.
    /// </summary>
    public bool IsTopLevel => Parent is NavMenu;

    /// <inheritdoc/>
    bool INavMenuItem.IsPointerOverSubMenu => _popup?.IsPointerOverPopup ?? false;

    /// <summary>
    /// 获取或者设置菜单项的 Key
    /// </summary>
    public string? ItemKey { get; set; }

    /// <inheritdoc/>
    INavMenuElement? INavMenuItem.Parent => Parent as INavMenuElement;

    protected override bool IsEnabledCore => base.IsEnabledCore && _commandCanExecute;

    /// <inheritdoc/>
    INavMenuItem? INavMenuElement.SelectedItem
    {
        get
        {
            var index = SelectedIndex;
            return (index != -1) ? (INavMenuItem?)ContainerFromIndex(index) : null;
        }

        set => SelectedIndex = value is Control c ? IndexFromContainer(c) : -1;
    }

    /// <inheritdoc/>
    IEnumerable<INavMenuItem> INavMenuElement.SubItems => LogicalChildren.OfType<INavMenuItem>();

    private INavMenuInteractionHandler? MenuInteractionHandler =>
        this.FindLogicalAncestorOfType<NavMenuBase>()?.InteractionHandler;

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> ActiveBarWidthProperty =
        NavMenu.ActiveBarWidthProperty.AddOwner<NavMenuItem>();

    internal static readonly StyledProperty<double> ActiveBarHeightProperty =
        NavMenu.ActiveBarHeightProperty.AddOwner<NavMenuItem>();

    internal static readonly DirectProperty<NavMenuItem, double> EffectiveActiveBarWidthProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, double>(nameof(EffectiveActiveBarWidth),
            o => o.EffectiveActiveBarWidth,
            (o, v) => o.EffectiveActiveBarWidth = v);

    internal static readonly DirectProperty<NavMenuItem, double> EffectivePopupMinWidthProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, double>(nameof(EffectivePopupMinWidth),
            o => o.EffectivePopupMinWidth,
            (o, v) => o.EffectivePopupMinWidth = v);

    internal static readonly DirectProperty<NavMenuItem, double> PopupMinWidthProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, double>(nameof(PopupMinWidth),
            o => o.PopupMinWidth,
            (o, v) => o.PopupMinWidth = v);

    internal static readonly DirectProperty<NavMenuItem, NavMenuMode> ModeProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, NavMenuMode>(nameof(Mode),
            o => o.Mode,
            (o, v) => o.Mode = v);

    internal static readonly DirectProperty<NavMenuItem, TimeSpan> OpenCloseMotionDurationProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, TimeSpan>(nameof(OpenCloseMotionDuration),
            o => o.OpenCloseMotionDuration,
            (o, v) => o.OpenCloseMotionDuration = v);

    internal static readonly DirectProperty<NavMenuItem, bool> HasSubMenuProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, bool>(nameof(HasSubMenu),
            o => o.HasSubMenu,
            (o, v) => o.HasSubMenu = v);

    internal static readonly DirectProperty<NavMenuItem, double> InlineItemIndentUnitProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, double>(nameof(InlineItemIndentUnit),
            o => o.InlineItemIndentUnit,
            (o, v) => o.InlineItemIndentUnit = v);

    internal static readonly DirectProperty<NavMenuItem, bool> IsDarkStyleProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, bool>(nameof(IsDarkStyle),
            o => o.IsDarkStyle,
            (o, v) => o.IsDarkStyle = v);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<NavMenuItem>();

    internal double ActiveBarWidth
    {
        get => GetValue(ActiveBarWidthProperty);
        set => SetValue(ActiveBarWidthProperty, value);
    }

    internal double ActiveBarHeight
    {
        get => GetValue(ActiveBarHeightProperty);
        set => SetValue(ActiveBarHeightProperty, value);
    }

    private double _effectiveActiveBarWidth;

    internal double EffectiveActiveBarWidth
    {
        get => _effectiveActiveBarWidth;
        set => SetAndRaise(EffectiveActiveBarWidthProperty, ref _effectiveActiveBarWidth, value);
    }

    private double _effectivePopupMinWidth;

    internal double EffectivePopupMinWidth
    {
        get => _effectivePopupMinWidth;
        set => SetAndRaise(EffectivePopupMinWidthProperty, ref _effectivePopupMinWidth, value);
    }

    private double _popupMinWidth;

    internal double PopupMinWidth
    {
        get => _popupMinWidth;
        set => SetAndRaise(PopupMinWidthProperty, ref _popupMinWidth, value);
    }

    private TimeSpan _openCloseMotionDuration;

    internal TimeSpan OpenCloseMotionDuration
    {
        get => _openCloseMotionDuration;
        set => SetAndRaise(OpenCloseMotionDurationProperty, ref _openCloseMotionDuration, value);
    }

    private NavMenuMode _mode;

    internal NavMenuMode Mode
    {
        get => _mode;
        set => SetAndRaise(ModeProperty, ref _mode, value);
    }

    private double _inlineItemIndentUnit;

    internal double InlineItemIndentUnit
    {
        get => _inlineItemIndentUnit;
        set => SetAndRaise(InlineItemIndentUnitProperty, ref _inlineItemIndentUnit, value);
    }

    private bool _isDarkStyle;

    internal bool IsDarkStyle
    {
        get => _isDarkStyle;
        set => SetAndRaise(IsDarkStyleProperty, ref _isDarkStyle, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 公共事件定义

    /// <summary>
    /// Defines the <see cref="Click"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<NavMenuItem, RoutedEventArgs>(
            nameof(Click),
            RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="PointerExitedItem"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> PointerEnteredItemEvent =
        RoutedEvent.Register<NavMenuItem, RoutedEventArgs>(
            nameof(PointerEnteredItem),
            RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="SubmenuOpened"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> PointerExitedItemEvent =
        RoutedEvent.Register<NavMenuItem, RoutedEventArgs>(
            nameof(PointerExitedItem),
            RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="SubmenuOpened"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> SubmenuOpenedEvent =
        RoutedEvent.Register<NavMenuItem, RoutedEventArgs>(
            nameof(SubmenuOpened),
            RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when a <see cref="NavMenuItem"/> without a submenu is clicked.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    /// <summary>
    /// Occurs when the pointer enters a menu item.
    /// </summary>
    /// <remarks>
    /// A bubbling version of the <see cref="InputElement.PointerEntered"/> event for menu items.
    /// </remarks>
    public event EventHandler<RoutedEventArgs>? PointerEnteredItem
    {
        add => AddHandler(PointerEnteredItemEvent, value);
        remove => RemoveHandler(PointerEnteredItemEvent, value);
    }

    /// <summary>
    /// Raised when the pointer leaves a menu item.
    /// </summary>
    /// <remarks>
    /// A bubbling version of the <see cref="InputElement.PointerExited"/> event for menu items.
    /// </remarks>
    public event EventHandler<RoutedEventArgs>? PointerExitedItem
    {
        add => AddHandler(PointerExitedItemEvent, value);
        remove => RemoveHandler(PointerExitedItemEvent, value);
    }

    /// <summary>
    /// Occurs when a <see cref="NavMenuItem"/>'s submenu is opened.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? SubmenuOpened
    {
        add => AddHandler(SubmenuOpenedEvent, value);
        remove => RemoveHandler(SubmenuOpenedEvent, value);
    }

    #endregion

    #region 私有事件定义

    private EventHandler? _canExecuteChangeHandler = null;

    private EventHandler CanExecuteChangedHandler => _canExecuteChangeHandler ??= new(CanExecuteChanged);

    #endregion
    
    internal static PlatformKeyGestureConverter KeyGestureConverter = new();
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());

    private bool _commandCanExecute = true;
    private bool _commandBindingError;
    private Popup? _popup;
    private KeyGesture? _hotkey;
    private bool _isEmbeddedInMenu;
    private Border? _horizontalFrame;
    private BaseMotionActor? _childItemsLayoutTransform;
    private Border? _headerFrame;
    
    // inline
    private Border? _menuIndicatorIconFrame;
    
    // toplevel horizontal
    private Rectangle? _activeIndicator;
    
    private bool _animating;
    
    internal Popup? Popup => _popup;

    private readonly Dictionary<NavMenuItem, CompositeDisposable> _itemsBindingDisposables = new();
    
    static NavMenuItem()
    {
        SelectableMixin.Attach<NavMenuItem>(IsSelectedProperty);
        PressedMixin.Attach<NavMenuItem>();
        FocusableProperty.OverrideDefaultValue<NavMenuItem>(true);
        ItemsPanelProperty.OverrideDefaultValue<NavMenuItem>(DefaultPanel);
        ClickEvent.AddClassHandler<NavMenuItem>((x, e) => x.OnClick(e));
        SubmenuOpenedEvent.AddClassHandler<NavMenuItem>((x, e) => x.OnSubmenuOpened(e));
    }

    public NavMenuItem()
    {
        Items.CollectionChanged  += HandleItemsCollectionChanged;
    }
    
    private void HandleItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is NavMenuItem menuItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(menuItem, out var disposable))
                        {
                            disposable.Dispose();
                        }
                        _itemsBindingDisposables.Remove(menuItem);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Opens the submenu.
    /// </summary>
    /// <remarks>
    /// This has the same effect as setting <see cref="IsSubMenuOpen"/> to true.
    /// </remarks>
    public void Open()
    {
        if (Mode == NavMenuMode.Inline)
        {
            if (_animating)
            {
                return;
            }
        }

        IsSubMenuOpen = true;
    }

    /// <summary>
    /// Closes the submenu.
    /// </summary>
    /// <remarks>
    /// This has the same effect as setting <see cref="IsSubMenuOpen"/> to false.
    /// </remarks>
    public void Close()
    {
        if (Mode == NavMenuMode.Inline)
        {
            if (_animating)
            {
                return;
            }                                       
        }
        
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await CloseItemAsync(this);
        });
    }

    public async Task CloseItemAsync(INavMenuItem navMenuItem)
    {

        foreach (var child in navMenuItem.SubItems)
        {
            if (child is NavMenuItem childNavMenuItem)
            {
                await CloseItemAsync(childNavMenuItem);
            }
        }

        if (navMenuItem is NavMenuItem navMenuItem2)
        {
            if (navMenuItem2._popup != null && navMenuItem2._popup.IsMotionAwareOpen)
            {
                await navMenuItem2._popup.MotionAwareCloseAsync();
            }

            navMenuItem2.IsSubMenuOpen = false;
        }
    }

    /// <inheritdoc/>
    void INavMenuItem.RaiseClick() => RaiseEvent(new RoutedEventArgs(ClickEvent));

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new NavMenuItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (item is NavMenuItem or Separator)
        {
            recycleKey = null;
            return false;
        }

        recycleKey = DefaultRecycleKey;
        return true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!_isEmbeddedInMenu)
        {
            //Normally the Menu's IMenuInteractionHandler is sending the click events for us
            //However when the item is not embedded into a menu we need to send them ourselves.
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        if (_hotkey != null) // Control attached again, set Hotkey to create a hotkey manager for this control
        {
            SetCurrentValue(HotKeyProperty, _hotkey);
        }

        base.OnAttachedToLogicalTree(e);

        Level = CalculateDistanceFromLogicalParent<NavMenu>(this) - 1;

        (var command, var parameter) = (Command, CommandParameter);
        if (command is not null)
        {
            command.CanExecuteChanged += CanExecuteChangedHandler;
        }

        TryUpdateCanExecute(command, parameter);

        var parent = Parent;

        while (parent is NavMenuItem)
        {
            parent = parent.Parent;
        }

        _isEmbeddedInMenu = parent?.FindLogicalAncestorOfType<INavMenu>(true) != null;
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        // This will cause the hotkey manager to dispose the observer and the reference to this control
        if (HotKey != null)
        {
            _hotkey = HotKey;
            SetCurrentValue(HotKeyProperty, null);
        }

        base.OnDetachedFromLogicalTree(e);

        if (Command != null)
        {
            Command.CanExecuteChanged -= CanExecuteChangedHandler;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
        TryUpdateCanExecute();
    }

    /// <summary>
    /// Called when the <see cref="NavMenuItem"/> is clicked.
    /// </summary>
    /// <param name="e">The click event args.</param>
    protected virtual void OnClick(RoutedEventArgs e)
    {
        (var command, var parameter) = (Command, CommandParameter);
        if (!e.Handled && command is not null && command.CanExecute(parameter) == true)
        {
            command.Execute(parameter);
            e.Handled = true;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Don't handle here: let event bubble up to menu.
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        RaiseEvent(new RoutedEventArgs(PointerEnteredItemEvent));
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        RaiseEvent(new RoutedEventArgs(PointerExitedItemEvent));
    }

    /// <summary>
    /// Called when a submenu is opened on this NavMenuItem or a child NavMenuItem.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected virtual void OnSubmenuOpened(RoutedEventArgs e)
    {
        var menuItem = e.Source as NavMenuItem;

        if (menuItem != null && menuItem.Parent == this)
        {
            // TODO 我们在这里对模式做一个区分, Inline 暂时不互斥关闭，后面有时间看是否加一个互斥的标记
            if (Mode != NavMenuMode.Inline)
            {
                foreach (var child in ((INavMenuItem)this).SubItems)
                {
                    if (child != menuItem && child.IsSubMenuOpen)
                    {
                        child.IsSubMenuOpen = false;
                    }
                }
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_popup != null)
        {
            _popup.Opened             -= PopupOpened;
            _popup.Closed             -= PopupClosed;
            _popup.DependencyResolver =  null;
        }

        _popup       = e.NameScope.Find<Popup>(NavMenuItemThemeConstants.PopupPart);
        _headerFrame = e.NameScope.Find<Border>(BaseNavMenuItemThemeConstants.HeaderDecoratorPart);
        if (_headerFrame is not null)
        {
            _headerFrame.PointerEntered += HandleHeaderFrameEnter;
            _headerFrame.PointerExited  += HandleHeaderFrameExit;
        }

        if (_popup != null)
        {
            _popup.Opened += PopupOpened;
            _popup.Closed += PopupClosed;
        }

        _horizontalFrame = e.NameScope.Find<Border>(TopLevelHorizontalNavMenuItemThemeConstants.FramePart);

        if (Mode == NavMenuMode.Inline)
        {
            _menuIndicatorIconFrame = e.NameScope.Find<Border>(InlineNavMenuItemThemeConstants.MenuIndicatorIconFramePart);
        }

        if (IsTopLevel && Mode == NavMenuMode.Horizontal)
        {
            _activeIndicator =
                e.NameScope.Find<Rectangle>(TopLevelHorizontalNavMenuItemThemeConstants.ActiveIndicatorPart);
        }
        
        if (Mode == NavMenuMode.Inline)
        {
            _childItemsLayoutTransform =
                e.NameScope.Find<BaseMotionActor>(InlineNavMenuItemThemeConstants.ChildItemsLayoutTransformPart);
            if (_childItemsLayoutTransform is not null)
            {
                _childItemsLayoutTransform.SetCurrentValue(IsVisibleProperty, IsSubMenuOpen);
            }
        }

        if (_headerFrame != null)
        {
            _headerFrame.Loaded += (sender, args) =>
            {
                ConfigureHeaderFrameTransitions(false);
            };
            _headerFrame.Unloaded += (sender, args) =>
            {
                _headerFrame.Transitions = null;
            };
        }

        if (_menuIndicatorIconFrame != null)
        {
            _menuIndicatorIconFrame.Loaded += (sender, args) =>
            {
                ConfigureMenuIndicatorIconFrameTransitions(false);
            };
            _menuIndicatorIconFrame.Unloaded += (sender, args) =>
            {
                _menuIndicatorIconFrame.Transitions = null;
            };
        }

        if (_activeIndicator != null)
        {
            _activeIndicator.Loaded += (sender, args) =>
            {
                ConfigureActiveIndicatorTransitions(false);
            };
            _activeIndicator.Unloaded += (sender, args) =>
            {
                _activeIndicator.Transitions = null;
            };
        }
    }

    private void ConfigureHeaderFrameTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (_headerFrame != null)
            {
                if (force || _headerFrame.Transitions == null)
                {
                    _headerFrame.Transitions = [
                        TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                        TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
                    ];
                }
            }
        }
        else
        {
            if (_headerFrame != null)
            {
                _headerFrame.Transitions = null;
            }
        }
    }
    
    private void ConfigureMenuIndicatorIconFrameTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (_menuIndicatorIconFrame != null)
            {
                if (force || _menuIndicatorIconFrame.Transitions == null)
                {
                    // inline mode
                    _menuIndicatorIconFrame.Transitions = [
                        TransitionUtils.CreateTransition<TransformOperationsTransition>(RenderTransformProperty)
                    ];
                }
            }
        }
        else
        {
            if (_menuIndicatorIconFrame != null)
            {
                _menuIndicatorIconFrame.Transitions = null;
            }
        }
    }
    
    private void ConfigureActiveIndicatorTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            // toplevel horizontal
            if (_activeIndicator != null)
            {
                if (force || _activeIndicator.Transitions == null)
                {
                    _activeIndicator.Transitions =
                    [
                        TransitionUtils.CreateTransition<SolidColorBrushTransition>(Rectangle.FillProperty)
                    ];
                }
            }
        }
        else
        {
            if (_activeIndicator != null)
            {
                _activeIndicator.Transitions = null;
            }
        }
    }

    internal void DisableAllTransitions()
    {
        if (_headerFrame != null)
        {
            _headerFrame.DisableTransitions();
        }
            
        // inline mode
        if (_menuIndicatorIconFrame != null)
        {
            _menuIndicatorIconFrame.DisableTransitions();
        }
            
        // toplevel horizontal
        if (_activeIndicator != null)
        {
            _activeIndicator.DisableTransitions();
        }
    }

    internal void EnableAllTransitions()
    {
        if (_headerFrame != null)
        {
            _headerFrame.EnableTransitions();
        }
            
        // inline mode
        if (_menuIndicatorIconFrame != null)
        {
            _menuIndicatorIconFrame.EnableTransitions();
        }
            
        // toplevel horizontal
        if (_activeIndicator != null)
        {
            _activeIndicator.EnableTransitions();
        }
    }

    private void HandleHeaderFrameEnter(object? sender, PointerEventArgs args)
    {
        if (!IsDarkStyle || Icon is null || HasSubMenu)
        {
            return;
        }

        Icon.IconMode = IconMode.Active;
    }

    private void HandleHeaderFrameExit(object? sender, PointerEventArgs args)
    {
        if (!IsDarkStyle || Icon is null || HasSubMenu)
        {
            return;
        }

        if (IsSelected)
        {
            Icon.IconMode = IconMode.Selected;
        }
        else
        {
            Icon.IconMode = IconMode.Normal;
        }
    }

    protected override void UpdateDataValidation(
        AvaloniaProperty property,
        BindingValueType state,
        Exception? error)
    {
        base.UpdateDataValidation(property, state, error);
        if (property == CommandProperty)
        {
            _commandBindingError = state == BindingValueType.BindingError;
            if (_commandBindingError && _commandCanExecute)
            {
                _commandCanExecute = false;
                UpdateIsEffectivelyEnabled();
            }
        }
    }

    /// <summary>
    /// Closes all submenus of the menu item.
    /// </summary>
    private void CloseSubmenus()
    {
        foreach (var child in ((INavMenuItem)this).SubItems)
        {
            child.IsSubMenuOpen = false;
        }
    }

    /// <summary>
    /// Called when the <see cref="Command"/> property changes.
    /// </summary>
    /// <param name="e">The event args.</param>
    private static void CommandChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newCommand = e.NewValue as ICommand;
        if (e.Sender is NavMenuItem menuItem)

        {
            if (((ILogical)menuItem).IsAttachedToLogicalTree)
            {
                if (e.OldValue is ICommand oldCommand)
                {
                    oldCommand.CanExecuteChanged -= menuItem.CanExecuteChangedHandler;
                }

                if (newCommand is not null)
                {
                    newCommand.CanExecuteChanged += menuItem.CanExecuteChangedHandler;
                }
            }

            menuItem.TryUpdateCanExecute(newCommand, menuItem.CommandParameter);
        }
    }

    /// <summary>
    /// Called when the <see cref="CommandParameter"/> property changes.
    /// </summary>
    /// <param name="e">The event args.</param>
    private static void CommandParameterChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is NavMenuItem menuItem)
        {
            (var command, var parameter) = (menuItem.Command, e.NewValue);
            menuItem.TryUpdateCanExecute(command, parameter);
        }
    }

    /// <summary>
    /// Called when the <see cref="ICommand.CanExecuteChanged"/> event fires.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event args.</param>
    public void CanExecuteChanged(object? sender, EventArgs e)
    {
        TryUpdateCanExecute();
    }

    /// <summary>
    /// Tries to evaluate CanExecute value of a Command if menu is opened
    /// </summary>
    private void TryUpdateCanExecute()
    {
        TryUpdateCanExecute(Command, CommandParameter);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void TryUpdateCanExecute(ICommand? command, object? parameter)
    {
        if (command == null)
        {
            _commandCanExecute = !_commandBindingError;
            UpdateIsEffectivelyEnabled();
            return;
        }

        //Perf optimization - only raise CanExecute event if the menu is open
        if (!((ILogical)this).IsAttachedToLogicalTree || Parent is NavMenuItem { IsSubMenuOpen: false })
        {
            return;
        }

        var canExecute = command.CanExecute(parameter);
        if (canExecute != _commandCanExecute)
        {
            _commandCanExecute = canExecute;
            UpdateIsEffectivelyEnabled();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == HeaderProperty)
        {
            HeaderChanged(change);
        }
        else if (change.Property == ParentProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == IsSelectedProperty)
        {
            IsSelectedChanged(change);
        }
        else if (change.Property == IsSubMenuOpenProperty)
        {
            SubMenuOpenChanged(change);
        }
        else if (change.Property == CommandProperty)
        {
            CommandChanged(change);
            CommandChanged(change);
        }
        else if (change.Property == CommandParameterProperty)
        {
            CommandParameterChanged(change);
        }
        else if (change.Property == BoundsProperty)
        {
            SetupHorizontalEffectiveIndicatorWidth();
        }
        else if (change.Property == ItemCountProperty)
        {
            HasSubMenu = ItemCount > 0;
        }
        else if (change.Property == BoundsProperty ||
                 change.Property == PopupMinWidthProperty)
        {
            SetupEffectivePopupMinWidth();
        }
        else if (change.Property == IconProperty)
        {
            if (change.OldValue is Icon oldIcon)
            {
                PseudoClasses.Remove(NavMenuItemPseudoClass.Icon);
            }

            if (change.NewValue is Icon newIcon)
            {
                PseudoClasses.Add(NavMenuItemPseudoClass.Icon);
            }
        }

        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureActiveIndicatorTransitions(true);
                ConfigureHeaderFrameTransitions(true);
                ConfigureMenuIndicatorIconFrameTransitions(true);
            }
        }
    }

    private void SetupHorizontalEffectiveIndicatorWidth()
    {
        var width = Bounds.Width;
        if (_horizontalFrame != null)
        {
            width = _horizontalFrame.Bounds.Width;
        }

        EffectiveActiveBarWidth = ActiveBarWidth * width;
    }

    private void SetupEffectivePopupMinWidth()
    {
        if (IsTopLevel)
        {
            if (Parent is NavMenu navMenu)
            {
                if (navMenu.Mode == NavMenuMode.Horizontal)
                {
                    EffectivePopupMinWidth = Math.Max(_horizontalFrame?.Bounds.Width ?? Bounds.Width, PopupMinWidth);
                }
                else
                {
                    EffectivePopupMinWidth = PopupMinWidth;
                }
            }
        }
    }

    /// <summary>
    /// Called when the <see cref="HeaderedSelectingItemsControl.Header"/> property changes.
    /// </summary>
    /// <param name="e">The property change event.</param>
    private void HeaderChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var (oldValue, newValue) = e.GetOldAndNewValue<object?>();
        if (Equals(newValue, "-"))
        {
            PseudoClasses.Add(NavMenuItemPseudoClass.Separator);
            Focusable = false;
        }
        else if (Equals(oldValue, "-"))
        {
            PseudoClasses.Remove(NavMenuItemPseudoClass.Separator);
            Focusable = true;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(NavMenuItemPseudoClass.TopLevel, IsTopLevel);
    }

    /// <summary>
    /// Called when the <see cref="IsSelected"/> property changes.
    /// </summary>
    /// <param name="e">The property change event.</param>
    private void IsSelectedChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var parentMenu = Parent as NavMenu;
        var isSelected = e.GetNewValue<bool>();
        if (isSelected && (parentMenu is null || parentMenu.IsOpen))
        {
            Focus();
        }

        if (Icon is not null)
        {
            if (isSelected)
            {
                Icon.SetValue(Icon.IconModeProperty, IconMode.Selected);
            }
            else
            {
                Icon.SetValue(Icon.IconModeProperty, IconMode.Normal);
            }
        }
    }

    /// <summary>
    /// Called when the <see cref="IsSubMenuOpen"/> property changes.
    /// </summary>
    /// <param name="e">The property change event.</param>
    private void SubMenuOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var value = (bool)e.NewValue!;

        if (Mode == NavMenuMode.Inline)
        {
            // 在这里我们有一个动画的效果
            if (value)
            {
                foreach (var item in ItemsView.OfType<NavMenuItem>())
                {
                    item.TryUpdateCanExecute();
                }

                RaiseEvent(new RoutedEventArgs(SubmenuOpenedEvent));
                PseudoClasses.Add(StdPseudoClass.Open);
                OpenInlineItem();
            }
            else
            {
                PseudoClasses.Remove(StdPseudoClass.Open);
                CloseInlineItem();
            }
        }
        else
        {
            if (value)
            {
                foreach (var item in ItemsView.OfType<NavMenuItem>())
                {
                    item.TryUpdateCanExecute();
                }

                RaiseEvent(new RoutedEventArgs(SubmenuOpenedEvent));
                PseudoClasses.Add(StdPseudoClass.Open);
            }
            else
            {
                CloseSubmenus();
                PseudoClasses.Remove(StdPseudoClass.Open);
            }
        }
    }

    internal void OpenInlineItem(bool forceDisableMotion = false)
    {
        if (_childItemsLayoutTransform is not null)
        {
            if (IsMotionEnabled && !forceDisableMotion)
            {
                if (_animating)
                {
                    return;
                }

                _animating                           = true;
                _childItemsLayoutTransform.IsVisible = true;
                var motion = new SlideUpInMotion(_openCloseMotionDuration, new CubicEaseOut());
                motion.Run(_childItemsLayoutTransform,
                    () => { _childItemsLayoutTransform.IsVisible = true; },
                    () =>
                    {
                        _animating                           = false;
                    });
            }
            else
            {
                _childItemsLayoutTransform.IsVisible = true;
            }
        }
    }

    internal void CloseInlineItem(bool forceDisableMotion = false)
    {
        if (_childItemsLayoutTransform is not null)
        {
            if (IsMotionEnabled && !forceDisableMotion && _childItemsLayoutTransform.IsVisible)
            {
                if (_animating)
                {
                    return;
                }

                _animating                           = true;
                _childItemsLayoutTransform.IsVisible = true;
                var motion = new SlideUpOutMotion(_openCloseMotionDuration, new CubicEaseIn());
                motion.Run(_childItemsLayoutTransform, null, () =>
                {
                    _childItemsLayoutTransform.IsVisible = false;
                    _animating                           = false;
                });
            }
            else
            {
                _childItemsLayoutTransform.IsVisible = false;
            }
        }
    }

    /// <summary>
    /// Called when the submenu's <see cref="Popup"/> is opened.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event args.</param>
    private void PopupOpened(object? sender, EventArgs e)
    {
        // If we're using overlay popups, there's a chance we need to do a layout pass before
        // the child items are added to the visual tree. If we don't do this here, then
        // selection breaks.
        if (Presenter?.GetVisualRoot() != null)
        {
            UpdateLayout();
        }

        var selected = SelectedIndex;

        if (selected != -1)
        {
            var container = ContainerFromIndex(selected);
            container?.Focus();
        }
    }

    /// <summary>
    /// Called when the submenu's <see cref="Popup"/> is closed.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event args.</param>
    private void PopupClosed(object? sender, EventArgs e)
    {
    }

    void ICommandSource.CanExecuteChanged(object sender, EventArgs e) => CanExecuteChangedHandler(sender, e);

    void IClickableControl.RaiseClick()
    {
        if (IsEffectivelyEnabled)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }

    public bool HitTest(Point point)
    {
        if (IsTopLevel)
        {
            if (Parent is NavMenu navMenu)
            {
                if (navMenu.Mode == NavMenuMode.Horizontal && _horizontalFrame is not null)
                {
                    var offset     = _horizontalFrame.TranslatePoint(new Point(0, 0), this) ?? default;
                    var targetRect = new Rect(offset, _horizontalFrame.Bounds.Size);
                    if (targetRect.Contains(point))
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        return true;
    }

    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);
        if (element is NavMenuItem navMenuItem)
        {
            var disposables = new CompositeDisposable(4);
            disposables.Add(BindUtils.RelayBind(this, ModeProperty, navMenuItem, ModeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsDarkStyleProperty, navMenuItem, IsDarkStyleProperty));
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, navMenuItem, IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, ItemContainerThemeProperty, navMenuItem, ItemContainerThemeProperty));
            if (_itemsBindingDisposables.TryGetValue(navMenuItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(navMenuItem);
            }
            _itemsBindingDisposables.Add(navMenuItem, disposables);
        }
    }

    internal void SelectItemRecursively()
    {
        DisableAllTransitions();
        IsSelected = true;
        EnableAllTransitions();
        if (Parent is NavMenuItem parent)
        {
            parent.DisableAllTransitions();
            parent.SelectItemRecursively();
            parent.EnableAllTransitions();
        }
    }

    internal void RegenerateContainers()
    {
        foreach (var item in Items)
        {
            if (item is NavMenuItem childNavMenuItem)
            {
                childNavMenuItem.RegenerateContainers();
            }
        }

        ItemsPanel = new FuncTemplate<Panel?>(() => new StackPanel());
        RefreshContainers();
    }

    private static int CalculateDistanceFromLogicalParent<T>(ILogical? logical, int @default = -1) where T : class
    {
        var result = 0;

        while (logical != null && !(logical is T))
        {
            ++result;
            logical = logical.LogicalParent;
        }

        return logical != null ? result : @default;
    }

    internal bool PointInNavMenuItemHeader(Point point)
    {
        var targetFrame = Mode == NavMenuMode.Horizontal && IsTopLevel ? _horizontalFrame : _headerFrame;
        if (targetFrame is null)
        {
            return false;
        }

        var offset     = targetFrame.TranslatePoint(new Point(0, 0), this) ?? default;
        var targetRect = new Rect(offset, targetFrame.Bounds.Size);
        return targetRect.Contains(point);
    }
    
}