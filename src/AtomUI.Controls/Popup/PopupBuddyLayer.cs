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
    private PixelPoint? _lastBuddyPopupPosition;
    private Size? _lastBuddyPopupSize;
    
    internal readonly PopupBuddyDecorator BuddyDecorator;
    
    public PopupBuddyLayer(Popup buddyPopup, TopLevel parent)
        : base(parent)
    {
        _buddyPopup    = buddyPopup;
        BuddyDecorator = new PopupBuddyDecorator(_buddyPopup);
        BindUtils.RelayBind(this, MaskShadowsProperty, BuddyDecorator, PopupBuddyDecorator.MaskShadowsProperty);
        SetMotionActor(BuddyDecorator);
        BuddyDecorator.NotifyMotionTargetAddedToScene();
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
                popupHostProvider.PopupHostChanged += SetupPopupHost; 
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
        var popupOffset          = popupRoot.PlatformImpl.Position;
        var popupOffsetSize      = popupRoot.ClientSize;
        var maskShadowsThickness = MaskShadows.Thickness();
        
        if (popupOffsetSize == _lastBuddyPopupSize && 
            popupOffset == _lastBuddyPopupPosition && 
            maskShadowsThickness == default)
        {
            return;
        }
        
        _lastBuddyPopupSize     = popupOffsetSize;
        _lastBuddyPopupPosition = popupOffset;
        
        var topLevel    = GetTopLevel(popupRoot);
        var scaling     = 1.0;
        if (topLevel is WindowBase windowBase)
        {
            scaling = windowBase.DesktopScaling;
        }
        var offset         = new Point(popupOffset.X, popupOffset.Y);
        
 
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
        MoveAndResize(layerOffset, popupOffsetSize.Inflate(MaskShadows.Thickness()));
    }

    public void Attach()
    {
        SetupPopupHost();
        BuddyDecorator.CaptureContentControl();
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
    }

    public void Detach()
    {
        Hide();
        if (this is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public void AttachWithMotion(Action? aboutToStart = null, Action? completedAction = null)
    {
        BuddyDecorator.CaptureContentControl();
        BuddyDecorator.Opacity = 0.0d;
        BuddyDecorator.NotifySceneShowed();
        aboutToStart?.Invoke();
        var motion       = OpenMotion ?? new ZoomBigInMotion();
        if (MotionDuration != TimeSpan.Zero)
        {
            motion.Duration = MotionDuration;
        }
        NotifyAboutToRunAttachMotion();
        motion.Run(BuddyDecorator, null, () =>
        {
            completedAction?.Invoke();
            if (_buddyPopup.ConfigureBlankMaskWhenMotionAwareOpen)
            {
                BuddyDecorator.ConfigureBlankContentControl();
            }
            NotifyAttachMotionCompleted();
        });
    }
    
    public void DetachWithMotion(Action? aboutToStart = null, Action? completedAction = null)
    {
        var motion       = CloseMotion ?? new ZoomBigOutMotion();
        if (MotionDuration != TimeSpan.Zero)
        {
            motion.Duration = MotionDuration;
        }
        BuddyDecorator.CaptureContentControl();
        NotifyAboutToRunDetachMotion();

        Dispatcher.UIThread.Post(() =>
        {
            aboutToStart?.Invoke();
            Dispatcher.UIThread.Post(() =>
            {
                motion.Run(BuddyDecorator, null, () =>
                {
                    completedAction?.Invoke();
                    NotifyDetachMotionCompleted();
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