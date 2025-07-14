using AtomUI.Data;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class PopupBuddyLayer : SceneLayer, IPopupBuddyLayer, IShadowAwareLayer
{
    #region 公共属性定义

    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Border.BoxShadowProperty.AddOwner<PopupBuddyLayer>();
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<PopupBuddyLayer>();
    
    public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty = 
        Popup.OpenMotionProperty.AddOwner<PopupBuddyLayer>();
        
    public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty = 
        Popup.CloseMotionProperty.AddOwner<PopupBuddyLayer>();
    
    public BoxShadows MaskShadows
    {
        get => GetValue(MaskShadowsProperty);
        set => SetValue(MaskShadowsProperty, value);
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

    private Popup _buddyPopup;
    private IPopupHost? _popupHost;
    
    internal PopupBuddyDecorator _buddyDecorator;
    
    public PopupBuddyLayer(Popup buddyPopup, TopLevel parent)
        : base(parent)
    {
        _buddyPopup     = buddyPopup;
        _buddyDecorator = new PopupBuddyDecorator(_buddyPopup);
        BindUtils.RelayBind(this, MaskShadowsProperty, _buddyDecorator, PopupBuddyDecorator.MaskShadowsProperty);
        SetMotionActor(_buddyDecorator);
        _buddyDecorator.NotifyMotionTargetAddedToScene();
    }

    private void SetupPopupHost()
    {
        if (_buddyPopup is IPopupHostProvider popupHostProvider)
        {
            if (popupHostProvider.PopupHost != null)
            {
                SetupPopupHost(popupHostProvider.PopupHost);
            }
            else
            {
                popupHostProvider.PopupHostChanged += host =>
                {
                    SetupPopupHost(host);
                }; 
            }
        }
    }
    
    private void SetupPopupHost(IPopupHost? popupHost)
    {
        if (popupHost is PopupRoot popupRoot)
        {
            if (_popupHost is PopupRoot oldPopupRoot)
            {
                oldPopupRoot.SizeChanged     -= HandleBuddyPopupRootSizeChanged;
                oldPopupRoot.PositionChanged -= HandleBuddyPopupRootPositionChanged;
            }
            popupRoot.SizeChanged     += HandleBuddyPopupRootSizeChanged;
            popupRoot.PositionChanged += HandleBuddyPopupRootPositionChanged;
            ConfigureSizeAndPosition(popupRoot);
        }
        _popupHost = popupHost;
    }

    private void HandleBuddyPopupRootSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (sender is PopupRoot popupRoot)
        {
            ConfigureSizeAndPosition(popupRoot);
        }
    }

    private void HandleBuddyPopupRootPositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (sender is PopupRoot popupRoot)
        {
            ConfigureSizeAndPosition(popupRoot);
        }
    }

    private void ConfigureSizeAndPosition(PopupRoot popupRoot)
    {
        if (popupRoot.PlatformImpl == null)
        {
            return;
        }
        // 这个是否大小和位置信息都有了
        var popupOffset = popupRoot.PlatformImpl.Position;
        var topLevel    = GetTopLevel(popupRoot);
        var scaling     = 1.0;
        if (topLevel is WindowBase windowBase)
        {
            scaling = windowBase.DesktopScaling;
        }
        var offset         = new Point(popupOffset.X, popupOffset.Y);
        
        var maskShadowsThickness = MaskShadows.Thickness();
        var layerOffset = new Point(offset.X - maskShadowsThickness.Left * scaling,
            offset.Y - maskShadowsThickness.Top * scaling);
        if (OperatingSystem.IsMacOS())
        {
            var topOffsetMark = 0d;
            if (ManagedPopupPositionerPopup != null)
            {
                var screens        = ManagedPopupPositionerPopup.Screens;
                foreach (var screen in screens)
                {
                    topOffsetMark = Math.Max(topOffsetMark, screen.WorkingArea.Top);
                }
            }
            var delta = layerOffset.Y - topOffsetMark;
            if (delta < 0)
            {
                RenderTransform = new TranslateTransform(0, delta);
            }
            else
            {
                RenderTransform = null;
            }
        }
        MoveAndResize(layerOffset, popupRoot.ClientSize.Inflate(MaskShadows.Thickness()));
    }

    public void Attach()
    {
        _buddyDecorator.CaptureContentControl();
        if (OperatingSystem.IsMacOS())
        {
            PlatformImpl?.SetTopmost(true);
            var popupRoot = _popupHost as PopupRoot;
            popupRoot?.PlatformImpl?.SetTopmost(true);
            Show();
            popupRoot?.Activate();
        }
        else if (OperatingSystem.IsLinux())
        {
            Show();
            var popupRoot = _popupHost as PopupRoot;
            popupRoot?.Hide();
            popupRoot?.Show();
        }
        else
        {
            Show();
        }
       
        SetupPopupHost();
    }

    public void Detach()
    {
        _buddyDecorator.CaptureContentControl();
        if (this is IDisposable disposable)
        {
            disposable.Dispose();
        }
        Hide();
    }

    public void AttachWithMotion(Action? aboutToStart = null,
                                 Action? completedAction = null)
    {
        Attach();
        _buddyDecorator.CaptureContentControl();
        _buddyDecorator.Opacity = 0.0d;
        _buddyDecorator.NotifySceneShowed();
        aboutToStart?.Invoke();
        var motion       = OpenMotion ?? new ZoomBigInMotion();
        if (MotionDuration != TimeSpan.Zero)
        {
            motion.Duration = MotionDuration;
        }
        NotifyAboutToRunAttachMotion();
        motion.Run(_buddyDecorator, null, () =>
        {
            completedAction?.Invoke();
            _buddyDecorator.ConfigureBlankContentControl();
            NotifyAttachMotionCompleted();
        });
    }
    
    public void DetachWithMotion(Action? aboutToStart = null,
                                 Action? completedAction = null)
    {
        var motion       = CloseMotion ?? new ZoomBigOutMotion();
        if (MotionDuration != TimeSpan.Zero)
        {
            motion.Duration = MotionDuration;
        }
        _buddyDecorator.CaptureContentControl();
        NotifyAboutToRunDetachMotion();

        Dispatcher.UIThread.Post(() =>
        {
            aboutToStart?.Invoke();
            Dispatcher.UIThread.Post(() =>
            {
                motion.Run(_buddyDecorator, null, () =>
                {
                    completedAction?.Invoke();
                    NotifyDetachMotionCompleted();
                    Detach();
                });
            });
        });
    }

    protected void NotifyAboutToRunAttachMotion()
    {
    }
    
    protected void NotifyAttachMotionCompleted()
    {
    }
    
    protected void NotifyAboutToRunDetachMotion()
    {
    }
    
    protected void NotifyDetachMotionCompleted()
    {
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MaskShadowsProperty)
        {
            if (_popupHost is PopupRoot popupRoot)
            {
                ConfigureSizeAndPosition(popupRoot);
            }
        }
    }
}