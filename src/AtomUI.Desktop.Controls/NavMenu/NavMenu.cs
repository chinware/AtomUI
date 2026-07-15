using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(NavMenuPseudoClass.InlineMode,
    NavMenuPseudoClass.InlineCollapsed,
    NavMenuPseudoClass.HorizontalMode,
    NavMenuPseudoClass.VerticalMode,
    NavMenuPseudoClass.DarkStyle,
    NavMenuPseudoClass.LightStyle)]
public class NavMenu : ItemsControl,
                       IFocusScope,
                       INavMenu,
                       IMotionAwareControl,
                       IMenuChildSelectable
{
    #region 公共属性定义
    public static readonly DirectProperty<NavMenu, INavMenuNode?> SelectedItemProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, INavMenuNode?>(
            nameof(SelectedItem),
            o => o.SelectedItem,
            (o, v) => o.SelectedItem = v,
            defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);
    
    public static readonly DirectProperty<NavMenu, TreeNodePath?> DefaultSelectedPathProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, TreeNodePath?>(
            nameof(DefaultSelectedPath),
            o => o.DefaultSelectedPath,
            (o, v) => o.DefaultSelectedPath = v);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<NavMenu>();
    
    public static readonly StyledProperty<bool> IsAccordionModeProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsAccordionMode), false);
    
    public static readonly StyledProperty<NavMenuMode> ModeProperty =
        AvaloniaProperty.Register<NavMenu, NavMenuMode>(nameof(Mode), NavMenuMode.Inline);

    public static readonly StyledProperty<bool> IsInlineCollapsedProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsInlineCollapsed));

    public static readonly StyledProperty<double> InlineCollapsedWidthProperty =
        AvaloniaProperty.Register<NavMenu, double>(nameof(InlineCollapsedWidth));
    
    public static readonly StyledProperty<bool> IsDarkStyleProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsDarkStyle), false);
    
    public static readonly StyledProperty<bool> IsItemBackgroundEnabledProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsItemBackgroundEnabled), true);

    public static readonly DirectProperty<NavMenu, IList<TreeNodePath>?> DefaultOpenPathsProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, IList<TreeNodePath>?>(
            nameof(DefaultOpenPaths),
            o => o.DefaultOpenPaths,
            (o, v) => o.DefaultOpenPaths = v);
    
    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty = 
        AvaloniaProperty.Register<NavMenu, bool>(nameof (ShouldUseOverlayPopup));
    
    private INavMenuNode? _selectedItem;
    private int _selectedItemRevision;

    public INavMenuNode? SelectedItem
    {
        get => _selectedItem;
        set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
    }
    
    private IList<TreeNodePath>? _defaultOpenPaths;
    
    public IList<TreeNodePath>? DefaultOpenPaths
    {
        get => _defaultOpenPaths;
        set => SetAndRaise(DefaultOpenPathsProperty, ref _defaultOpenPaths, value);
    }
    
    private TreeNodePath? _defaultSelectedPath;
    
    public TreeNodePath? DefaultSelectedPath
    {
        get => _defaultSelectedPath;
        set => SetAndRaise(DefaultSelectedPathProperty, ref _defaultSelectedPath, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsAccordionMode
    {
        get => GetValue(IsAccordionModeProperty);
        set => SetValue(IsAccordionModeProperty, value);
    }
    
    public NavMenuMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public bool IsInlineCollapsed
    {
        get => GetValue(IsInlineCollapsedProperty);
        set => SetValue(IsInlineCollapsedProperty, value);
    }

    public double InlineCollapsedWidth
    {
        get => GetValue(InlineCollapsedWidthProperty);
        set => SetValue(InlineCollapsedWidthProperty, value);
    }
    
    public bool IsDarkStyle
    {
        get => GetValue(IsDarkStyleProperty);
        set => SetValue(IsDarkStyleProperty, value);
    }
    
    public bool IsItemBackgroundEnabled
    {
        get => GetValue(IsItemBackgroundEnabledProperty);
        set => SetValue(IsItemBackgroundEnabledProperty, value);
    }

    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }
    #endregion
    
    #region 公共事件定义

    public static readonly RoutedEvent<NavMenuItemClickEventArgs> NavMenuItemClickEvent =
        RoutedEvent.Register<NavMenu, NavMenuItemClickEventArgs>(nameof(NavMenuItemClick), RoutingStrategies.Bubble);
    
    public static readonly RoutedEvent<NavMenuNodeSelectedEventArgs> NavMenuNodeSelectedEvent =
        RoutedEvent.Register<NavMenu, NavMenuNodeSelectedEventArgs>(nameof(NavMenuNodeSelected), RoutingStrategies.Bubble);

    public event EventHandler<NavMenuItemClickEventArgs>? NavMenuItemClick
    {
        add => AddHandler(NavMenuItemClickEvent, value);
        remove => RemoveHandler(NavMenuItemClickEvent, value);
    }
    
    public event EventHandler<NavMenuNodeSelectedEventArgs>? NavMenuNodeSelected
    {
        add => AddHandler(NavMenuNodeSelectedEvent, value);
        remove => RemoveHandler(NavMenuNodeSelectedEvent, value);
    }
    
    #endregion
    
    #region 内部属性定义

    IEnumerable<INavMenuItem> INavMenuElement.SubItems => EnumerateSubItems();

    internal static readonly DirectProperty<NavMenu, NavMenuMode> EffectiveModeProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, NavMenuMode>(
            nameof(EffectiveMode),
            o => o.EffectiveMode);

    internal static readonly DirectProperty<NavMenu, bool> IsEffectiveInlineCollapsedProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, bool>(
            nameof(IsEffectiveInlineCollapsed),
            o => o.IsEffectiveInlineCollapsed);

    internal static readonly StyledProperty<double> InlineCollapsedLayoutWidthProperty =
        AvaloniaProperty.Register<NavMenu, double>(
            nameof(InlineCollapsedLayoutWidth),
            double.NaN);

    private NavMenuMode _effectiveMode = NavMenuMode.Inline;

    internal NavMenuMode EffectiveMode
    {
        get => _effectiveMode;
        private set => SetAndRaise(EffectiveModeProperty, ref _effectiveMode, value);
    }

    private bool _isEffectiveInlineCollapsed;

    internal bool IsEffectiveInlineCollapsed
    {
        get => _isEffectiveInlineCollapsed;
        private set => SetAndRaise(IsEffectiveInlineCollapsedProperty, ref _isEffectiveInlineCollapsed, value);
    }

    internal double InlineCollapsedLayoutWidth
    {
        get => GetValue(InlineCollapsedLayoutWidthProperty);
        private set => SetValue(InlineCollapsedLayoutWidthProperty, value);
    }

    #endregion

    private IEnumerable<INavMenuItem> EnumerateSubItems()
    {
        foreach (var child in LogicalChildren)
        {
            if (child is INavMenuItem item)
            {
                yield return item;
            }
        }
    }
    
    internal INavMenuInteractionHandler? InteractionHandler { get; private set; }
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel { Orientation = Orientation.Vertical });

    private static readonly TimeSpan InlineCollapsedWidthMotionFrameInterval = TimeSpan.FromMilliseconds(16);

    private bool _defaultOpenPathsApplied;
    private int _motionContextLevel;
    private bool _originIsMotionEnabled;
    private List<IReadOnlyList<INavMenuNode>>? _inlineCollapsedOpenNodePathCache;
    private List<TreeNodePath>? _inlineCollapsedDefaultOpenPathCache;
    private CancellationTokenSource? _inlineCollapsedWidthMotionCancellationTokenSource;
    private readonly NavMenuSelectionCoordinator _selectionCoordinator = new();
    private double _lastInlineExpandedWidth = double.NaN;
    
    static NavMenu()
    {
        WidthProperty.OverrideMetadata<NavMenu>(
            new StyledPropertyMetadata<double>(coerce: CoerceInlineCollapsedWidth));
        MinWidthProperty.OverrideMetadata<NavMenu>(
            new StyledPropertyMetadata<double>(coerce: CoerceInlineCollapsedMinWidth));
        ItemsPanelProperty.OverrideDefaultValue(typeof(NavMenu), DefaultPanel);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
            typeof(NavMenu),
            KeyboardNavigationMode.Once);
        FocusableProperty.OverrideDefaultValue<NavMenu>(true);
        AutomationProperties.AccessibilityViewProperty.OverrideDefaultValue<NavMenu>(AccessibilityView.Control);
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<NavMenu>(AutomationControlType.Menu);
        NavMenuItem.SubmenuOpenedEvent.AddClassHandler<NavMenu>((navMenu, e) => navMenu.NotifySubmenuOpened(e));
        NavMenu.ModeProperty.Changed.AddClassHandler<NavMenu>((navMenu, e) => navMenu.HandleModeChanged());
        NavMenu.IsInlineCollapsedProperty.Changed.AddClassHandler<NavMenu>((navMenu, e) => navMenu.HandleInlineCollapsedChanged());
        NavMenu.InlineCollapsedWidthProperty.Changed.AddClassHandler<NavMenu>(
            (navMenu, e) => navMenu.HandleInlineCollapsedWidthChanged());
        NavMenu.InlineCollapsedLayoutWidthProperty.Changed.AddClassHandler<NavMenu>(
            (navMenu, e) => navMenu.CoerceInlineCollapsedLayoutConstraints());
    }
    
    public NavMenu()
    {
        UpdatePseudoClasses();
        Items.CollectionChanged += HandleItemsViewCollectionChanged;

        // 阻止弹出式子菜单内的元素把 BringIntoView 冒泡到外层 ScrollViewer，避免
        // 再次打开选中菜单项时触发页面滚动。Inline 模式下子菜单在视觉树内，
        // 让 NavMenu 自己的 ScrollViewer 正常响应即可。
        AddHandler(RequestBringIntoViewEvent, (_, e) =>
        {
            if (EffectiveMode == NavMenuMode.Inline || e.TargetObject == this)
            {
                return;
            }

            e.Handled = true;
        }, handledEventsToo: true);
    }
    
    private protected virtual void HandleItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!Items.IsReadOnly)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is not INavMenuNode)
                            {
                                throw new InvalidOperationException("The item does not implement the INavMenuNode interface.");
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsAccordionModeProperty)
        {
            if (change.GetNewValue<bool>())
            {
                for (var i = 0; i < LogicalChildren.Count; i++)
                {
                    if (LogicalChildren[i] is NavMenuItem child)
                    {
                        child.IsSubMenuOpen = false;
                    }
                }
            }
        }
        if (change.Property == IsDarkStyleProperty ||
            change.Property == ModeProperty ||
            change.Property == IsInlineCollapsedProperty)
        {
            UpdatePseudoClasses();
        }

        if (change.Property == SelectedItemProperty)
        {
            _selectedItemRevision++;
            if (SelectedItem != null)
            {
                if (!IsSelectedNodeAlreadyApplied(SelectedItem))
                {
                    SelectTargetMenuNode(SelectedItem, _selectedItemRevision);
                }
            }
            else
            {
                ClearSelectionState();
            }
        }
        else if (change.Property == IsMotionEnabledProperty &&
                 !change.GetNewValue<bool>())
        {
            CancelInlineCollapsedWidthMotion();
            ClearInlineCollapsedLayoutWidth();
        }
    }
    
    private void HandleModeChanged()
    {
        Close();
        UpdateEffectiveMode();
        ConfigureInteractionHandler(true);
    }

    private void HandleInlineCollapsedChanged()
    {
        if (Mode != NavMenuMode.Inline)
        {
            UpdateEffectiveMode();
            ConfigureInteractionHandler(true);
            return;
        }

        if (IsInlineCollapsed)
        {
            CollapseInlineMode();
        }
        else
        {
            ExpandInlineMode();
        }
    }

    private void UpdateEffectiveMode()
    {
        var isEffectiveInlineCollapsed = Mode == NavMenuMode.Inline && IsInlineCollapsed;
        IsEffectiveInlineCollapsed = isEffectiveInlineCollapsed;
        EffectiveMode              = isEffectiveInlineCollapsed ? NavMenuMode.Vertical : Mode;
        CoerceInlineCollapsedLayoutConstraints();
        UpdatePseudoClasses();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        CoerceInlineCollapsedLayoutConstraints();
        ConfigureInteractionHandler(true);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        CancelInlineCollapsedWidthMotion();
        ClearInlineCollapsedLayoutWidth();
        InteractionHandler?.Detach(this);
        InteractionHandler = null;
        _selectionCoordinator.Reset();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<NavMenuItem>(item, out recycleKey);
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new NavMenuItem();
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is NavMenuItem menuItem)
        {
            menuItem.OwnerMenu = this;
            var nodeBindingDisposables = menuItem.ResetNodeBindingDisposables();
            NavMenuItemContainerBinder.BindNode(menuItem, item, this, nodeBindingDisposables);

            if (!NavMenuItemContainerBinder.TryBindNodeHeaderTemplate(menuItem, item, nodeBindingDisposables) &&
                ItemTemplate != null)
            {
                nodeBindingDisposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, menuItem,
                    NavMenuItem.HeaderTemplateProperty));
            }
            
            menuItem[!NavMenuItem.ModeProperty]                  = this[!EffectiveModeProperty];
            menuItem[!NavMenuItem.IsInlineCollapsedProperty]     = this[!IsEffectiveInlineCollapsedProperty];
            menuItem[!NavMenuItem.IsDarkStyleProperty]           = this[!IsDarkStyleProperty];
            menuItem[!NavMenuItem.IsItemBackgroundEnabledProperty] = this[!IsItemBackgroundEnabledProperty];
            menuItem[!NavMenuItem.IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
            menuItem[!NavMenuItem.ShouldUseOverlayPopupProperty] = this[!ShouldUseOverlayPopupProperty];
           
            PrepareNavMenuItem(menuItem, item, index);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type NavMenuItem.");
        }
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        if (container is NavMenuItem menuItem)
        {
            menuItem.SetCurrentValue(NavMenuItem.IsKeyboardActiveProperty, false);
            _selectionCoordinator.Forget(menuItem);
            menuItem.ClearNodeBindingDisposables();
        }

        base.ClearContainerForItemOverride(container);
    }
    
    internal virtual void PrepareNavMenuItem(NavMenuItem menuItem, object? item, int index)
    {
    }

    internal void SelectNavMenuItem(NavMenuItem menuItem)
    {
        _selectionCoordinator.Select(this, menuItem);
    }

    internal void ClearSelectionState()
    {
        _selectionCoordinator.ClearSelection();
    }
    
    private void ConfigureInteractionHandler(bool needMount = false)
    {
        if (needMount)
        {
            InteractionHandler?.Detach(this);
        }
        
        if (EffectiveMode == NavMenuMode.Inline)
        {
            InteractionHandler = new InlineNavMenuInteractionHandler();
        }
        else
        {
            InteractionHandler = new DefaultNavMenuInteractionHandler();
        }
        
        if (needMount)
        {
            InteractionHandler?.Attach(this);
        }
    }

    private static double CoerceInlineCollapsedWidth(AvaloniaObject instance, double value)
    {
        if (instance is not NavMenu navMenu)
        {
            return value;
        }

        if (navMenu.TryGetInlineCollapsedLayoutWidth(out var layoutWidth))
        {
            return layoutWidth;
        }

        return navMenu.IsEffectiveInlineCollapsed
            ? NormalizeInlineCollapsedWidth(navMenu.InlineCollapsedWidth)
            : value;
    }

    private static double CoerceInlineCollapsedMinWidth(AvaloniaObject instance, double value)
    {
        if (instance is not NavMenu navMenu)
        {
            return value;
        }

        if (navMenu.TryGetInlineCollapsedLayoutWidth(out var layoutWidth))
        {
            return Math.Min(value, layoutWidth);
        }

        return navMenu.IsEffectiveInlineCollapsed
            ? Math.Min(value, NormalizeInlineCollapsedWidth(navMenu.InlineCollapsedWidth))
            : value;
    }

    private static double NormalizeInlineCollapsedWidth(double value)
    {
        return double.IsFinite(value) ? Math.Max(0, value) : 0;
    }

    private void CoerceInlineCollapsedLayoutConstraints()
    {
        CoerceValue(WidthProperty);
        CoerceValue(MinWidthProperty);
    }

    private bool TryGetInlineCollapsedLayoutWidth(out double layoutWidth)
    {
        layoutWidth = InlineCollapsedLayoutWidth;
        if (!double.IsFinite(layoutWidth))
        {
            return false;
        }

        layoutWidth = NormalizeInlineCollapsedWidth(layoutWidth);
        return true;
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(NavMenuPseudoClass.HorizontalMode, Mode == NavMenuMode.Horizontal);
        PseudoClasses.Set(NavMenuPseudoClass.VerticalMode, Mode == NavMenuMode.Vertical);
        PseudoClasses.Set(NavMenuPseudoClass.InlineMode, Mode == NavMenuMode.Inline);
        PseudoClasses.Set(NavMenuPseudoClass.InlineCollapsed, IsEffectiveInlineCollapsed);
        PseudoClasses.Set(NavMenuPseudoClass.DarkStyle, IsDarkStyle);
        PseudoClasses.Set(NavMenuPseudoClass.LightStyle, !IsDarkStyle);
    }
    
    protected virtual void NotifySubmenuOpened(RoutedEventArgs e)
    {
        if (IsAccordionMode)
        {
            if (e.Source is INavMenuItem menuItem && menuItem.Parent == this)
            {
                for (var i = 0; i < LogicalChildren.Count; i++)
                {
                    if (LogicalChildren[i] is INavMenuItem child &&
                        child != menuItem &&
                        child.IsSubMenuOpen)
                    {
                        child.IsSubMenuOpen = false;
                    }
                }
            }
        }
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureDefaultOpenedPaths();
        ConfigureDefaultSelectedPath();
    }

    private void CollapseInlineMode()
    {
        var shouldAnimateWidth = PrepareInlineCollapsedWidthMotion(true, out var motionStartWidth, out var motionTargetWidth);
        _inlineCollapsedOpenNodePathCache = CollectOpenInlineNodePaths();
        CloseOpenSubmenusPreservingSelection(this);
        UpdateEffectiveMode();
        ConfigureInteractionHandler(true);
        QueueApplySelectedStateToRealizedPath();
        StartPreparedInlineCollapsedWidthMotion(shouldAnimateWidth, motionStartWidth, motionTargetWidth);
    }

    private void ExpandInlineMode()
    {
        var shouldAnimateWidth = PrepareInlineCollapsedWidthMotion(false, out var motionStartWidth, out var motionTargetWidth);
        CloseOpenSubmenusPreservingSelection(this);
        UpdateEffectiveMode();
        ConfigureInteractionHandler(true);
        RestoreInlineCollapsedOpenPaths();
        QueueApplySelectedStateToRealizedPath();
        StartPreparedInlineCollapsedWidthMotion(shouldAnimateWidth, motionStartWidth, motionTargetWidth);
    }

    private void HandleInlineCollapsedWidthChanged()
    {
        if (_inlineCollapsedWidthMotionCancellationTokenSource is not null)
        {
            CancelInlineCollapsedWidthMotion();
            ClearInlineCollapsedLayoutWidth();
        }

        CoerceInlineCollapsedLayoutConstraints();
    }

    private bool PrepareInlineCollapsedWidthMotion(bool collapse,
                                                   out double startWidth,
                                                   out double targetWidth)
    {
        CancelInlineCollapsedWidthMotion();

        startWidth  = NormalizeInlineCollapsedWidth(Bounds.Width);
        targetWidth = collapse
            ? NormalizeInlineCollapsedWidth(InlineCollapsedWidth)
            : ResolveExpandedWidthForInlineCollapsedMotion(startWidth);

        if (collapse && double.IsFinite(startWidth))
        {
            _lastInlineExpandedWidth = startWidth;
        }

        if (!CanRunInlineCollapsedWidthMotion(startWidth, targetWidth))
        {
            ClearInlineCollapsedLayoutWidth();
            return false;
        }

        InlineCollapsedLayoutWidth = startWidth;
        CoerceInlineCollapsedLayoutConstraints();
        return true;
    }

    private bool CanRunInlineCollapsedWidthMotion(double startWidth, double targetWidth)
    {
        return IsLoaded &&
               IsMotionEnabled &&
               double.IsFinite(startWidth) &&
               double.IsFinite(targetWidth) &&
               !MathUtils.AreClose(startWidth, targetWidth);
    }

    private double ResolveExpandedWidthForInlineCollapsedMotion(double fallbackWidth)
    {
        var baseWidth = GetBaseValue(WidthProperty);
        if (baseWidth.HasValue &&
            baseWidth.Value is double width &&
            double.IsFinite(width))
        {
            return NormalizeInlineCollapsedWidth(width);
        }

        if (double.IsFinite(_lastInlineExpandedWidth))
        {
            return NormalizeInlineCollapsedWidth(_lastInlineExpandedWidth);
        }

        return fallbackWidth;
    }

    private void StartPreparedInlineCollapsedWidthMotion(bool shouldAnimate,
                                                         double startWidth,
                                                         double targetWidth)
    {
        if (!shouldAnimate)
        {
            return;
        }

        var duration = ResolveInlineCollapsedWidthMotionDuration();
        if (duration <= TimeSpan.Zero)
        {
            InlineCollapsedLayoutWidth = targetWidth;
            ClearInlineCollapsedLayoutWidth();
            return;
        }

        var cancellationTokenSource = new CancellationTokenSource();
        _inlineCollapsedWidthMotionCancellationTokenSource = cancellationTokenSource;
        _ = RunInlineCollapsedWidthMotionAsync(startWidth, targetWidth, duration, cancellationTokenSource);
    }

    private async Task RunInlineCollapsedWidthMotionAsync(double startWidth,
                                                          double targetWidth,
                                                          TimeSpan duration,
                                                          CancellationTokenSource cancellationTokenSource)
    {
        var easing = new CubicEaseOut();
        var startTimestamp = Stopwatch.GetTimestamp();
        try
        {
            while (true)
            {
                await Task.Delay(InlineCollapsedWidthMotionFrameInterval, cancellationTokenSource.Token);

                var progress = ResolveInlineCollapsedWidthMotionProgress(startTimestamp, duration);
                if (progress >= 1d)
                {
                    break;
                }

                ApplyInlineCollapsedWidthMotionFrame(
                    InterpolateInlineCollapsedWidth(startWidth, targetWidth, easing.Ease(progress)),
                    cancellationTokenSource);
            }
        }
        catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
        {
            // A newer inline collapsed transition owns the layout width now.
            return;
        }

        Dispatcher.UIThread.Post(() => CompleteInlineCollapsedWidthMotion(targetWidth, cancellationTokenSource));
    }

    private static double ResolveInlineCollapsedWidthMotionProgress(long startTimestamp, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            return 1d;
        }

        var elapsedSeconds = (Stopwatch.GetTimestamp() - startTimestamp) / (double)Stopwatch.Frequency;
        return Math.Clamp(elapsedSeconds / duration.TotalSeconds, 0d, 1d);
    }

    private static double InterpolateInlineCollapsedWidth(double startWidth, double targetWidth, double progress)
    {
        return startWidth + (targetWidth - startWidth) * progress;
    }

    private void ApplyInlineCollapsedWidthMotionFrame(double width,
                                                      CancellationTokenSource cancellationTokenSource)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (ReferenceEquals(_inlineCollapsedWidthMotionCancellationTokenSource, cancellationTokenSource))
            {
                InlineCollapsedLayoutWidth = width;
            }
        });
    }

    private void CompleteInlineCollapsedWidthMotion(double targetWidth,
                                                    CancellationTokenSource cancellationTokenSource)
    {
        if (!ReferenceEquals(_inlineCollapsedWidthMotionCancellationTokenSource, cancellationTokenSource))
        {
            return;
        }

        _inlineCollapsedWidthMotionCancellationTokenSource = null;
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        InlineCollapsedLayoutWidth = targetWidth;
        ClearInlineCollapsedLayoutWidth();
    }

    private TimeSpan ResolveInlineCollapsedWidthMotionDuration()
    {
        var application = Application.Current;
        var themeVariant = application?.ActualThemeVariant;
        if (application?.TryGetResource(SharedTokenKind.MotionDurationMid, themeVariant, out var value) == true &&
            value is TimeSpan duration)
        {
            return duration;
        }

        return TimeSpan.FromMilliseconds(200);
    }

    private void CancelInlineCollapsedWidthMotion()
    {
        var cancellationTokenSource = _inlineCollapsedWidthMotionCancellationTokenSource;
        if (cancellationTokenSource is null)
        {
            return;
        }

        _inlineCollapsedWidthMotionCancellationTokenSource = null;
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }

    private void ClearInlineCollapsedLayoutWidth()
    {
        if (double.IsFinite(InlineCollapsedLayoutWidth))
        {
            InlineCollapsedLayoutWidth = double.NaN;
        }
        CoerceInlineCollapsedLayoutConstraints();
    }

    private List<IReadOnlyList<INavMenuNode>>? CollectOpenInlineNodePaths()
    {
        var paths = new List<IReadOnlyList<INavMenuNode>>();
        CollectOpenInlineNodePaths(this, new List<INavMenuNode>(), paths);
        return paths.Count > 0 ? paths : null;
    }

    private static void CollectOpenInlineNodePaths(
        ItemsControl owner,
        List<INavMenuNode> currentPath,
        List<IReadOnlyList<INavMenuNode>> openPaths)
    {
        for (var i = 0; i < owner.ItemCount; i++)
        {
            if (owner.ContainerFromIndex(i) is not NavMenuItem item ||
                ((INavMenuItem)item).Node is not { } node)
            {
                continue;
            }

            currentPath.Add(node);
            if (item.HasSubMenu && item.IsSubMenuOpen)
            {
                openPaths.Add(currentPath.ToArray());
                CollectOpenInlineNodePaths(item, currentPath, openPaths);
            }

            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    private static void CloseOpenSubmenusPreservingSelection(ItemsControl owner)
    {
        for (var i = 0; i < owner.ItemCount; i++)
        {
            if (owner.ContainerFromIndex(i) is not NavMenuItem item)
            {
                continue;
            }

            CloseOpenSubmenusPreservingSelection(item);
            item.SetCurrentValue(NavMenuItem.IsSubMenuOpenProperty, false);
        }
    }

    private void RestoreInlineCollapsedOpenPaths()
    {
        var nodePaths = _inlineCollapsedOpenNodePathCache;
        _inlineCollapsedOpenNodePathCache = null;
        if (nodePaths != null)
        {
            foreach (var path in nodePaths)
            {
                RestoreInlineOpenNodePath(path);
            }
        }

        var defaultOpenPaths = _inlineCollapsedDefaultOpenPathCache;
        _inlineCollapsedDefaultOpenPathCache = null;
        if (defaultOpenPaths != null)
        {
            ReplayDefaultOpenPaths(defaultOpenPaths, GetMaxPathReplayPassCount(defaultOpenPaths));
        }
    }

    private void RestoreInlineOpenNodePath(IReadOnlyList<INavMenuNode> path)
    {
        ItemsControl current = this;
        for (var i = 0; i < path.Count; i++)
        {
            ExecutePendingContainerLayout(current);
            if (current.ContainerFromItem(path[i]) is not NavMenuItem item)
            {
                return;
            }

            if (item.HasSubMenu)
            {
                item.SetCurrentValue(NavMenuItem.IsSubMenuOpenProperty, true);
                ExecutePendingContainerLayout(item);
            }

            current = item;
        }
    }

    private void ApplySelectedStateToRealizedPath()
    {
        if (SelectedItem is null)
        {
            return;
        }

        var pathNodes = CollectPathNodes(SelectedItem);
        if (pathNodes.Count == 0)
        {
            return;
        }

        ItemsControl current = this;
        for (var i = 0; i < pathNodes.Count; i++)
        {
            if (current.ContainerFromItem(pathNodes[i]) is not NavMenuItem item)
            {
                return;
            }

            if (i == pathNodes.Count - 1)
            {
                item.SetCurrentValue(NavMenuItem.IsSelectedProperty, true);
                return;
            }

            item.SetCurrentValue(NavMenuItem.IsInSelectedPathProperty, true);
            current = item;
        }
    }

    private void QueueApplySelectedStateToRealizedPath()
    {
        ApplySelectedStateToRealizedPath();
        Dispatcher.InvokeAsync(ApplySelectedStateToRealizedPath, DispatcherPriority.Loaded);
    }
    
    private void ConfigureDefaultOpenedPaths()
    {
        if (DefaultOpenPaths != null && !_defaultOpenPathsApplied)
        {
            if (IsEffectiveInlineCollapsed)
            {
                _inlineCollapsedDefaultOpenPathCache = new List<TreeNodePath>(DefaultOpenPaths);
                _defaultOpenPathsApplied             = true;
                return;
            }

            Dispatcher.InvokeAsync(
                () => ReplayDefaultOpenPaths(DefaultOpenPaths, GetMaxPathReplayPassCount(DefaultOpenPaths)),
                DispatcherPriority.Loaded);
        }
    }
    
    private void ConfigureDefaultSelectedPath()
    {
        // 直接设置 SelectedItem 优先级高于 DefaultSelectedPath
        if (SelectedItem != null)
        {
            SelectTargetMenuNode(SelectedItem, _selectedItemRevision);
        }
        else if (DefaultSelectedPath != null)
        {
            Dispatcher.InvokeAsync(
                () => ReplayDefaultSelectedPath(DefaultSelectedPath, DefaultSelectedPath.Length + 1),
                DispatcherPriority.Loaded);
        }
    }

    private void SelectTargetMenuNode(INavMenuNode node, int selectedItemRevision)
    {
        var selectPathNodes = CollectPathNodes(node);
        if (selectPathNodes.Count > 0)
        {
            Dispatcher.InvokeAsync(
                () => ReplaySelectedNodePath(node, selectPathNodes, selectedItemRevision, selectPathNodes.Count + 1),
                DispatcherPriority.Loaded);
        }
    }

    private static int GetMaxPathReplayPassCount(IList<TreeNodePath> paths)
    {
        var maxLength = 0;
        foreach (var path in paths)
        {
            maxLength = Math.Max(maxLength, path.Length);
        }

        return maxLength + 1;
    }

    private void ReplayDefaultOpenPaths(IList<TreeNodePath> paths, int remainingPasses)
    {
        var allApplied = true;
        foreach (var path in paths)
        {
            if (TraverseNavMenuPath(path) is null)
            {
                allApplied = false;
            }
        }

        if (allApplied || remainingPasses <= 0)
        {
            _defaultOpenPathsApplied = true;
            return;
        }

        Dispatcher.InvokeAsync(
            () => ReplayDefaultOpenPaths(paths, remainingPasses - 1),
            DispatcherPriority.Loaded);
    }

    private void ReplayDefaultSelectedPath(TreeNodePath path, int remainingPasses)
    {
        var pathItems = TraverseNavMenuPath(path, (menuItem, i) =>
        {
            if (i == path.Length - 1)
            {
                InteractionHandler?.Select(menuItem);
            }
        });

        if (pathItems != null || remainingPasses <= 0)
        {
            return;
        }

        Dispatcher.InvokeAsync(() => ReplayDefaultSelectedPath(path, remainingPasses - 1),
            DispatcherPriority.Loaded);
    }

    private void ReplaySelectedNodePath(
        INavMenuNode node,
        IReadOnlyList<INavMenuNode> pathNodes,
        int selectedItemRevision,
        int remainingPasses)
    {
        if (selectedItemRevision != _selectedItemRevision ||
            !ReferenceEquals(SelectedItem, node))
        {
            return;
        }

        var pathItems = TraverseNavMenuPath(pathNodes, (menuItem, i) =>
        {
            if (i == pathNodes.Count - 1 &&
                selectedItemRevision == _selectedItemRevision &&
                ReferenceEquals(SelectedItem, node))
            {
                InteractionHandler?.Select(menuItem);
            }
        });

        if (pathItems != null || remainingPasses <= 0)
        {
            return;
        }

        Dispatcher.InvokeAsync(() => ReplaySelectedNodePath(node, pathNodes, selectedItemRevision,
            remainingPasses - 1), DispatcherPriority.Loaded);
    }

    private bool IsSelectedNodeAlreadyApplied(INavMenuNode node)
    {
        var pathNodes = CollectPathNodes(node);
        if (pathNodes.Count == 0)
        {
            return false;
        }

        ItemsControl current = this;
        for (var i = 0; i < pathNodes.Count; i++)
        {
            var menuItem = current.ContainerFromItem(pathNodes[i]) as NavMenuItem;
            if (menuItem is null)
            {
                return false;
            }

            var isLeaf = i == pathNodes.Count - 1;
            if (isLeaf)
            {
                return menuItem.IsSelected;
            }

            if (!menuItem.IsInSelectedPath)
            {
                return false;
            }

            current = menuItem;
        }

        return false;
    }

    public void Close()
    {
        var children = LogicalChildren;
        for (var i = 0; i < children.Count; i++)
        {
            if (children[i] is INavMenuItem menuItem)
            {
                menuItem.Close();
            }
        }
        
        SelectedItem = null;
    }
    
    internal void RaiseNavMenuItemClick(INavMenuItem menuItem)
    {
        RaiseEvent(new NavMenuItemClickEventArgs(NavMenuItemClickEvent, menuItem));
    }
    
    internal void RaiseNavMenuItemSelected(NavMenuItem menuItem)
    {
        var node = (menuItem as INavMenuItem).Node;
        Debug.Assert(node != null);
        RaiseEvent(new NavMenuNodeSelectedEventArgs(NavMenuNodeSelectedEvent, node));
        SetCurrentValue(SelectedItemProperty, node);
    }

    internal static List<NavMenuItem> CollectSelectPathItems(NavMenuItem menuItem)
    {
        var itemCount = CountSelectPathItems(menuItem);
        var items     = new List<NavMenuItem>(itemCount);
        for (var i = 0; i < itemCount; i++)
        {
            items.Add(null!);
        }

        var current = menuItem.GetLogicalParent<NavMenuItem>();
        for (var i = itemCount - 1; current != null; i--)
        {
            items[i] = current;
            current  = current.GetLogicalParent<NavMenuItem>();
        }
        return items;
    }

    internal static HashSet<NavMenuItem> BuildSelectPathSet(IReadOnlyCollection<NavMenuItem> items)
    {
        var itemSet = new HashSet<NavMenuItem>(items.Count);
        foreach (var item in items)
        {
            itemSet.Add(item);
        }

        return itemSet;
    }

    private static int CountSelectPathItems(NavMenuItem menuItem)
    {
        var          count   = 0;
        NavMenuItem? current = menuItem.GetLogicalParent<NavMenuItem>();
        while (current != null)
        {
            count++;
            current = current.GetLogicalParent<NavMenuItem>();
        }

        return count;
    }
    
    void IMenuChildSelectable.SelectChildItem(NavMenuItem child, bool isSelected)
    {
        if (child.IsTopLevel)
        {
            child.SetCurrentValue(NavMenuItem.IsSelectedProperty, isSelected);
        }
    }
    
    private List<NavMenuItem>? TraverseNavMenuPath(TreeNodePath treeNodePath, Action<NavMenuItem, int>? action = null)
    {
        if (treeNodePath.Length == 0)
        {
            return null;
        }

        return TraverseNavMenuPath(
            treeNodePath.Segments,
            (_, menuItem, segment) => menuItem.ItemKey != null && menuItem.ItemKey.Value == segment,
            action);
    }
    
    private List<NavMenuItem>? TraverseNavMenuPath(IReadOnlyList<INavMenuNode> pathNodes, Action<NavMenuItem, int>? action = null)
    {
        if (pathNodes.Count == 0)
        {
            return null;
        }

        return TraverseNavMenuPath(
            pathNodes,
            (node, _, currentNode) => ReferenceEquals(node, currentNode),
            action);
    }

    private List<NavMenuItem>? TraverseNavMenuPath<TSegment>(
        IReadOnlyList<TSegment> segments,
        Func<INavMenuNode, NavMenuItem, TSegment, bool> isTargetSegment,
        Action<NavMenuItem, int>? action = null)
    {
        try
        {
            EnterDisableMotionRegion();
            IList        items        = Items;
            var          pathItems    = new List<NavMenuItem>(segments.Count);
            NavMenuItem? previousItem = null;
            for (int i = 0; i < segments.Count; i++)
            {
                var  segment    = segments[i];
                bool childFound = false;
                for (var j = 0; j < items.Count; j++)
                {
                    if (items[j] is INavMenuNode node)
                    {
                        var navMenuItem = previousItem != null
                            ? GetNavMenuItemContainer(node, previousItem)
                            : GetNavMenuItemContainer(node, this);
                        if (navMenuItem == null)
                        {
                            return null;
                        }

                        if (isTargetSegment(node, navMenuItem, segment))
                        {
                            var requiresChildContainer = i < segments.Count - 1;
                            if (requiresChildContainer || HasNodeChildren(node))
                            {
                                if (!OpenPathSubmenu(navMenuItem, requiresChildContainer))
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                navMenuItem.SetCurrentValue(NavMenuItem.IsSubMenuOpenProperty, true);
                            }

                            items      = navMenuItem.Items;
                            childFound = true;
                            pathItems.Add(navMenuItem);
                            action?.Invoke(navMenuItem, i);
                            previousItem = navMenuItem;
                            break;
                        }
                    }
                }

                if (!childFound)
                {
                    return null;
                }
            }

            return pathItems;
        }
        finally
        {
            ExitDisableMotionRegion();
        }
    }

    private void EnterDisableMotionRegion()
    {
        if (_motionContextLevel == 0)
        {
            _originIsMotionEnabled = IsMotionEnabled;
            SetCurrentValue(IsMotionEnabledProperty, false);
        }

        ++_motionContextLevel;
    }

    private void ExitDisableMotionRegion()
    {
        --_motionContextLevel;
        if (_motionContextLevel == 0)
        {
            SetCurrentValue(IsMotionEnabledProperty, _originIsMotionEnabled);
        }
    }

    private List<INavMenuNode> CollectPathNodes(INavMenuNode node)
    {
        var pathCount = CountPathNodes(node);
        var pathNodes = new List<INavMenuNode>(pathCount);

        if (Items.Count > 0)
        {
            for (var i = 0; i < pathCount; i++)
            {
                pathNodes.Add(null!);
            }

            var current = node;
            for (var i = pathCount - 1; current != null; i--)
            {
                pathNodes[i] = current;
                current      = current.ParentNode as INavMenuNode;
            }
        
            Debug.Assert(pathNodes.Count > 0);
            // 检查是否是野数据
            var rootNode  = pathNodes[0];
            var foundRoot = false;
            foreach (var root in Items)
            {
                if (rootNode == root)
                {
                    foundRoot = true;
                    break;
                }
            }
            if (!foundRoot || node != pathNodes[^1])
            {
                throw new ArgumentOutOfRangeException(nameof(node), "Wild INavMenuNode, Only part of the path was found");
            }
        }
        
        return pathNodes;
    }

    private static int CountPathNodes(INavMenuNode node)
    {
        var count   = 0;
        var current = node;
        while (current != null)
        {
            count++;
            current = current.ParentNode as INavMenuNode;
        }

        return count;
    }
    
    private NavMenuItem? GetNavMenuItemContainer(INavMenuNode childNode, ItemsControl current)
    {
        var target = current.ContainerFromItem(childNode) as NavMenuItem;
        if (target != null)
        {
            return target;
        }

        ExecutePendingContainerLayout(current);
        return current.ContainerFromItem(childNode) as NavMenuItem;
    }

    internal void ExecutePendingContainerLayout(ItemsControl current)
    {
        current.ApplyTemplate();

        if (current.Presenter is { Panel: null } presenter)
        {
            presenter.ApplyTemplate();
            if (current.Presenter?.Panel != null)
            {
                return;
            }
        }

        if (current is NavMenuItem { Popup.Child: ILogical popupContent })
        {
            foreach (var descendant in popupContent.GetSelfAndLogicalDescendants())
            {
                if (descendant is ItemsPresenter popupPresenter &&
                    ReferenceEquals(popupPresenter.TemplatedParent, current))
                {
                    popupPresenter.ApplyTemplate();
                    if (current.Presenter?.Panel != null)
                    {
                        return;
                    }
                }
            }
        }

        if (current is NavMenuItem { Popup: { IsOpen: true, Child: { } popupChild } })
        {
            popupChild.UpdateLayout();
            if (current.Presenter?.Panel != null)
            {
                return;
            }
        }

        var topLevel = TopLevel.GetTopLevel(current);
        topLevel?.GetLayoutManager()?.ExecuteLayoutPass();
    }

    private static bool HasNodeChildren(INavMenuNode node)
    {
        foreach (var _ in node.Children)
        {
            return true;
        }

        return false;
    }

    private bool OpenPathSubmenu(NavMenuItem menuItem, bool requiresChildContainer)
    {
        if (menuItem.Mode != NavMenuMode.Inline &&
            !CanOpenPopup(menuItem))
        {
            return false;
        }

        var wasOpen = menuItem.IsSubMenuOpen;
        menuItem.SetCurrentValue(NavMenuItem.IsSubMenuOpenProperty, true);
        ExecutePendingContainerLayout(menuItem);
        if (menuItem.Mode == NavMenuMode.Inline &&
            !wasOpen)
        {
            return false;
        }

        return !requiresChildContainer || menuItem.Presenter?.Panel != null;
    }

    private static bool CanOpenPopup(NavMenuItem menuItem)
    {
        if (!menuItem.ShouldUseOverlayPopup &&
            RuntimePlatform.Features.SupportsNativeWindow)
        {
            return true;
        }

        return menuItem.GetPopupOverlayLayer() is not null;
    }
}
