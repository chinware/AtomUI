using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Icons.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using ControlList = Avalonia.Controls.Controls;

public enum FloatButtonGroupMenuPlacement
{
    Top,
    Bottom,
    Left,
    Right
}

public enum FloatButtonGroupTrigger
{
    Default,
    Click,
    Hover
}

public class FloatButtonGroup : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<FloatButtonPlacement> PlacementProperty =
        AbstractFloatButton.PlacementProperty.AddOwner<FloatButtonGroup>();
    
    public static readonly StyledProperty<double> FloatOffsetXProperty =
        AbstractFloatButton.FloatOffsetXProperty.AddOwner<FloatButtonGroup>();
    
    public static readonly StyledProperty<double> FloatOffsetYProperty =
        AbstractFloatButton.FloatOffsetYProperty.AddOwner<FloatButtonGroup>();
    
    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<FloatButtonGroup, PathIcon?>(nameof(Icon));
    
    public static readonly StyledProperty<PathIcon?> CloseIconProperty =
        AvaloniaProperty.Register<FloatButtonGroup, PathIcon?>(nameof(CloseIcon));
    
    public static readonly StyledProperty<FloatButtonType> ButtonTypeProperty =
        AbstractFloatButton.ButtonTypeProperty.AddOwner<FloatButtonGroup>();

    public static readonly StyledProperty<FloatButtonShape> ShapeProperty =
        AbstractFloatButton.ShapeProperty.AddOwner<FloatButtonGroup>();
    
    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        Border.BoxShadowProperty.AddOwner<FloatButtonGroup>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<FloatButtonGroup>();
    
    public static readonly StyledProperty<FloatButtonGroupMenuPlacement> MenuPlacementProperty =
        AvaloniaProperty.Register<FloatButtonGroup, FloatButtonGroupMenuPlacement>(nameof(MenuPlacement), FloatButtonGroupMenuPlacement.Top);
    
    public static readonly StyledProperty<FloatButtonGroupTrigger> TriggerProperty =
        AvaloniaProperty.Register<FloatButtonGroup, FloatButtonGroupTrigger>(nameof(Trigger), FloatButtonGroupTrigger.Default);
    
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<FloatButtonGroup, bool>(nameof(IsOpen));
    
    public static readonly StyledProperty<TimeSpan> MenuMotionDurationProperty =
        AvaloniaProperty.Register<FloatButtonGroup, TimeSpan>(nameof(MenuMotionDuration));
    
    public FloatButtonPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }
    
    public double FloatOffsetX
    {
        get => GetValue(FloatOffsetXProperty);
        set => SetValue(FloatOffsetXProperty, value);
    }

    public double FloatOffsetY
    {
        get => GetValue(FloatOffsetYProperty);
        set => SetValue(FloatOffsetYProperty, value);
    }
    
    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public PathIcon? CloseIcon
    {
        get => GetValue(CloseIconProperty);
        set => SetValue(CloseIconProperty, value);
    }
    
    public FloatButtonType ButtonType
    {
        get => GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }
    
    public FloatButtonShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }
    
    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public FloatButtonGroupMenuPlacement MenuPlacement
    {
        get => GetValue(MenuPlacementProperty);
        set => SetValue(MenuPlacementProperty, value);
    }
    
    public FloatButtonGroupTrigger Trigger
    {
        get => GetValue(TriggerProperty);
        set => SetValue(TriggerProperty, value);
    }
    
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }
    
    public TimeSpan MenuMotionDuration
    {
        get => GetValue(MenuMotionDurationProperty);
        set => SetValue(MenuMotionDurationProperty, value);
    }

    [Content] 
    public ControlList Children { get; } = new ();
    
    #endregion

    #region 公共事件定义
    public static readonly RoutedEvent<RoutedEventArgs> ClickedEvent =
        RoutedEvent.Register<FloatButtonGroup, RoutedEventArgs>(nameof(Clicked), RoutingStrategies.Bubble);
    
    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
        RoutedEvent.Register<FloatButtonGroup, RoutedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);
    
    public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent =
        RoutedEvent.Register<FloatButtonGroup, RoutedEventArgs>(nameof(Opened), RoutingStrategies.Bubble);
    
    public event EventHandler<RoutedEventArgs>? Clicked
    {
        add => AddHandler(ClickedEvent, value);
        remove => RemoveHandler(ClickedEvent, value);
    }
    
    public event EventHandler<RoutedEventArgs>? Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }
    
    public event EventHandler<RoutedEventArgs>? Opened
    {
        add => AddHandler(OpenedEvent, value);
        remove => RemoveHandler(OpenedEvent, value);
    }

    #endregion

    #region 内部属性定义

    internal event EventHandler? OpenRequest;
    internal event EventHandler? CloseRequest;

    #endregion
    
    private FloatButtonItemsControl? _itemsControl;
    private Canvas? _triggerLayout;
    private bool _initPressed;
    private IDisposable? _clickTriggerDisposable;
    private FloatButton? _triggerButton;
    ScopeAwareOverlayLayer? _overlayLayer;
    private BaseMotionActor? _motionActor;
    private CompositeDisposable? _menuDisposables;
    private readonly HashSet<AbstractFloatButton> _embeddedItems = new();
    private bool _showAnimating;
    private bool _hideAnimating;
    private bool _closeRequest;
    private bool _isAttachedToVisualTree;
    private bool _isMenuContentCodeCreated;
    
    static FloatButtonGroup()
    {
        AffectsMeasure<FloatButtonGroup>(IsOpenProperty);
    }

    public FloatButtonGroup()
    {
        this.RegisterTokenResourceScope(FloatButtonToken.ScopeProvider);
        Children.CollectionChanged += NotifyChildrenChanged;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Icon == null)
        {
            SetCurrentValue(IconProperty, new FileTextOutlined());
        }

        if (CloseIcon == null)
        {
            SetCurrentValue(CloseIconProperty, new CloseOutlined());
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TriggerProperty)
        {
            ConfigureTriggerType();
            if (Trigger == FloatButtonGroupTrigger.Default && _isMenuContentCodeCreated)
            {
                ReleaseMenuContent();
            }
            else if (IsOpen && Trigger != FloatButtonGroupTrigger.Default)
            {
                EnsureMenuContent();
            }
        }
        else if (change.Property == ParentProperty)
        {
            SetupParentLayer(Parent);
            if (_overlayLayer != null)
            {
                AbstractFloatButton.CalculatePosition(this, _overlayLayer.Bounds.Size, Placement, FloatOffsetX, FloatOffsetY);
            }
        }
        else if (change.Property == FloatOffsetXProperty ||
                 change.Property == FloatOffsetYProperty ||
                 change.Property == PlacementProperty)
        {
            if (_overlayLayer != null)
            {
                AbstractFloatButton.CalculatePosition(this, _overlayLayer.Bounds.Size, Placement, FloatOffsetX, FloatOffsetY);
            }
        }
        else if (change.Property == ShapeProperty ||
                 change.Property == IsMotionEnabledProperty)
        {
            SyncEmbeddedItems();
        }
        else if (change.Property == IsOpenProperty)
        {
            if (IsOpen && Trigger != FloatButtonGroupTrigger.Default)
            {
                EnsureMenuContent();
            }
            CalculateItemsControlPosition();
            Dispatcher.Post(this.ApplyOpenStateMotion);
        }
        if (change.Property == TriggerProperty ||
            change.Property == MenuPlacementProperty)
        {
            SyncItemsControlOrientation();
            CalculateItemsControlPosition();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DetachTriggerButtonHandlers();
        ReleaseMenuContent();
        _triggerLayout = e.NameScope.Find<Canvas>("PART_TriggerLayout");
        _itemsControl  = e.NameScope.Find<FloatButtonItemsControl>("ItemsControl");
        _triggerButton = e.NameScope.Find<FloatButton>("Trigger");
        _motionActor   = e.NameScope.Find<BaseMotionActor>(BaseMotionActor.MotionActorPart);
        _isMenuContentCodeCreated = false;

        if (_motionActor != null)
        {
            _motionActor.SetCurrentValue(IsVisibleProperty, IsOpen);
        }
        
        if (_triggerButton != null)
        {
            _triggerButton.PointerEntered += HandlePointerEntered;
            _triggerButton.PointerExited  += HandlePointerExited;
        }

        MaterializeItemsControlChildren();
        if (IsOpen && Trigger != FloatButtonGroupTrigger.Default)
        {
            EnsureMenuContent();
        }
        ConfigureTriggerType();
    }

    private void DetachTriggerButtonHandlers()
    {
        if (_triggerButton == null)
        {
            return;
        }

        _triggerButton.PointerEntered -= HandlePointerEntered;
        _triggerButton.PointerExited  -= HandlePointerExited;
        _triggerButton = null;
    }

    private void EnsureMenuContent()
    {
        if (Trigger == FloatButtonGroupTrigger.Default ||
            (_itemsControl != null && _motionActor != null) ||
            _triggerLayout == null)
        {
            return;
        }

        _menuDisposables?.Dispose();
        _menuDisposables = new CompositeDisposable();
        _itemsControl = new FloatButtonItemsControl
        {
            Name          = "ItemsControl",
            IsTriggerMode = true
        };
        _itemsControl.SetTemplatedParent(this);
        _menuDisposables.Add(BindUtils.RelayBind(this, BoxShadowProperty, _itemsControl,
            FloatButtonItemsControl.BoxShadowProperty, priority: BindingPriority.Template));
        _menuDisposables.Add(BindUtils.RelayBind(this, ShapeProperty, _itemsControl,
            FloatButtonItemsControl.ShapeProperty, priority: BindingPriority.Template));
        _menuDisposables.Add(BindUtils.RelayBind(this, MenuPlacementProperty, _itemsControl,
            FloatButtonItemsControl.MenuPlacementProperty, priority: BindingPriority.Template));
        SyncItemsControlOrientation();

        var shouldDelayShow = IsOpen && IsMotionEnabled;
        _motionActor = new MotionActor
        {
            Name         = BaseMotionActor.MotionActorPart,
            ClipToBounds = false,
            Content      = _itemsControl,
            IsVisible    = IsOpen && !shouldDelayShow,
            Opacity      = shouldDelayShow ? 0.0d : 1.0d
        };
        _motionActor.SetTemplatedParent(this);
        _triggerLayout.Children.Add(_motionActor);
        _isMenuContentCodeCreated = true;
        MaterializeItemsControlChildren();
        CalculateItemsControlPosition();
    }

    private void MaterializeItemsControlChildren()
    {
        if (_itemsControl == null)
        {
            return;
        }

        _itemsControl.IsTriggerMode = Trigger != FloatButtonGroupTrigger.Default;
        SyncItemsControlOrientation();
        if (_itemsControl.Children.Count > 0)
        {
            return;
        }

        foreach (var item in Children.OfType<AbstractFloatButton>())
        {
            NotifyAddItem(item);
        }
        _itemsControl.Children.AddRange(Children);
    }

    private void ReleaseMenuContent()
    {
        _menuDisposables?.Dispose();
        _menuDisposables = null;

        ReleaseItemsControlChildren();

        if (_isMenuContentCodeCreated)
        {
            if (_motionActor != null)
            {
                if (_motionActor.GetVisualParent() is Panel parent)
                {
                    parent.Children.Remove(_motionActor);
                }
                else
                {
                    _triggerLayout?.Children.Remove(_motionActor);
                }
                _motionActor.SetCurrentValue(ContentControl.ContentProperty, null);
                _motionActor.SetTemplatedParent(null);
            }
            _itemsControl?.SetTemplatedParent(null);
        }

        _itemsControl              = null;
        _motionActor               = null;
        _isMenuContentCodeCreated  = false;
        _showAnimating             = false;
        _hideAnimating             = false;
        _closeRequest              = false;
    }

    private void ReleaseItemsControlChildren()
    {
        if (_itemsControl != null)
        {
            var oldItems = _itemsControl.Children.OfType<Control>().ToList();
            if (oldItems.Count > 0)
            {
                _itemsControl.Children.RemoveAll(oldItems);
            }
        }

        foreach (var item in _embeddedItems.ToList())
        {
            NotifyRemoveItem(item);
        }
    }

    private void SyncItemsControlOrientation()
    {
        if (_itemsControl == null)
        {
            return;
        }

        var orientation = MenuPlacement == FloatButtonGroupMenuPlacement.Top ||
                          MenuPlacement == FloatButtonGroupMenuPlacement.Bottom
            ? Orientation.Vertical
            : Orientation.Horizontal;
        _itemsControl.SetValue(FloatButtonItemsControl.OrientationProperty, orientation, BindingPriority.Template);
    }

    protected virtual void NotifyChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var newItems = e.NewItems!.OfType<Control>().ToList();
                if (_itemsControl != null)
                {
                    foreach (var item in newItems.OfType<AbstractFloatButton>())
                    {
                        NotifyAddItem(item);
                    }
                    _itemsControl.Children.InsertRange(e.NewStartingIndex, newItems);
                }
                break;

            case NotifyCollectionChangedAction.Move:
                if (_itemsControl != null)
                {
                    _itemsControl.Children.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                var oldItems = e.OldItems!.OfType<Control>().ToList();
                foreach (var item in oldItems.OfType<AbstractFloatButton>())
                {
                    NotifyRemoveItem(item);
                }
                _itemsControl?.Children.RemoveAll(oldItems);
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var i = 0; i < e.OldItems!.Count; ++i)
                {
                    if (e.OldItems[i] is AbstractFloatButton oldFloatButton)
                    {
                        NotifyRemoveItem(oldFloatButton);
                    }
                    var child = (Control)e.NewItems![i]!;
                    if (child is AbstractFloatButton newFloatButton && _itemsControl != null)
                    {
                        NotifyAddItem(newFloatButton);
                    }
                    if (_itemsControl != null)
                    {
                        var index = i + e.OldStartingIndex;
                        _itemsControl.Children[index] = child;
                    }
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                ReleaseItemsControlChildren();
                break;
        }
    }

    protected void NotifyAddItem(AbstractFloatButton floatButton)
    {
        _embeddedItems.Add(floatButton);
        SyncEmbeddedItem(floatButton);
        floatButton.SetCurrentValue(AbstractFloatButton.IsEmbedModeProperty, true);
    }

    private void NotifyRemoveItem(AbstractFloatButton floatButton)
    {
        if (_embeddedItems.Remove(floatButton))
        {
            floatButton.ClearValue(AbstractFloatButton.ShapeProperty);
            floatButton.ClearValue(AbstractFloatButton.IsMotionEnabledProperty);
        }
        floatButton.SetCurrentValue(AbstractFloatButton.IsEmbedModeProperty, false);
    }

    private void SyncEmbeddedItems()
    {
        foreach (var item in _embeddedItems)
        {
            SyncEmbeddedItem(item);
        }
    }

    private void SyncEmbeddedItem(AbstractFloatButton floatButton)
    {
        floatButton.SetValue(AbstractFloatButton.ShapeProperty, Shape, BindingPriority.LocalValue);
        floatButton.SetValue(AbstractFloatButton.IsMotionEnabledProperty, IsMotionEnabled, BindingPriority.LocalValue);
    }

    private void HandlePointerEntered(object? sender, PointerEventArgs? e)
    {
        if (Trigger == FloatButtonGroupTrigger.Hover)
        {
            SetValue(IsOpenProperty, true, BindingPriority.Style);
            if (_overlayLayer != null)
            {
                OpenRequest?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    
    private void HandlePointerExited(object? sender, PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (Trigger == FloatButtonGroupTrigger.Hover)
        {
            SetValue(IsOpenProperty, false, BindingPriority.Style);
            if (_overlayLayer != null)
            {
                CloseRequest?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttachedToVisualTree = true;
        ConfigureTriggerType();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttachedToVisualTree = false;
        DisposeClickTrigger();
        ReleaseMenuContent();
        DetachTriggerButtonHandlers();
        SetupParentLayer(null);
        base.OnDetachedFromVisualTree(e);
    }

    private void ConfigureTriggerType()
    {
        DisposeClickTrigger();
        if (_isAttachedToVisualTree && Trigger == FloatButtonGroupTrigger.Click)
        {
            var inputManager = AvaloniaLocator.Current.GetService(typeof(IInputManager)) as IInputManager;
            _clickTriggerDisposable = inputManager?.Process.Subscribe(HandleMouseClick);
        }

        if (_itemsControl != null)
        {
            _itemsControl.IsTriggerMode = Trigger != FloatButtonGroupTrigger.Default;
        }
    }

    private void DisposeClickTrigger()
    {
        _clickTriggerDisposable?.Dispose();
        _clickTriggerDisposable = null;
    }

    private void HandleMouseClick(RawInputEventArgs args)
    {
        if (args is RawPointerEventArgs pointerEventArgs)
        {
            if (pointerEventArgs.Type == RawPointerEventType.LeftButtonDown)
            {
                if (pointerEventArgs.IsPointLogicalIn(this))
                {
                    _initPressed = true;
                }
            }
            else if (pointerEventArgs.Type == RawPointerEventType.LeftButtonUp)
            {
                if (pointerEventArgs.IsPointLogicalIn(this) || pointerEventArgs.IsPointLogicalIn(_itemsControl))
                {
                    if (_initPressed && pointerEventArgs.IsPointLogicalIn(_triggerButton))
                    {
                        SetValue(IsOpenProperty, !IsOpen, BindingPriority.Style);
                        if (_overlayLayer != null)
                        {
                            if (!IsOpen)
                            {
                                OpenRequest?.Invoke(this, EventArgs.Empty);
                            }
                            else
                            {
                                CloseRequest?.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }
                }
                else
                {
                    SetValue(IsOpenProperty, false, BindingPriority.Style);
                    if (_overlayLayer != null)
                    {
                        CloseRequest?.Invoke(this, EventArgs.Empty);
                    }
                }
                _initPressed = false;
            }
        }
    }
    
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        if (Shape == FloatButtonShape.Circle)
        {
            SetCurrentValue(CornerRadiusProperty, new CornerRadius(e.NewSize.Height / 2));
        }

        if (_overlayLayer != null)
        {
            AbstractFloatButton.CalculatePosition(this, _overlayLayer.Bounds.Size, Placement, FloatOffsetX, FloatOffsetY);
        }
    }
    
    private void SetupParentLayer(StyledElement? parent)
    {
        if (_overlayLayer != null)
        {
            _overlayLayer.SizeChanged -= HandleLayerSizeChanged;
        }
        if (parent is ScopeAwareOverlayLayer scopeAwareOverlayLayer)
        {
            _overlayLayer                      =  scopeAwareOverlayLayer;
            scopeAwareOverlayLayer.SizeChanged += HandleLayerSizeChanged;
        }
        else
        {
            _overlayLayer = null;
        }
    }

    private void HandleLayerSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        AbstractFloatButton.CalculatePosition(this, e.NewSize, Placement, FloatOffsetX, FloatOffsetY);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Trigger == FloatButtonGroupTrigger.Default)
        {
            return base.MeasureOverride(availableSize);
        }
        var size = base.MeasureOverride(availableSize);
        return _triggerButton?.DesiredSize ?? size; 
    }

    private void CalculateItemsControlPosition()
    {
        if (Trigger == FloatButtonGroupTrigger.Default)
        {
            return;
        }
        if (_motionActor == null || _triggerButton == null)
        {
            return;
        }

        var  offsetX         = 0.0d;
        var  offsetY         = 0.0d;
        var  width           = DesiredSize.Width;
        var  height          = DesiredSize.Height;
        var  originOpacity   = _motionActor.Opacity;
        var  originVisible   = _motionActor.IsVisible;
        Size motionActorSize = default;
        try
        {
            _motionActor.SetCurrentValue(IsVisibleProperty, true);
            _motionActor.SetCurrentValue(OpacityProperty, 0.0d);
            LayoutHelper.MeasureChild(_motionActor, new Size(double.PositiveInfinity, double.PositiveInfinity),
                new Thickness(0));
            motionActorSize = _motionActor.DesiredSize;
        }
        finally
        {
            _motionActor.SetCurrentValue(IsVisibleProperty, originVisible);
            _motionActor.SetCurrentValue(OpacityProperty, originOpacity);
        }
     
        if (MenuPlacement == FloatButtonGroupMenuPlacement.Top)
        {
            offsetY = -motionActorSize.Height;
        }
        else if (MenuPlacement == FloatButtonGroupMenuPlacement.Bottom)
        {
            offsetY = height;
        }
        else if (MenuPlacement == FloatButtonGroupMenuPlacement.Left)
        {
            offsetX = -motionActorSize.Width;
        }
        else if (MenuPlacement == FloatButtonGroupMenuPlacement.Right)
        {
            offsetX = width;
        }
        Canvas.SetLeft(_motionActor, offsetX);
        Canvas.SetTop(_motionActor, offsetY);
    }

    private void ApplyOpenStateMotion()
    {
        if (IsOpen)
        {
            Dispatcher.InvokeAsync(ApplyShowMotionAsync);
        }
        else
        {
            Dispatcher.InvokeAsync(ApplyHideMotionAsync);
        }
    }
    
    private async Task ApplyShowMotionAsync()
    {
        if (Trigger != FloatButtonGroupTrigger.Default)
        {
            EnsureMenuContent();
        }

        if (_motionActor is not null)
        {
            if (IsMotionEnabled)
            {
                if (_showAnimating)
                {
                    return;
                }
                _showAnimating = true;
                _motionActor.SetCurrentValue(IsVisibleProperty, false);
                AbstractMotion? motion = null;
                if (MenuPlacement == FloatButtonGroupMenuPlacement.Top)
                {
                    motion = new MoveDownInMotion(DesiredSize.Height, MenuMotionDuration, new CubicEaseOut());
                }
                else if (MenuPlacement == FloatButtonGroupMenuPlacement.Bottom)
                {
                    motion = new MoveUpInMotion(DesiredSize.Height, MenuMotionDuration, new CubicEaseOut());
                }
                else if (MenuPlacement == FloatButtonGroupMenuPlacement.Left)
                {
                    motion = new MoveRightInMotion(DesiredSize.Width, MenuMotionDuration, new CubicEaseOut());
                }
                else if (MenuPlacement == FloatButtonGroupMenuPlacement.Right)
                {
                    motion = new MoveLeftInMotion(DesiredSize.Width, MenuMotionDuration, new CubicEaseOut());
                }
                if (motion != null)
                {
                    await motion.RunAsync(_motionActor,
                        () => { _motionActor.SetCurrentValue(IsVisibleProperty, true); });
                }
                _showAnimating = false;
                if (_closeRequest)
                {
                    _closeRequest = false;
                    await ApplyHideMotionAsync();
                }
            }
            else
            {
                _motionActor.SetCurrentValue(IsVisibleProperty, true);
            }
        }
    }

    private async Task ApplyHideMotionAsync()
    {
        if (_motionActor is not null)
        {
            if (IsMotionEnabled)
            {
                if (_hideAnimating)
                {
                    return;
                }

                if (_showAnimating)
                {
                    _closeRequest = true;
                    return;
                }
                _hideAnimating = true;
                AbstractMotion? motion = null;
                if (MenuPlacement == FloatButtonGroupMenuPlacement.Top)
                {
                    motion =
                        new MoveDownOutMotion(DesiredSize.Height, MenuMotionDuration, new CubicEaseIn());
                }
                else if (MenuPlacement == FloatButtonGroupMenuPlacement.Bottom)
                {
                    motion =
                        new MoveUpOutMotion(DesiredSize.Height, MenuMotionDuration, new CubicEaseIn());
                }
                else if (MenuPlacement == FloatButtonGroupMenuPlacement.Left)
                {
                    motion =
                        new MoveRightOutMotion(DesiredSize.Width, MenuMotionDuration, new CubicEaseIn());
                }
                else if (MenuPlacement == FloatButtonGroupMenuPlacement.Right)
                {
                    motion =
                        new MoveLeftOutMotion(DesiredSize.Width, MenuMotionDuration, new CubicEaseIn());
                }
                if (motion != null)
                {
                    await motion.RunAsync(_motionActor);
                }
                _hideAnimating = false;
                _motionActor.SetCurrentValue(IsVisibleProperty, true);
            }
            else
            {
                _motionActor.SetCurrentValue(IsVisibleProperty, false);
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (IsOpen && Trigger != FloatButtonGroupTrigger.Default)
        {
            EnsureMenuContent();
        }
        CalculateItemsControlPosition();
        if (IsOpen)
        {
            Dispatcher.InvokeAsync(ApplyShowMotionAsync);
        }
        else if (_motionActor != null)
        {
            Dispatcher.InvokeAsync(ApplyHideMotionAsync);
        }
    }
}
