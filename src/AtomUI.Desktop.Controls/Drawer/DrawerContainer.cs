using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class DrawerContainer : ContentControl
{
    #region 内部属性定义

    internal static readonly DirectProperty<DrawerContainer, DrawerPlacement> PlacementProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, DrawerPlacement>(nameof(Placement),
            o => o.Placement,
            (o, v) => o.Placement = v);

    internal static readonly DirectProperty<DrawerContainer, bool> IsShowCloseButtonProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, bool>(nameof(IsShowCloseButton),
            o => o.IsShowCloseButton,
            (o, v) => o.IsShowCloseButton = v);

    internal static readonly DirectProperty<DrawerContainer, bool> IsShowMaskProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, bool>(nameof(IsShowMask),
            o => o.IsShowMask,
            (o, v) => o.IsShowMask = v);

    internal static readonly DirectProperty<DrawerContainer, string> TitleProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, string>(nameof(Title),
            o => o.Title,
            (o, v) => o.Title = v);

    internal static readonly DirectProperty<DrawerContainer, object?> FooterProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, object?>(nameof(Footer),
            o => o.Footer,
            (o, v) => o.Footer = v);

    internal static readonly DirectProperty<DrawerContainer, IDataTemplate?> FooterTemplateProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, IDataTemplate?>(nameof(FooterTemplate),
            o => o.FooterTemplate,
            (o, v) => o.FooterTemplate = v);

    internal static readonly DirectProperty<DrawerContainer, object?> ExtraProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, object?>(nameof(Extra),
            o => o.Extra,
            (o, v) => o.Extra = v);

    internal static readonly DirectProperty<DrawerContainer, IDataTemplate?> ExtraTemplateProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, IDataTemplate?>(nameof(ExtraTemplate),
            o => o.ExtraTemplate,
            (o, v) => o.ExtraTemplate = v);

    internal static readonly DirectProperty<DrawerContainer, double> DialogSizeProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, double>(nameof(DialogSize),
            o => o.DialogSize,
            (o, v) => o.DialogSize = v);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DrawerContainer>();

    internal static readonly DirectProperty<DrawerContainer, bool> IsCloseOnMaskClickProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, bool>(nameof(IsCloseOnMaskClick),
            o => o.IsCloseOnMaskClick,
            (o, v) => o.IsCloseOnMaskClick = v);
    
    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<DrawerContainer>();
    
    internal static readonly DirectProperty<DrawerContainer, double> PushOffsetPercentProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainer, double>(nameof(PushOffsetPercent),
            o => o.PushOffsetPercent,
            (o, v) => o.PushOffsetPercent = v);
    
    private DrawerPlacement _placement = DrawerPlacement.Right;

    internal DrawerPlacement Placement
    {
        get => _placement;
        set => SetAndRaise(PlacementProperty, ref _placement, value);
    }

    private bool _isShowMask;

    internal bool IsShowMask
    {
        get => _isShowMask;
        set => SetAndRaise(IsShowMaskProperty, ref _isShowMask, value);
    }

    private bool _isShowCloseButton = true;

    internal bool IsShowCloseButton
    {
        get => _isShowCloseButton;
        set => SetAndRaise(IsShowCloseButtonProperty, ref _isShowCloseButton, value);
    }

    private string _title = string.Empty;

    internal string Title
    {
        get => _title;
        set => SetAndRaise(TitleProperty, ref _title, value);
    }

    private object? _footer;

    internal object? Footer
    {
        get => _footer;
        set => SetAndRaise(FooterProperty, ref _footer, value);
    }

    private IDataTemplate? _footerTemplate;

    internal IDataTemplate? FooterTemplate
    {
        get => _footerTemplate;
        set => SetAndRaise(FooterTemplateProperty, ref _footerTemplate, value);
    }

    private object? _extra;

    internal object? Extra
    {
        get => _extra;
        set => SetAndRaise(ExtraProperty, ref _extra, value);
    }

    private IDataTemplate? _extraTemplate;

    internal IDataTemplate? ExtraTemplate
    {
        get => _extraTemplate;
        set => SetAndRaise(ExtraTemplateProperty, ref _extraTemplate, value);
    }

    private double _dialogSize;

    internal double DialogSize
    {
        get => _dialogSize;
        set => SetAndRaise(DialogSizeProperty, ref _dialogSize, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    private bool _closeWhenClickOnMask;

    internal bool IsCloseOnMaskClick
    {
        get => _closeWhenClickOnMask;
        set => SetAndRaise(IsCloseOnMaskClickProperty, ref _closeWhenClickOnMask, value);
    }
    
    internal TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    private double _pushOffsetPercent;

    internal double PushOffsetPercent
    {
        get => _pushOffsetPercent;
        set => SetAndRaise(PushOffsetPercentProperty, ref _pushOffsetPercent, value);
    }

    #endregion

    internal WeakReference<Drawer>? Drawer { get; set; }
    
    private BaseMotionActor? _motionActor;
    private DrawerInfoContainer? _infoContainer;
    private ITransform? _originInfoContainerTransform;
    private WeakReference<Drawer>? _pushedChildDrawer;
    private bool _isChildDrawerPushUpdateQueued;
    private int _operationVersion;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PlacementProperty ||
            change.Property == DialogSizeProperty ||
            change.Property == PushOffsetPercentProperty)
        {
            QueueChildDrawerPushUpdate();
        }
    }

    internal void Open(ScopeAwareAdornerLayer layer, Control adornedTarget)
    {
        if (Drawer != null && Drawer.TryGetTarget(out var drawer))
        {
            var alreadyAttachedToLayer = ReferenceEquals(this.GetVisualParent(), layer) || layer.Children.Contains(this);
            var operationVersion = BeginOperation();
            ScopeAwareAdornerLayer.SetAdornedElement(this, adornedTarget);
            ClearValue(BackgroundProperty);
            EnsureLayerParent(layer, adornedTarget);
            ApplyTemplate();
            if (alreadyAttachedToLayer)
            {
                if (_motionActor != null)
                {
                    _motionActor.Opacity = 1.0;
                }
                UpdateChildDrawerPush();
                return;
            }
            Dispatcher.InvokeAsync(async () =>
            {
                // 让 layer 更新
                if (!IsCurrentOperation(operationVersion) || _motionActor is null)
                {
                    return;
                }

                if (!IsMotionEnabled)
                {
                    _motionActor.Opacity = 1.0;
                    NotifyOpened(operationVersion, drawer);
                    return;
                }

                _motionActor.Opacity = 0.0;

                LayoutHelper.MeasureChild(_motionActor, DesiredSize, new Thickness());

                var motion = BuildMotionByPlacement(Placement, MotionDuration, true);

                await motion.RunAsync(_motionActor);
                if (!IsCurrentOperation(operationVersion))
                {
                    return;
                }
                NotifyOpened(operationVersion, drawer);
            });

        }
    }

    internal void Close(ScopeAwareAdornerLayer layer)
    {
        if (Drawer != null && Drawer.TryGetTarget(out var drawer))
        {
            var operationVersion = BeginOperation();
            if (_motionActor is null)
            {
                RemoveFromLayer(layer);
                drawer.NotifyClosed();
                return;
            }

            if (!IsMotionEnabled)
            {
                RemoveFromLayer(layer);
                ClearValue(BackgroundProperty);
                NotifyClosed(operationVersion, drawer);
                return;
            }
            
            SetCurrentValue(BackgroundProperty, Brushes.Transparent);
            var duration = TimeSpan.Zero;
            if (Transitions is not null)
            {
                foreach (var transition in Transitions)
                {
                    if (transition is TransitionBase transitionBase)
                    {
                        if (duration.CompareTo(transitionBase.Duration) < 0)
                        {
                            duration = transitionBase.Duration;
                        }
                    }
                }
            }

            var motion = BuildMotionByPlacement(Placement, MotionDuration, false);
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.WhenAll(motion.RunAsync(_motionActor), Task.Delay(duration));
                if (!IsCurrentOperation(operationVersion))
                {
                    return;
                }
                _motionActor.Opacity = 0.0;
                RemoveFromLayer(layer);
                _motionActor.Opacity = 1.0;
                ClearValue(BackgroundProperty);
                NotifyClosed(operationVersion, drawer);
            });
        }
    }

    private AbstractMotion BuildMotionByPlacement(DrawerPlacement placement, TimeSpan duration, bool isOpen)
    {
        if (_motionActor == null)
        {
            throw new InvalidOperationException("Drawer motion actor is not available.");
        }

        if (isOpen)
        {
            if (placement == DrawerPlacement.Left)
            {
                return new MoveLeftInMotion(_motionActor.DesiredSize.Width, duration, new CubicEaseOut());
            }
            if (placement == DrawerPlacement.Right)
            {
                return new MoveRightInMotion(_motionActor.DesiredSize.Width, duration, new CubicEaseOut());
            }
            if (placement == DrawerPlacement.Top)
            {
                return new MoveUpInMotion(_motionActor.DesiredSize.Height, duration, new CubicEaseOut());
            }
            return new MoveDownInMotion(_motionActor.DesiredSize.Height, duration, new CubicEaseOut());
        }

        if (placement == DrawerPlacement.Left)
        {
            return new MoveLeftOutMotion(_motionActor.DesiredSize.Width, duration, new CubicEaseOut());
        }
        if (placement == DrawerPlacement.Right)
        {
            return new MoveRightOutMotion(_motionActor.DesiredSize.Width, duration, new CubicEaseOut());
        }
        if (placement == DrawerPlacement.Top)
        {
            return new MoveUpOutMotion(_motionActor.DesiredSize.Height, duration, new CubicEaseOut());
        }
        return new MoveDownOutMotion(_motionActor.DesiredSize.Height, duration, new CubicEaseOut());
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (IsShowMask && IsCloseOnMaskClick)
        {
            if (Drawer != null && Drawer.TryGetTarget(out var drawer))
            {
                drawer.IsOpen = false;
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _motionActor   = e.NameScope.Find<BaseMotionActor>("PART_InfoContainerMotionActor");
        
        if (_infoContainer != null)
        {
            _infoContainer.CloseRequested -= HandleCloseRequested;
        }

        _infoContainer = e.NameScope.Find<DrawerInfoContainer>("PART_InfoContainer");
        if (_infoContainer != null)
        {
            _infoContainer.ApplyTemplate();
            _infoContainer.CloseRequested += HandleCloseRequested;
            UpdateChildDrawerPush();
        }
    }

    internal void Release()
    {
        BeginOperation();
        RemoveFromLayer(null);
        if (_infoContainer != null)
        {
            _infoContainer.CloseRequested -= HandleCloseRequested;
            _infoContainer = null;
        }
        _motionActor = null;
        _originInfoContainerTransform = null;
        _pushedChildDrawer = null;
        _isChildDrawerPushUpdateQueued = false;
        ScopeAwareAdornerLayer.SetAdornedElement(this, null);
        ClearValue(DataContextProperty);
        ClearValue(ContentProperty);
        ClearValue(ContentTemplateProperty);
        ClearValue(FooterProperty);
        ClearValue(FooterTemplateProperty);
        ClearValue(ExtraProperty);
        ClearValue(ExtraTemplateProperty);
    }

    private int BeginOperation()
    {
        unchecked
        {
            _operationVersion++;
        }
        return _operationVersion;
    }

    private bool IsCurrentOperation(int operationVersion)
    {
        return operationVersion == _operationVersion;
    }

    private void NotifyOpened(int operationVersion, Drawer drawer)
    {
        if (!IsCurrentOperation(operationVersion))
        {
            return;
        }
        drawer.NotifyOpened();
    }

    private void NotifyClosed(int operationVersion, Drawer drawer)
    {
        if (!IsCurrentOperation(operationVersion))
        {
            return;
        }
        drawer.NotifyClosed();
    }

    private void EnsureLayerParent(ScopeAwareAdornerLayer layer, Control logicalParent)
    {
        if (this.GetVisualParent() is Panel parent && !ReferenceEquals(parent, layer))
        {
            parent.Children.Remove(this);
            this.SetLogicalParent(null);
        }
        if (!layer.Children.Contains(this))
        {
            this.SetLogicalParent(logicalParent);
            layer.Children.Add(this);
        }
    }

    private void RemoveFromLayer(ScopeAwareAdornerLayer? layer)
    {
        if (layer != null && layer.Children.Contains(this))
        {
            layer.Children.Remove(this);
            this.SetLogicalParent(null);
            return;
        }
        if (this.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(this);
            this.SetLogicalParent(null);
        }
    }

    private void HandleCloseRequested(object? sender, EventArgs e)
    {
        if (Drawer != null && Drawer.TryGetTarget(out var drawer))
        {
            drawer.IsOpen = false;
        }
    }

    internal void NotifyChildDrawerAboutToOpen(Drawer childDrawer)
    {
        var pushedChildDrawer = GetPushedChildDrawer();
        if (pushedChildDrawer != null && !ReferenceEquals(pushedChildDrawer, childDrawer))
        {
            RestoreChildDrawerPush(true);
        }

        if (!ReferenceEquals(pushedChildDrawer, childDrawer))
        {
            _originInfoContainerTransform = _infoContainer?.RenderTransform;
            _pushedChildDrawer            = new WeakReference<Drawer>(childDrawer);
        }

        UpdateChildDrawerPush();
    }

    internal void NotifyChildDrawerAboutToClose(Drawer childDrawer)
    {
        if (IsPushedChildDrawer(childDrawer))
        {
            RestoreChildDrawerPush(true);
            _pushedChildDrawer = null;
        }
    }

    internal void NotifyChildDrawerPlacementChanged(Drawer childDrawer)
    {
        if (IsPushedChildDrawer(childDrawer))
        {
            QueueChildDrawerPushUpdate();
        }
    }

    private void QueueChildDrawerPushUpdate()
    {
        if (_pushedChildDrawer == null || _isChildDrawerPushUpdateQueued)
        {
            return;
        }

        _isChildDrawerPushUpdateQueued = true;
        Dispatcher.Post(UpdateQueuedChildDrawerPush);
    }

    private void UpdateQueuedChildDrawerPush()
    {
        _isChildDrawerPushUpdateQueued = false;
        UpdateChildDrawerPush();
    }

    private void UpdateChildDrawerPush()
    {
        if (_infoContainer == null)
        {
            return;
        }

        var childDrawer = GetPushedChildDrawer();
        if (childDrawer == null || !childDrawer.IsOpen)
        {
            RestoreChildDrawerPush(true);
            _pushedChildDrawer = null;
            return;
        }

        if (Placement != childDrawer.Placement)
        {
            RestoreChildDrawerPush(false);
            return;
        }

        var builder = new TransformOperations.Builder(1);
        var offsetX = 0d;
        var offsetY = 0d;

        var offset = DialogSize * PushOffsetPercent;
        if (Placement == DrawerPlacement.Left)
        {
            offsetX = offset;
        }
        else if (Placement == DrawerPlacement.Right)
        {
            offsetX = -offset;
        }
        else if (Placement == DrawerPlacement.Top)
        {
            offsetY = offset;
        }
        else
        {
            offsetY = -offset;
        }
        builder.AppendTranslate(offsetX, offsetY);
        _infoContainer.RenderTransform = builder.Build();
    }

    private void RestoreChildDrawerPush(bool clearOrigin)
    {
        if (_infoContainer != null)
        {
            _infoContainer.RenderTransform = _originInfoContainerTransform;
        }
        if (clearOrigin)
        {
            _originInfoContainerTransform = null;
        }
    }

    private Drawer? GetPushedChildDrawer()
    {
        if (_pushedChildDrawer != null && _pushedChildDrawer.TryGetTarget(out var childDrawer))
        {
            return childDrawer;
        }

        return null;
    }

    private bool IsPushedChildDrawer(Drawer childDrawer)
    {
        return ReferenceEquals(GetPushedChildDrawer(), childDrawer);
    }
}
