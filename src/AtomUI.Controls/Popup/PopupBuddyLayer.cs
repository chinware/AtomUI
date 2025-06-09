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
        Popup.MotionDurationProperty.AddOwner<PopupBuddyLayer>();
    
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
    
    #endregion

    private Popup _buddyPopup;
    private IPopupHost? _popupHost;
    
    private PopupBuddyDecorator _buddyDecorator;
    
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
        var offset = new Point(popupOffset.X, popupOffset.Y);
        var maskShadowsThickness = MaskShadows.Thickness();
        var layerOffset = new Point(offset.X - maskShadowsThickness.Left * scaling,
            offset.Y - maskShadowsThickness.Top * scaling);
        MoveAndResize(layerOffset, popupRoot.ClientSize.Inflate(MaskShadows.Thickness()));
    }

    public void Attach()
    {
        Show();
        SetupPopupHost();
        _buddyDecorator.HideDecoratorContent();
    }

    public void Detach()
    {
        Hide();
    }

    public void AttachWithMotion(Action? aboutToStart = null,
                                 Action? completedAction = null)
    {
        Attach();
        _buddyDecorator.Opacity = 0.0d;
        _buddyDecorator.NotifySceneShowed();
        aboutToStart?.Invoke();
        var motion       = new ZoomBigInMotion(MotionDuration);
        NotifyAboutToRunAttachMotion();
        motion.Run(_buddyDecorator, null, () =>
        {
            completedAction?.Invoke();
            NotifyAttachMotionCompleted();
        });
    }
    
    public void DetachWithMotion(Action? aboutToStart = null,
                                 Action? completedAction = null)
    {
        aboutToStart?.Invoke();
        var motion = new ZoomBigOutMotion(MotionDuration);
        NotifyAboutToRunDetachMotion();
        motion.Run(_buddyDecorator, null, () =>
        {
            completedAction?.Invoke();
            NotifyDetachMotionCompleted();
            Detach();
        });
    }

    protected void NotifyAboutToRunAttachMotion()
    {
        _buddyDecorator.ShowDecoratorContent();
    }
    
    protected void NotifyAttachMotionCompleted()
    {
        Dispatcher.UIThread.Post(() =>
        {
            _buddyDecorator.HideDecoratorContent();
        });
    }
    
    protected void NotifyAboutToRunDetachMotion()
    {
        _buddyDecorator.ShowDecoratorContent();
    }
    
    protected void NotifyDetachMotionCompleted()
    {
        Dispatcher.UIThread.Post(() =>
        {
            _buddyDecorator.HideDecoratorContent();
        });
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