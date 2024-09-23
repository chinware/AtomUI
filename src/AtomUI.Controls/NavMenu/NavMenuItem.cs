using System.Windows.Input;
using AtomUI.Input;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[TemplatePart(ThemeConstants.PopupPart, typeof(Popup))]
[PseudoClasses(SeparatorPC, IconPC, StdPseudoClass.Open, StdPseudoClass.Pressed, StdPseudoClass.Selected, TopLevelPC)]
public class NavMenuItem : HeaderedSelectingItemsControl,
                           INavMenuItem,
                           ISelectable,
                           ICommandSource,
                           IClickableControl
{
    public const string TopLevelPC = ":toplevel";
    public const string SeparatorPC = ":separator";
    public const string IconPC = ":icon";

    #region 公共属性定义

    /// <summary>
    /// Defines the <see cref="Command"/> property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        Button.CommandProperty.AddOwner<NavMenuItem>(new(enableDataValidation: true));

    /// <summary>
    /// Defines the <see cref="HotKey"/> property.
    /// </summary>
    public static readonly StyledProperty<KeyGesture?> HotKeyProperty =
        HotKeyManager.HotKeyProperty.AddOwner<NavMenuItem>();

    /// <summary>
    /// Defines the <see cref="CommandParameter"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        Button.CommandParameterProperty.AddOwner<NavMenuItem>();

    /// <summary>
    /// Defines the <see cref="Icon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<NavMenuItem, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="InputGesture"/> property.
    /// </summary>
    public static readonly StyledProperty<KeyGesture?> InputGestureProperty =
        AvaloniaProperty.Register<NavMenuItem, KeyGesture?>(nameof(InputGesture));

    /// <summary>
    /// Defines the <see cref="IsSubMenuOpen"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSubMenuOpenProperty =
        AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsSubMenuOpen));

    /// <summary>
    /// Defines the <see cref="StaysOpenOnClick"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> StaysOpenOnClickProperty =
        AvaloniaProperty.Register<NavMenuItem, bool>(nameof(StaysOpenOnClick));

    /// <summary>
    /// Defines the <see cref="IsChecked"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsChecked));

    /// <summary>
    /// Gets or sets the command associated with the menu item.
    /// </summary>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets an <see cref="KeyGesture"/> associated with this control
    /// </summary>
    public KeyGesture? HotKey
    {
        get => GetValue(HotKeyProperty);
        set => SetValue(HotKeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter to pass to the <see cref="Command"/> property of a
    /// <see cref="NavMenuItem"/>.
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon that appears in a <see cref="NavMenuItem"/>.
    /// </summary>
    public object? Icon
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

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="NavMenuItem"/> has a submenu.
    /// </summary>
    public bool HasSubMenu => !Classes.Contains(StdPseudoClass.Empty);

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
    bool INavMenuElement.MoveSelection(NavigationDirection direction, bool wrap) => MoveSelection(direction, wrap);

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
    
    public double ActiveBarWidth
    {
        get => GetValue(ActiveBarWidthProperty);
        set => SetValue(ActiveBarWidthProperty, value);
    }
    
    public double ActiveBarHeight
    {
        get => GetValue(ActiveBarHeightProperty);
        set => SetValue(ActiveBarHeightProperty, value);
    }
    
    private double _effectiveActiveBarWidth;
    public double EffectiveActiveBarWidth
    {
        get => _effectiveActiveBarWidth;
        set => SetAndRaise(EffectiveActiveBarWidthProperty, ref _effectiveActiveBarWidth, value);
    }
    
    private double _effectivePopupMinWidth;
    public double EffectivePopupMinWidth
    {
        get => _effectivePopupMinWidth;
        set => SetAndRaise(EffectivePopupMinWidthProperty, ref _effectivePopupMinWidth, value);
    }
    
    private double _popupMinWidth;
    public double PopupMinWidth
    {
        get => _popupMinWidth;
        set => SetAndRaise(PopupMinWidthProperty, ref _popupMinWidth, value);
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

    private EventHandler? _canExecuteChangeHandler = default;

    private EventHandler CanExecuteChangedHandler => _canExecuteChangeHandler ??= new(CanExecuteChanged);

    #endregion
    
    internal static PlatformKeyGestureConverter KeyGestureConverter = new();

    /// <summary>
    /// The default value for the <see cref="ItemsControl.ItemsPanel"/> property.
    /// </summary>
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());

    private bool _commandCanExecute = true;
    private bool _commandBindingError;
    private Popup? _popup;
    private KeyGesture? _hotkey;
    private bool _isEmbeddedInMenu;
    private Border? _horizontalFrame;

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
        AffectsRender<MenuItem>(BackgroundProperty);
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Opens the submenu.
    /// </summary>
    /// <remarks>
    /// This has the same effect as setting <see cref="IsSubMenuOpen"/> to true.
    /// </remarks>
    public void Open() => SetCurrentValue(IsSubMenuOpenProperty, true);

    /// <summary>
    /// Closes the submenu.
    /// </summary>
    /// <remarks>
    /// This has the same effect as setting <see cref="IsSubMenuOpen"/> to false.
    /// </remarks>
    public void Close() => SetCurrentValue(IsSubMenuOpenProperty, false);

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

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        TryUpdateCanExecute();
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

    /// <inheritdoc/>
    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        e.Handled = UpdateSelectionFromEventSource(e.Source, true);
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Don't handle here: let event bubble up to menu.
    }

    /// <inheritdoc/>
    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        RaiseEvent(new RoutedEventArgs(PointerEnteredItemEvent));
    }

    /// <inheritdoc/>
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
            foreach (var child in ((INavMenuItem)this).SubItems)
            {
                if (child != menuItem && child.IsSubMenuOpen)
                {
                    child.IsSubMenuOpen = false;
                }
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_popup != null)
        {
            _popup.Opened             -= PopupOpened;
            _popup.Closed             -= PopupClosed;
            _popup.DependencyResolver =  null;
        }

        _popup = e.NameScope.Find<Popup>("PART_Popup");

        if (_popup != null)
        {
            _popup.Opened += PopupOpened;
            _popup.Closed += PopupClosed;
        }

        _horizontalFrame = e.NameScope.Find<Border>(TopLevelHorizontalNavMenuItemTheme.FramePart);
        TokenResourceBinder.CreateTokenBinding(this, PopupMinWidthProperty, NavMenuTokenResourceKey.MenuPopupMinWidth);
        SetupItemIcon();
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
        if (change.Property == ParentProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == IconProperty)
        {
            IconChanged(change);
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
        }
        else if (change.Property == CommandParameterProperty)
        {
            CommandParameterChanged(change);
        }
        else if (change.Property == BoundsProperty)
        {
            SetupHorizontalEffectiveIndicatorWidth();
        }
        else if (change.Property == IconProperty)
        {
            SetupItemIcon();
        }

        if (change.Property == BoundsProperty ||
            change.Property == PopupMinWidthProperty)
        {
            SetupEffectivePopupMinWidth();
        }
    }

    private void SetupItemIcon()
    {
        if (Icon is not null && Icon is PathIcon menuItemIcon)
        {
            menuItemIcon.Name = ThemeConstants.ItemIconPart;
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
            PseudoClasses.Add(SeparatorPC);
            Focusable = false;
        }
        else if (Equals(oldValue, "-"))
        {
            PseudoClasses.Remove(SeparatorPC);
            Focusable = true;
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(TopLevelPC, IsTopLevel);
    }

    /// <summary>
    /// Called when the <see cref="Icon"/> property changes.
    /// </summary>
    /// <param name="e">The property change event.</param>
    private void IconChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var (oldValue, newValue) = e.GetOldAndNewValue<object?>();

        if (oldValue is ILogical oldLogical)
        {
            LogicalChildren.Remove(oldLogical);
            PseudoClasses.Remove(IconPC);
        }

        if (newValue is ILogical newLogical)
        {
            LogicalChildren.Add(newLogical);
            PseudoClasses.Add(IconPC);
        }
    }

    /// <summary>
    /// Called when the <see cref="IsSelected"/> property changes.
    /// </summary>
    /// <param name="e">The property change event.</param>
    private void IsSelectedChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var parentMenu = Parent as NavMenu;

        if ((bool)e.NewValue! && (parentMenu is null || parentMenu.IsOpen))
        {
            Focus();
        }
    }

    /// <summary>
    /// Called when the <see cref="IsSubMenuOpen"/> property changes.
    /// </summary>
    /// <param name="e">The property change event.</param>
    private void SubMenuOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var value = (bool)e.NewValue!;

        if (value)
        {
            foreach (var item in ItemsView.OfType<NavMenuItem>())
            {
                item.TryUpdateCanExecute();
            }

            RaiseEvent(new RoutedEventArgs(SubmenuOpenedEvent));
            SetCurrentValue(IsSelectedProperty, true);
            PseudoClasses.Add(StdPseudoClass.Open);
        }
        else
        {
            CloseSubmenus();
            SelectedIndex = -1;
            PseudoClasses.Remove(StdPseudoClass.Open);
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
        SelectedItem = null;
    }

    void ICommandSource.CanExecuteChanged(object sender, EventArgs e) => CanExecuteChangedHandler(sender, e);

    void IClickableControl.RaiseClick()
    {
        if (IsEffectivelyEnabled)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }
}