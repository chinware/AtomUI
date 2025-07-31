using AtomUI.Controls.Themes;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class PopupBuddyLayer : SceneLayer, IShadowAwareLayer
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

    #region 内部属性定义

    internal static readonly StyledProperty<CornerRadius> MaskShadowsContentCornerRadiusProperty = 
        AvaloniaProperty.Register<PopupBuddyLayer, CornerRadius>(nameof (MaskShadowsContentCornerRadius));
    
    internal CornerRadius MaskShadowsContentCornerRadius
    {
        get => GetValue(MaskShadowsContentCornerRadiusProperty);
        set => SetValue(MaskShadowsContentCornerRadiusProperty, value);
    }

    #endregion
    
    private Popup _buddyPopup;
    private IPopupHost? _popupHost;
    private PixelPoint? _lastBuddyPopupPosition;
    private Size? _lastBuddyPopupSize;
    private BaseMotionActor? _motionActor;
    private Panel? _shadowRendererPanel;
    
    public PopupBuddyLayer(Popup buddyPopup, TopLevel parent)
        : base(parent)
    {
        _buddyPopup    = buddyPopup;
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
            ConfigureShadowInfo(popupRoot.Presenter);
            ConfigurePaddingForShadows();
        }
        _popupHost = popupHost;
    }

    private void ConfigureShadowInfo(ContentPresenter? presenter)
    {
        if (presenter != null)
        {
            var content = presenter.Child;
            if (content is IShadowMaskInfoProvider shadowMaskInfoProvider)
            {
                MaskShadowsContentCornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
            }
            else if (content is Border bordered)
            {
                MaskShadowsContentCornerRadius = bordered.CornerRadius;
            }
            else if (content is TemplatedControl templatedControl)
            {
                MaskShadowsContentCornerRadius = templatedControl.CornerRadius;
            }
        }
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
    }
    
    public void Detach()
    {
        Hide();
        if (this is IDisposable disposable)
        {
            disposable.Dispose();
        }
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

            ConfigurePaddingForShadows();
        }
        if (this.IsAttachedToVisualTree() && _shadowRendererPanel != null)
        {
            if (change.Property == MaskShadowsProperty)
            {
                for (var i = 0; i < MaskShadows.Count; ++i)
                {
                    if (_shadowRendererPanel.Children[i] is Border shadowControl)
                    {
                        shadowControl.BoxShadow = new BoxShadows(MaskShadows[i]);
                    }
                }
            }
        }
    }

    private void ConfigurePaddingForShadows()
    {
        var thickness = MaskShadows.Thickness();
        if (_buddyPopup is IPopupHostProvider popupHostProvider)
        {
            if (popupHostProvider.PopupHost is PopupRoot popupRoot)
            {
                if (popupRoot.Presenter?.Child is IArrowAwareShadowMaskInfoProvider arrowAwareShadowMaskInfoProvider)
                {
                    if (arrowAwareShadowMaskInfoProvider.IsShowArrow)
                    {
                        var maskBounds = arrowAwareShadowMaskInfoProvider.GetMaskBounds();
                        var arrowPosition         = arrowAwareShadowMaskInfoProvider.ArrowPosition;
                        var direction = ArrowDecoratedBox.GetDirection(arrowPosition);
                        var delta = arrowAwareShadowMaskInfoProvider.ArrowIndicatorBounds.Height;
                        if (direction == Direction.Bottom)
                        {
                            thickness = new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom + delta);
                        }
                        else if (direction == Direction.Top)
                        {
                            thickness = new Thickness(thickness.Left, thickness.Top + delta, thickness.Right, thickness.Bottom);
                        }
                        else if (direction == Direction.Left)
                        {
                            thickness = new Thickness(thickness.Left + delta, thickness.Top, thickness.Right, thickness.Bottom);
                        }
                        else
                        {
                            thickness = new Thickness(thickness.Left, thickness.Top, thickness.Right + delta, thickness.Bottom);
                        }
                    }
                }
            }
        }

        Padding = thickness;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _shadowRendererPanel = e.NameScope.Find<Panel>(PopupBuddyLayerXThemeConstants.ShadowRendererPart);
        if (_shadowRendererPanel != null)
        {
            var shadowControls = BuildShadowRenderers(MaskShadows);
            _shadowRendererPanel.Children.AddRange(shadowControls);
        }

        // if (MotionActor != null)
        // {
        //     MotionActor.UseRenderTransform = true;
        // }
    }
    
    /// <summary>
    /// 目前的 Avalonia 版本中，当控件渲染到 RenderTargetBitmap 的时候，如果 BoxShadows 的 Count > 1 的时候，如果不是主阴影，后面的阴影如果
    /// 指定 offset，再 RenderScaling > 1 的情况下是错的。
    /// </summary>
    /// <returns></returns>
    private List<Control> BuildShadowRenderers(in BoxShadows shadows)
    {
        // 不知道这里为啥不行
        var renderers = new List<Control>();
        for (var i = 0; i < shadows.Count; ++i)
        {
            var renderer = new Border
            {
                Background      = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                BoxShadow       = new BoxShadows(shadows[i]),
            };
            renderer[!CornerRadiusProperty] = this[!MaskShadowsContentCornerRadiusProperty];
            renderers.Add(renderer);
        }

        return renderers;
    }
}