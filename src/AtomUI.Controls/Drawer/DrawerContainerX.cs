using System.Diagnostics;
using AtomUI.MotionScene;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class DrawerContainerX : ContentControl
{
    #region 内部属性定义

    internal static readonly DirectProperty<DrawerContainerX, DrawerPlacement> PlacementProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, DrawerPlacement>(nameof(Placement),
            o => o.Placement,
            (o, v) => o.Placement = v);

    internal static readonly DirectProperty<DrawerContainerX, bool> IsShowCloseButtonProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, bool>(nameof(IsShowCloseButton),
            o => o.IsShowCloseButton,
            (o, v) => o.IsShowCloseButton = v);

    internal static readonly DirectProperty<DrawerContainerX, bool> IsShowMaskProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, bool>(nameof(IsShowMask),
            o => o.IsShowMask,
            (o, v) => o.IsShowMask = v);

    internal static readonly DirectProperty<DrawerContainerX, string> TitleProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, string>(nameof(Title),
            o => o.Title,
            (o, v) => o.Title = v);

    internal static readonly DirectProperty<DrawerContainerX, object?> FooterProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, object?>(nameof(Footer),
            o => o.Footer,
            (o, v) => o.Footer = v);

    internal static readonly DirectProperty<DrawerContainerX, IDataTemplate?> FooterTemplateProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, IDataTemplate?>(nameof(FooterTemplate),
            o => o.FooterTemplate,
            (o, v) => o.FooterTemplate = v);

    internal static readonly DirectProperty<DrawerContainerX, object?> ExtraProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, object?>(nameof(Extra),
            o => o.Extra,
            (o, v) => o.Extra = v);

    internal static readonly DirectProperty<DrawerContainerX, IDataTemplate?> ExtraTemplateProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, IDataTemplate?>(nameof(ExtraTemplate),
            o => o.ExtraTemplate,
            (o, v) => o.ExtraTemplate = v);

    internal static readonly DirectProperty<DrawerContainerX, SizeType> SizeTypeProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, SizeType>(nameof(SizeType),
            o => o.SizeType,
            (o, v) => o.SizeType = v);

    internal static readonly DirectProperty<DrawerContainerX, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    internal static readonly DirectProperty<DrawerContainerX, bool> CloseWhenClickOnMaskProperty
        = AvaloniaProperty.RegisterDirect<DrawerContainerX, bool>(nameof(CloseWhenClickOnMask),
            o => o.CloseWhenClickOnMask,
            (o, v) => o.CloseWhenClickOnMask = v);

    internal static readonly DirectProperty<DrawerContainerX, TimeSpan> MotionDurationProperty =
        AvaloniaProperty.RegisterDirect<DrawerContainerX, TimeSpan>(nameof(MotionDuration),
            o => o.MotionDuration,
            (o, v) => o.MotionDuration = v);

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

    private SizeType _sizeType;

    internal SizeType SizeType
    {
        get => _sizeType;
        set => SetAndRaise(SizeTypeProperty, ref _sizeType, value);
    }

    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }

    private bool _closeWhenClickOnMask;

    internal bool CloseWhenClickOnMask
    {
        get => _closeWhenClickOnMask;
        set => SetAndRaise(CloseWhenClickOnMaskProperty, ref _closeWhenClickOnMask, value);
    }

    private TimeSpan _motionDuration;

    internal TimeSpan MotionDuration
    {
        get => _motionDuration;
        set => SetAndRaise(MotionDurationProperty, ref _motionDuration, value);
    }

    #endregion

    internal WeakReference<DrawerX>? Drawer { get; set; }

    private MotionActorControl? _motionActor;
    private DrawerInfoContainerX? _infoContainer;
    private bool _openAnimating;
    private bool _closeAnimating;

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, SharedTokenKey.MotionDurationSlow);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TokenResourceBinder.CreateTokenBinding(this, BackgroundProperty, SharedTokenKey.ColorBgMask);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        TokenResourceBinder.CreateTokenBinding(this, BackgroundProperty, SharedTokenKey.ColorTransparent);
    }

    internal void Open(DrawerLayer layer)
    {
        if (Drawer != null && Drawer.TryGetTarget(out var drawer))
        {
            DrawerLayer.SetAttachTargetElement(this, drawer.OpenOn);
            DrawerLayer.SetIsClipEnabled(this, true);
            layer.Children.Add(this);
            Dispatcher.UIThread.Post(() =>
            {
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

                _openAnimating         = true;
                _motionActor.IsVisible = true;
                
                LayoutHelper.MeasureChild(_motionActor, DesiredSize, new Thickness());
                
                var motion = BuildMotionByPlacement(Placement, MotionDuration, true);
                MotionInvoker.Invoke(_motionActor, motion, null,
                    () =>
                    {
                        _openAnimating = false;
                        drawer.NotifyOpened();
                    });
            });
           
        }
    }

    internal void Close(DrawerLayer layer)
    {
        if (Drawer != null && Drawer.TryGetTarget(out var drawer))
        {
            if (_motionActor is null || _closeAnimating)
            {
                return;
            }
        
            if (!IsMotionEnabled)
            {
                layer.Children.Remove(this);
                drawer.NotifyClosed();
                return;
            }
        
            _closeAnimating = true;
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
        
            SetValue(BackgroundProperty, Brushes.Transparent, BindingPriority.Template);
        
            var maskRunTaskSrc  = new TaskCompletionSource();
            var moveAnimTaskSrc = new TaskCompletionSource();
            var motion          = BuildMotionByPlacement(Placement, MotionDuration, false);
            MotionInvoker.Invoke(_motionActor, motion, null,
                () =>
                {
                    _closeAnimating = false;
                    moveAnimTaskSrc.SetResult();
                });
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(duration);
                maskRunTaskSrc.SetResult();
            });
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.WhenAll(moveAnimTaskSrc.Task, moveAnimTaskSrc.Task);
                layer.Children.Remove(this);
                drawer.NotifyClosed();
            });
        }
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
        if (CloseWhenClickOnMask)
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
        _motionActor   = e.NameScope.Find<MotionActorControl>(DrawerContainerTheme.InfoContainerMotionActorPart);
        _infoContainer = e.NameScope.Find<DrawerInfoContainerX>(DrawerContainerTheme.InfoContainerPart);
        if (_infoContainer != null)
        {
            _infoContainer.CloseRequested -= HandleCloseRequested;
            _infoContainer.CloseRequested += HandleCloseRequested;
        }
    }

    private void HandleCloseRequested(object? sender, EventArgs e)
    {
        if (Drawer != null && Drawer.TryGetTarget(out var drawer))
        {
            drawer.IsOpen = false;
        }
    }
}