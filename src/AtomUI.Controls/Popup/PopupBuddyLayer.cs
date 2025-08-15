using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class PopupBuddyLayer : SceneLayer, IShadowAwareLayer
{
    #region 公共属性定义
    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Border.BoxShadowProperty.AddOwner<PopupBuddyLayer>();
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        Popup.MotionDurationProperty.AddOwner<PopupBuddyLayer>();

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
    
    internal static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<PopupBuddyLayer>();
    
    internal static readonly StyledProperty<double> ArrowSizeProperty =
        ArrowDecoratedBox.ArrowSizeProperty.AddOwner<PopupBuddyLayer>();
    
    internal static readonly StyledProperty<IBrush?> ArrowFillColorProperty = 
        AvaloniaProperty.Register<PopupBuddyLayer, IBrush?>(nameof(ArrowFillColor));

    internal static readonly StyledProperty<Direction> ArrowDirectionProperty = 
        ArrowDecoratedBox.ArrowDirectionProperty.AddOwner<PopupBuddyLayer>();
    
    internal static readonly DirectProperty<PopupBuddyLayer, Rect> ArrowIndicatorLayoutBoundsProperty =
        AvaloniaProperty.RegisterDirect<PopupBuddyLayer, Rect>(
            nameof(ArrowIndicatorLayoutBounds),
            o => o.ArrowIndicatorLayoutBounds,
            (o, v) => o.ArrowIndicatorLayoutBounds = v);
    
    public static readonly StyledProperty<Thickness> MotionPaddingProperty = 
        AvaloniaProperty.Register<PopupBuddyLayer, Thickness>(nameof (MotionPadding));
    
    internal CornerRadius MaskShadowsContentCornerRadius
    {
        get => GetValue(MaskShadowsContentCornerRadiusProperty);
        set => SetValue(MaskShadowsContentCornerRadiusProperty, value);
    }
    
    internal bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }
    
    internal double ArrowSize
    {
        get => GetValue(ArrowSizeProperty);
        set => SetValue(ArrowSizeProperty, value);
    }
    
    internal IBrush? ArrowFillColor
    {
        get => GetValue(ArrowFillColorProperty);
        set => SetValue(ArrowFillColorProperty, value);
    }

    internal Direction ArrowDirection
    {
        get => GetValue(ArrowDirectionProperty);
        set => SetValue(ArrowDirectionProperty, value);
    }
        
    private Rect _arrowIndicatorLayoutBounds;

    internal Rect ArrowIndicatorLayoutBounds
    {
        get => _arrowIndicatorLayoutBounds;
        set => SetAndRaise(ArrowIndicatorLayoutBoundsProperty, ref _arrowIndicatorLayoutBounds, value);
    }
    
    internal Thickness MotionPadding
    {
        get => GetValue(MotionPaddingProperty);
        set => SetValue(MotionPaddingProperty, value);
    }
    
    #endregion
    
    private Popup _buddyPopup;
    private IPopupHost? _popupHost;
    private PixelPoint? _lastBuddyPopupPosition;
    private Size? _lastBuddyPopupSize;
    private Panel? _shadowRendererPanel;
    private LayoutTransformControl? _arrowIndicatorLayout;
    private ContentPresenter? _ghostContentPresenter;
    private CompositeDisposable? _disposables;
    private IArrowAwareShadowMaskInfoProvider? _popupArrowDecoratedBox;
    
    // 用于保证动画状态最终一致性
    private IDisposable? _openMotionForceDisposable;
    private IDisposable? _closeMotionForceDisposable;
    
    internal Popup BuddyPopup => _buddyPopup;
    
    static PopupBuddyLayer()
    {
        AffectsRender<PopupBuddyLayer>(MaskShadowsContentCornerRadiusProperty, ArrowFillColorProperty);
        AffectsMeasure<PopupBuddyLayer>(ArrowIndicatorLayoutBoundsProperty, IsShowArrowProperty);
        AffectsArrange<PopupBuddyLayer>(ArrowSizeProperty, ArrowDirectionProperty);
    }
    
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
            if (content is IArrowAwareShadowMaskInfoProvider arrowAwareShadowMaskInfoProvider)
            {
                var arrowDecoratedBox = arrowAwareShadowMaskInfoProvider.GetArrowDecoratedBox();
                _popupArrowDecoratedBox = arrowDecoratedBox;
                _popupArrowDecoratedBox.SetArrowOpacity(0.0);
                this[!MaskShadowsContentCornerRadiusProperty]    = arrowDecoratedBox[!ArrowDecoratedBox.CornerRadiusProperty];
                this[!ArrowIndicatorLayoutBoundsProperty] =
                    arrowDecoratedBox[!ArrowDecoratedBox.ArrowIndicatorLayoutBoundsProperty];
                this[!ArrowSizeProperty]      = arrowDecoratedBox[!ArrowDecoratedBox.ArrowSizeProperty];
                this[!ArrowDirectionProperty] = arrowDecoratedBox[!ArrowDecoratedBox.ArrowDirectionProperty];
                this[!ArrowFillColorProperty] = arrowDecoratedBox[!ArrowDecoratedBox.BackgroundProperty];
                this[!IsShowArrowProperty] = arrowDecoratedBox[!ArrowDecoratedBox.IsShowArrowProperty];
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
        
        var topLevel = GetTopLevel(popupRoot);
        var scaling  = 1.0;
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
        _disposables = new CompositeDisposable();
        _disposables.Add(BindUtils.RelayBind(_buddyPopup, Popup.MaskShadowsProperty, this, MaskShadowsProperty));
        _disposables.Add(BindUtils.RelayBind(_buddyPopup, Popup.MotionDurationProperty, this, MotionDurationProperty));
        _disposables.Add(BindUtils.RelayBind(_buddyPopup, Popup.OpenMotionProperty, this, OpenMotionProperty));
        _disposables.Add(BindUtils.RelayBind(_buddyPopup, Popup.CloseMotionProperty, this, CloseMotionProperty));
        SetupPopupHost();
    }
    
    public void Detach()
    {
        Hide();
        _disposables?.Dispose();
        _disposables = null;
        if (this is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
    
    public void RunOpenMotion(Action? aboutToStart = null, Action? completedAction = null)
    {
        if (MotionActor == null)
        {
            completedAction?.Invoke();
            _openMotionForceDisposable?.Dispose();
            return;
        }
        var motion       = OpenMotion ?? new ZoomBigInMotion();
        if (MotionDuration != TimeSpan.Zero)
        {
            motion.Duration = MotionDuration;
        }

        var shadowAwareLayer = this as IShadowAwareLayer;
        Debug.Assert(shadowAwareLayer != null);
        shadowAwareLayer.NotifyOpenMotionAboutToStart();

        var completedFuncCalled = false;
        
        _openMotionForceDisposable?.Dispose();
        _openMotionForceDisposable = DispatcherTimer.RunOnce(() =>
        {
            if (!completedFuncCalled)
            {
                shadowAwareLayer.NotifyOpenMotionCompleted();
                completedAction?.Invoke();
                completedFuncCalled = true;
            }
      
        }, motion.Duration * 1.2);
        
        motion.Run(MotionActor, aboutToStart, () =>
        {
            _openMotionForceDisposable.Dispose();
            if (!completedFuncCalled)
            {
                shadowAwareLayer.NotifyOpenMotionCompleted();
                completedAction?.Invoke();
                completedFuncCalled = true;
            }
        });
    }

    public void RunCloseMotion(Action? aboutToStart = null, Action? completedAction = null)
    {
        if (MotionActor == null)
        {
            completedAction?.Invoke();
            _closeMotionForceDisposable?.Dispose();
            return;
        }
        var motion       = CloseMotion ?? new ZoomBigOutMotion();
        if (MotionDuration != TimeSpan.Zero)
        {
            motion.Duration = MotionDuration;
        }
        var shadowAwareLayer = this as IShadowAwareLayer;
        Debug.Assert(shadowAwareLayer != null);
        shadowAwareLayer.NotifyCloseMotionAboutToStart();
        
        var completedFuncCalled = false;
        
        _closeMotionForceDisposable?.Dispose();
        _closeMotionForceDisposable = DispatcherTimer.RunOnce(() =>
        {
            if (!completedFuncCalled)
            {
                shadowAwareLayer.NotifyCloseMotionCompleted();
                completedAction?.Invoke();
                completedFuncCalled = true;
            }
        }, motion.Duration * 1.2);
        motion.Run(MotionActor, aboutToStart, () =>
        {
            _closeMotionForceDisposable.Dispose();
            if (!completedFuncCalled)
            {
                shadowAwareLayer.NotifyCloseMotionCompleted();
                completedAction?.Invoke();
                completedFuncCalled = true;
            }
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

            ConfigurePaddingForShadows();
        }
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == MaskShadowsProperty)
            {
                if (_shadowRendererPanel != null)
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
    }

    private void ConfigurePaddingForShadows()
    {
        var thickness = MaskShadows.Thickness();
        MotionPadding = thickness;
        if (_buddyPopup is IPopupHostProvider popupHostProvider)
        {
            if (popupHostProvider.PopupHost is PopupRoot popupRoot)
            {
                if (popupRoot.Presenter?.Child is IArrowAwareShadowMaskInfoProvider arrowAwareShadowMaskInfoProvider)
                {
                    if (arrowAwareShadowMaskInfoProvider.IsShowArrow())
                    {
                        var arrowPosition = arrowAwareShadowMaskInfoProvider.GetArrowPosition();
                        var direction     = ArrowDecoratedBox.GetDirection(arrowPosition);
                        var delta         = arrowAwareShadowMaskInfoProvider.GetArrowIndicatorBounds().Height + 0.5;
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
        if (_popupHost is PopupRoot popupRoot)
        {
            var popupMotionAction  = popupRoot.FindDescendantOfType<MotionActor>();
            if (popupMotionAction != null && MotionActor != null)
            {
                popupMotionAction.Follow(MotionActor);
            }
        }

        _arrowIndicatorLayout = e.NameScope.Find<LayoutTransformControl>(PopupBuddyLayerThemeConstants.ArrowIndicatorLayoutPart);
        _ghostContentPresenter = e.NameScope.Find<ContentPresenter>(PopupBuddyLayerThemeConstants.GhostContentPresenterPart);
       
        _shadowRendererPanel = e.NameScope.Find<Panel>(PopupBuddyLayerThemeConstants.ShadowRendererPart);
        if (_shadowRendererPanel != null)
        {
            var shadowControls = BuildShadowRenderers(MaskShadows);
            _shadowRendererPanel.Children.AddRange(shadowControls);
        }
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
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (IsShowArrow && _arrowIndicatorLayout != null && _shadowRendererPanel != null)
        {
            var shadowRenderBounds = _shadowRendererPanel.Bounds;
            var indicatorBounds    = _arrowIndicatorLayout.Bounds;
            var offsetX            = 0.0;
            var offsetY            = 0.0;
            if (ArrowDirection == Direction.Bottom)
            {
                offsetX = ArrowIndicatorLayoutBounds.X;
                offsetY = shadowRenderBounds.Bottom;
               
            }
            else if (ArrowDirection == Direction.Top)
            {
                offsetX = ArrowIndicatorLayoutBounds.X;
                offsetY = shadowRenderBounds.Top - indicatorBounds.Height;
            }
            else if (ArrowDirection == Direction.Left)
            {
                offsetX = - indicatorBounds.Width;
                offsetY = ArrowIndicatorLayoutBounds.Top;
            }
            else if (ArrowDirection == Direction.Right)
            {
                offsetX = shadowRenderBounds.Width;
                offsetY = ArrowIndicatorLayoutBounds.Top;
            }
            _arrowIndicatorLayout.Arrange(new Rect(new Point(offsetX, offsetY), new Size(indicatorBounds.Width, indicatorBounds.Height)));
        }
        return size;
    }

    void IShadowAwareLayer.NotifyOpenMotionAboutToStart()
    {
        Debug.Assert(_ghostContentPresenter != null);
        var popupPresenter = _popupHost?.Presenter;
        if (_arrowIndicatorLayout != null)
        {
            _arrowIndicatorLayout.IsVisible = false;
        }
        if (_popupArrowDecoratedBox != null)
        {
            _popupArrowDecoratedBox.SetArrowOpacity(1.0);
        }
        if (popupPresenter != null)
        {
            _ghostContentPresenter.Content = new MotionTargetBitmapControl(popupPresenter.CaptureCurrentBitmap())
            {
                Width = popupPresenter.DesiredSize.Width,
                Height = popupPresenter.DesiredSize.Height,
            };
            _ghostContentPresenter.Opacity = 1.0;
            popupPresenter.Opacity         = 0.0;
        }
    }

    void IShadowAwareLayer.NotifyOpenMotionCompleted()
    {
        Debug.Assert(_ghostContentPresenter != null);
        var popupPresenter = _popupHost?.Presenter;

        if (popupPresenter != null)
        {
            popupPresenter.Opacity         = 1.0;
        }
        if (_popupArrowDecoratedBox != null)
        {
            _popupArrowDecoratedBox.SetArrowOpacity(0.0);
        }
        if (_arrowIndicatorLayout != null)
        {
            _arrowIndicatorLayout.IsVisible = true;
        }
        _ghostContentPresenter.Opacity = 0.0;
        _ghostContentPresenter.Content   = null;
    }
    
    void IShadowAwareLayer.NotifyCloseMotionAboutToStart()
    {
        Debug.Assert(_ghostContentPresenter != null);
        Debug.Assert(MotionActor != null);
        var popupPresenter = _popupHost?.Presenter;
        if (_popupArrowDecoratedBox != null)
        {
            _popupArrowDecoratedBox.SetArrowOpacity(1.0);
        }
        if (popupPresenter != null)
        {
            _ghostContentPresenter.Opacity = 1.0;
        }
    }

    void IShadowAwareLayer.NotifyCloseMotionCompleted()
    {
    }
}