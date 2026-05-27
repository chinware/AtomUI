using System.Diagnostics;
using System.Reactive.Disposables;
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
using Avalonia.Threading;

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
        set
        {
            if (SetAndRaise(PlacementProperty, ref _placement, value))
            {
                RefreshActiveChildDrawerPushTransform();
            }
        }
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
        set
        {
            if (SetAndRaise(PushOffsetPercentProperty, ref _pushOffsetPercent, value))
            {
                RefreshActiveChildDrawerPushTransform();
            }
        }
    }

    #endregion

    internal WeakReference<Drawer>? Drawer { get; set; }
    
    private BaseMotionActor? _motionActor;
    private DrawerInfoContainer? _infoContainer;
    private ITransform? _originInfoContainerTransform;
    private WeakReference<Drawer>? _activeChildDrawer;
    private bool _isChildDrawerPushed;
    private bool _openAnimating;
    private bool _closeAnimating;
    private int _lifecycleVersion;
    private int _pushTransformVersion;
    private CompositeDisposable? _drawerBindingDisposables;

    internal void BindToDrawer(Drawer drawer)
    {
        _drawerBindingDisposables?.Dispose();
        _drawerBindingDisposables = new CompositeDisposable
        {
            Bind(DataContextProperty, drawer[!DataContextProperty]),
            Bind(ContentProperty, drawer[!AtomUI.Desktop.Controls.Drawer.ContentProperty]),
            Bind(ContentTemplateProperty, drawer[!AtomUI.Desktop.Controls.Drawer.ContentTemplateProperty]),
            Bind(FooterProperty, drawer[!AtomUI.Desktop.Controls.Drawer.FooterProperty]),
            Bind(FooterTemplateProperty, drawer[!AtomUI.Desktop.Controls.Drawer.FooterTemplateProperty]),
            Bind(ExtraProperty, drawer[!AtomUI.Desktop.Controls.Drawer.ExtraProperty]),
            Bind(ExtraTemplateProperty, drawer[!AtomUI.Desktop.Controls.Drawer.ExtraTemplateProperty]),
            Bind(DialogSizeProperty, drawer[!AtomUI.Desktop.Controls.Drawer.EffectiveDialogSizeProperty]),
            Bind(PlacementProperty, drawer[!AtomUI.Desktop.Controls.Drawer.PlacementProperty]),
            Bind(TitleProperty, drawer[!AtomUI.Desktop.Controls.Drawer.TitleProperty]),
            Bind(IsShowMaskProperty, drawer[!AtomUI.Desktop.Controls.Drawer.IsShowMaskProperty]),
            Bind(IsShowCloseButtonProperty, drawer[!AtomUI.Desktop.Controls.Drawer.IsShowCloseButtonProperty]),
            Bind(IsMotionEnabledProperty, drawer[!AtomUI.Desktop.Controls.Drawer.IsMotionEnabledProperty]),
            Bind(IsCloseOnMaskClickProperty, drawer[!AtomUI.Desktop.Controls.Drawer.IsCloseOnMaskClickProperty]),
            Bind(PushOffsetPercentProperty, drawer[!AtomUI.Desktop.Controls.Drawer.PushOffsetPercentProperty])
        };
    }

    internal void Open(ScopeAwareAdornerLayer layer)
    {
        if (Drawer != null && Drawer.TryGetTarget(out var drawer))
        {
            var lifecycleVersion = ++_lifecycleVersion;
            _closeAnimating = false;
            ScopeAwareAdornerLayer.SetAdornedElement(this, drawer.OpenOn);
            AttachToLayer(layer);
            ApplyTemplate();
            Dispatcher.InvokeAsync(async () =>
            {
                if (lifecycleVersion != _lifecycleVersion)
                {
                    return;
                }

                // 让 layer 更新
                if (_motionActor is null || _openAnimating)
                {
                    return;
                }

                if (!IsMotionEnabled)
                {
                    _motionActor.Opacity = 1.0;
                    drawer.NotifyOpened();
                    return;
                }

                _openAnimating       = true;
                _motionActor.Opacity = 0.0;

                LayoutHelper.MeasureChild(_motionActor, DesiredSize, new Thickness());

                var motion = BuildMotionByPlacement(Placement, MotionDuration, true);

                await motion.RunAsync(_motionActor);
                if (lifecycleVersion != _lifecycleVersion)
                {
                    return;
                }
                _openAnimating = false;
                drawer.NotifyOpened();
            });

        }
    }

    internal void Close(ScopeAwareAdornerLayer layer)
    {
        if (Drawer != null && Drawer.TryGetTarget(out var drawer))
        {
            if (_closeAnimating)
            {
                return;
            }

            var lifecycleVersion = ++_lifecycleVersion;
            _openAnimating = false;
            if (_motionActor is null || !IsMotionEnabled)
            {
                DetachFromLayer(layer);
                drawer.NotifyClosed();
                return;
            }
            
            _closeAnimating = true;
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
                if (lifecycleVersion != _lifecycleVersion)
                {
                    _closeAnimating = false;
                    return;
                }
                _closeAnimating      = false;
                _motionActor.Opacity = 0.0;
                DetachFromLayer(layer);
                _motionActor.Opacity = 1.0;
                drawer.NotifyClosed();
            });
        }
    }

    internal void Release()
    {
        ++_lifecycleVersion;
        ++_pushTransformVersion;
        _openAnimating  = false;
        _closeAnimating = false;
        DetachFromLayer(null);
        _drawerBindingDisposables?.Dispose();
        _drawerBindingDisposables = null;
        ReleaseTemplateContent();

        if (_infoContainer != null)
        {
            _infoContainer.CloseRequested -= HandleCloseRequested;
            _infoContainer = null;
        }

        _motionActor                  = null;
        _originInfoContainerTransform = null;
        _activeChildDrawer            = null;
        _isChildDrawerPushed          = false;
        Drawer                        = null;
    }

    private void ReleaseTemplateContent()
    {
        ClearValue(DataContextProperty);
        ClearValue(ContentProperty);
        ClearValue(ContentTemplateProperty);
        ClearValue(FooterProperty);
        ClearValue(FooterTemplateProperty);
        ClearValue(ExtraProperty);
        ClearValue(ExtraTemplateProperty);
        ClearValue(DialogSizeProperty);
        ClearValue(PlacementProperty);
        ClearValue(TitleProperty);
        ClearValue(IsShowMaskProperty);
        ClearValue(IsShowCloseButtonProperty);
        ClearValue(IsMotionEnabledProperty);
        ClearValue(IsCloseOnMaskClickProperty);
        ClearValue(PushOffsetPercentProperty);

        if (_infoContainer == null)
        {
            return;
        }

        _infoContainer.Content         = null;
        _infoContainer.ContentTemplate = null;
        _infoContainer.Footer          = null;
        _infoContainer.FooterTemplate  = null;
        _infoContainer.Extra           = null;
        _infoContainer.ExtraTemplate   = null;
    }

    private void AttachToLayer(ScopeAwareAdornerLayer layer)
    {
        if (this.GetVisualParent() is Panel currentParent)
        {
            if (ReferenceEquals(currentParent, layer))
            {
                return;
            }

            currentParent.Children.Remove(this);
        }

        if (!layer.Children.Contains(this))
        {
            layer.Children.Add(this);
        }
    }

    private void DetachFromLayer(ScopeAwareAdornerLayer? layer)
    {
        if (this.GetVisualParent() is Panel currentParent)
        {
            currentParent.Children.Remove(this);
        }
        else
        {
            layer?.Children.Remove(this);
        }

        ScopeAwareAdornerLayer.SetAdornedElement(this, null);
    }

    private AbstractMotion BuildMotionByPlacement(DrawerPlacement placement, TimeSpan duration, bool isOpen)
    {
        AbstractMotion? motion = null;
        Debug.Assert(_motionActor != null);
        if (isOpen)
        {
            if (placement == DrawerPlacement.Left)
            {
                motion = new MoveLeftInMotion(_motionActor.DesiredSize.Width, duration, new CubicEaseOut());
            }
            else if (placement == DrawerPlacement.Right)
            {
                motion = new MoveRightInMotion(_motionActor.DesiredSize.Width, duration, new CubicEaseOut());
            }
            else if (placement == DrawerPlacement.Top)
            {
                motion = new MoveUpInMotion(_motionActor.DesiredSize.Height, duration, new CubicEaseOut());
            }
            else
            {
                motion = new MoveDownInMotion(_motionActor.DesiredSize.Height, duration, new CubicEaseOut());
            }
        }
        else
        {
            if (placement == DrawerPlacement.Left)
            {
                motion = new MoveLeftOutMotion(_motionActor.DesiredSize.Width, duration, new CubicEaseOut());
            }
            else if (placement == DrawerPlacement.Right)
            {
                motion = new MoveRightOutMotion(_motionActor.DesiredSize.Width, duration, new CubicEaseOut());
            }
            else if (placement == DrawerPlacement.Top)
            {
                motion = new MoveUpOutMotion(_motionActor.DesiredSize.Height, duration, new CubicEaseOut());
            }
            else
            {
                motion = new MoveDownOutMotion(_motionActor.DesiredSize.Height, duration, new CubicEaseOut());
            }
        }

        Debug.Assert(motion != null);
        return motion;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (IsCloseOnMaskClick)
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
            _infoContainer.CloseRequested += HandleCloseRequested;
        }
        RefreshActiveChildDrawerPushTransform();
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
        _activeChildDrawer = new WeakReference<Drawer>(childDrawer);
        RefreshActiveChildDrawerPushTransform();
    }

    internal void NotifyChildDrawerAboutToClose(Drawer childDrawer)
    {
        if (IsActiveChildDrawer(childDrawer))
        {
            _activeChildDrawer = null;
            RestoreChildDrawerPushTransform();
        }
    }

    internal void NotifyChildDrawerPlacementChanged(Drawer childDrawer)
    {
        if (IsActiveChildDrawer(childDrawer))
        {
            RefreshActiveChildDrawerPushTransform();
        }
    }

    private bool IsActiveChildDrawer(Drawer childDrawer)
    {
        return _activeChildDrawer != null &&
               _activeChildDrawer.TryGetTarget(out var activeChildDrawer) &&
               ReferenceEquals(activeChildDrawer, childDrawer);
    }

    private void RefreshActiveChildDrawerPushTransform()
    {
        if (_activeChildDrawer == null || !_activeChildDrawer.TryGetTarget(out var childDrawer))
        {
            _activeChildDrawer = null;
            RestoreChildDrawerPushTransform();
            return;
        }

        if (_infoContainer == null)
        {
            return;
        }

        if (Placement != childDrawer.Placement)
        {
            // Parent and child Placement bindings can settle in separate notifications.
            // Keep the current push for this dispatcher turn so no zero-offset frame is shown.
            ScheduleChildDrawerPushRestore(childDrawer);
            return;
        }

        ++_pushTransformVersion;
        if (!_isChildDrawerPushed)
        {
            _originInfoContainerTransform = _infoContainer.RenderTransform;
            _isChildDrawerPushed          = true;
        }

        double offsetX = 0d;
        double offsetY = 0d;

        if (Placement == DrawerPlacement.Left)
        {
            offsetX = DesiredSize.Width * PushOffsetPercent;
        }
        else if (Placement == DrawerPlacement.Right)
        {
            offsetX = -DesiredSize.Width * PushOffsetPercent;
        }
        else if (Placement == DrawerPlacement.Top)
        {
            offsetY = DesiredSize.Height * PushOffsetPercent;
        }
        else
        {
            offsetY = -DesiredSize.Height * PushOffsetPercent;
        }

        var builder = new TransformOperations.Builder(1);
        builder.AppendTranslate(offsetX, offsetY);
        _infoContainer.RenderTransform = builder.Build();
    }

    private void ScheduleChildDrawerPushRestore(Drawer childDrawer)
    {
        var pushTransformVersion = ++_pushTransformVersion;
        Dispatcher.Post(() =>
        {
            if (pushTransformVersion != _pushTransformVersion ||
                _infoContainer == null ||
                !IsActiveChildDrawer(childDrawer) ||
                Placement == childDrawer.Placement)
            {
                return;
            }

            RestoreChildDrawerPushTransform();
        });
    }

    private void RestoreChildDrawerPushTransform()
    {
        ++_pushTransformVersion;
        if (_infoContainer != null && _isChildDrawerPushed)
        {
            _infoContainer.RenderTransform = _originInfoContainerTransform;
        }

        _originInfoContainerTransform = null;
        _isChildDrawerPushed          = false;
    }
}
