using System.Reactive.Disposables;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Native;
using AtomUI.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class PopupShadowLayer : AvaloniaObject, IShadowDecorator
{
    #region 公共属性定义

    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Border.BoxShadowProperty.AddOwner<PopupShadowLayer>();

    public static readonly StyledProperty<Control?> ChildProperty =
        AvaloniaProperty.Register<PopupShadowLayer, Control?>(nameof(Child));

    public static readonly StyledProperty<double> OpacityProperty =
        Visual.OpacityProperty.AddOwner<PopupShadowLayer>();

    public BoxShadows MaskShadows
    {
        get => GetValue(MaskShadowsProperty);
        set => SetValue(MaskShadowsProperty, value);
    }

    public Control? Child
    {
        get => GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }
    
    public double Opacity
    {
        get => GetValue(Visual.OpacityProperty);
        set => SetValue(Visual.OpacityProperty, value);
    }

    public IPopupHost? ShadowLayerHost => _openState?.PopupHost;

    #endregion

    private Popup _target;
    private ShadowLayerPopupOpenState? _openState;
    private ShadowRenderer? _shadowRenderer;
    private IManagedPopupPositionerPopup? _managedPopupPositionerPopup;

    public PopupShadowLayer(Popup target)
    {
        _target = target;
    }

    private void HandleTargetPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (ReferenceEquals(sender, _target) && e.Property == ChildProperty)
        {
            CreateShadowRenderer();
        }
    }

    private void CreateShadowRenderer()
    {
        ShadowLayerHost?.SetChild(null);
        _shadowRenderer ??= new ShadowRenderer();
        BindUtils.RelayBind(this, OpacityProperty, _shadowRenderer, OpacityProperty);
        var layout = new Canvas();
        layout.Children.Add(_shadowRenderer);
        Child = layout;
        ConfigureShadowRenderer();
        ShadowLayerHost?.SetChild(Child);
    }
    
    private void ConfigureShadowRenderer()
    {
        if (_target.Child != null && _shadowRenderer != null)
        {
            // 理论上现在已经有大小了
            var content            = _target.Child;
            var targetPopupHostSize = (_target.Host as PopupRoot)?.ClientSize ?? content.Bounds.Size;
            _shadowRenderer.Shadows = MaskShadows;
            CornerRadius cornerRadius = default;
            if (content is IShadowMaskInfoProvider shadowMaskInfoProvider)
            {
                cornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
                var maskBounds   = shadowMaskInfoProvider.GetMaskBounds();
                var rendererSize = CalculateShadowRendererSize(new Size(maskBounds.Width, maskBounds.Height));
                Canvas.SetLeft(_shadowRenderer, maskBounds.Left);
                Canvas.SetTop(_shadowRenderer, maskBounds.Top);
                _shadowRenderer.Width  = rendererSize.Width;
                _shadowRenderer.Height = rendererSize.Height;
            }
            else if (content is Border bordered)
            {
                cornerRadius = bordered.CornerRadius;
                var rendererSize = CalculateShadowRendererSize(new Size(
                    Math.Max(targetPopupHostSize.Width, content.DesiredSize.Width),
                    Math.Max(targetPopupHostSize.Height, content.DesiredSize.Height)));
                _shadowRenderer.Width  = rendererSize.Width;
                _shadowRenderer.Height = rendererSize.Height;
            }
            else if (content is TemplatedControl templatedControl)
            {
                cornerRadius = templatedControl.CornerRadius;
                var rendererSize = CalculateShadowRendererSize(new Size(
                    Math.Max(targetPopupHostSize.Width, templatedControl.DesiredSize.Width),
                    Math.Max(targetPopupHostSize.Height, templatedControl.DesiredSize.Height)));
                _shadowRenderer.Width  = rendererSize.Width;
                _shadowRenderer.Height = rendererSize.Height;
            }

            _shadowRenderer.MaskCornerRadius = cornerRadius;
        }
    }
    
    private Size CalculateShadowRendererSize(Size content)
    {
        var shadowThickness = MaskShadows.Thickness();
        var targetWidth     = shadowThickness.Left + shadowThickness.Right;
        var targetHeight    = shadowThickness.Top + shadowThickness.Bottom;
        targetWidth  += content.Width;
        targetHeight += content.Height;
        return new Size(targetWidth, targetHeight);
    }

    protected virtual void NotifyPopupHostCreated(IPopupHost popupHost)
    {
    }

    public virtual void Open()
    {
        // Popup is currently open
        if (_openState != null)
        {
            return;
        }
        
        var popupHost = OverlayPopupHost.CreatePopupHost(_target.PlacementTarget ?? _target.GetVisualParent()!, DependencyResolver.Instance);
        popupHost.Topmost = false;
        NotifyPopupHostCreated(popupHost);
        if (popupHost is PopupRoot popupRoot)
        {
            popupRoot.SetWindowIgnoreMouseEvents(true);
            if (popupRoot.PlatformImpl?.PopupPositioner is ManagedPopupPositioner managedPopupPositioner)
            {
                _managedPopupPositionerPopup = managedPopupPositioner.GetManagedPopupPositionerPopup();
            }
        }

        var handlerCleanup = new CompositeDisposable(7);

        if (Child == null && _target.Child != null)
        {
            // 尝试手动
            CreateShadowRenderer();
        }
        else
        {
            ConfigureShadowRenderer();
        }
        
        popupHost.SetChild(Child);
        ((ISetLogicalParent)popupHost).SetParent(_target);
        
        UpdateLayoutHostPositionAndSize(popupHost);

        if (popupHost is PopupRoot topLevelPopup)
        {
            topLevelPopup
                .Bind(
                    ThemeVariantScope.ActualThemeVariantProperty,
                    this.GetBindingObservable(ThemeVariantScope.ActualThemeVariantProperty))
                .DisposeWith(handlerCleanup);
        }

        SubscribeToEventHandler<IPopupHost, EventHandler<TemplateAppliedEventArgs>>(popupHost, RootTemplateApplied,
            (x, handler) => x.TemplateApplied += handler,
            (x, handler) => x.TemplateApplied -= handler).DisposeWith(handlerCleanup);

        var targetPopupRoot = _target.Host as PopupRoot;

        if (targetPopupRoot != null)
        {
            SubscribeToEventHandler<PopupRoot, EventHandler<PixelPointEventArgs>>(targetPopupRoot,
                HandleParentPopupPositionChanged,
                (x, handler) => x.PositionChanged += handler,
                (x, handler) => x.PositionChanged -= handler).DisposeWith(handlerCleanup);
        }

        SubscribeToEventHandler<Popup, EventHandler<AvaloniaPropertyChangedEventArgs>>(_target,
            HandleTargetPropertyChanged,
            (x, handler) => x.PropertyChanged += handler,
            (x, handler) => x.PropertyChanged -= handler).DisposeWith(handlerCleanup);

        var cleanupPopup = Disposable.Create((popupHost, handlerCleanup), state =>
        {
            state.handlerCleanup.Dispose();

            state.popupHost.SetChild(null);
            state.popupHost.Hide();

            ((ISetLogicalParent)state.popupHost).SetParent(null);
            _managedPopupPositionerPopup = null;
            state.popupHost.Dispose();
        });

        _openState        = new ShadowLayerPopupOpenState(popupHost, cleanupPopup);
        popupHost.Show();
    
        if (targetPopupRoot != null)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (_target.Host is not null)
                {
                    _target.Host.Topmost = true;
                }
                targetPopupRoot.Activate();
            });
        }
    }

    public virtual void Close()
    {
        if (_openState is null)
        {
            return;
        }

        _openState.Dispose();
        _openState = null;
    }

    private void UpdateLayoutHostPositionAndSize(IPopupHost popupHost)
    {
        if (_target.Host is PopupRoot targetPopupRoot && _shadowRenderer != null)
        {
            var    impl           = targetPopupRoot.PlatformImpl!;
            var    targetPosition = impl.Position;
            double offsetX        = targetPosition.X;
            double offsetY        = targetPosition.Y;

            var scaling = _managedPopupPositionerPopup!.Scaling;
            
            var shadowThickness = MaskShadows.Thickness();
            offsetX -= shadowThickness.Left * scaling;
            offsetY -= shadowThickness.Top * scaling;

            offsetX = Math.Round(offsetX);
            offsetY = Math.Floor(offsetY + 0.5);

            _managedPopupPositionerPopup?.MoveAndResize(new Point(offsetX, offsetY),
                new Size(_shadowRenderer.Width, _shadowRenderer.Height));
        }
    }

    private void HandleParentPopupPositionChanged(object? src, PixelPointEventArgs e)
    {
        if (src is PopupRoot targetPopupRoot)
        {
            UpdateLayoutHostPositionAndSize(targetPopupRoot);
        }
    }
    

    private void RootTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        if (_openState is null)
        {
            return;
        }

        var popupHost = _openState.PopupHost;

        popupHost.TemplateApplied -= RootTemplateApplied;

        _openState.SetPresenterSubscription(null);

        // If the Popup appears in a control template, then the child controls
        // that appear in the popup host need to have their TemplatedParent
        // properties set.
        if (_target.TemplatedParent != null && popupHost.Presenter is Control presenter)
        {
            presenter.ApplyTemplate();

            var presenterSubscription = presenter.GetObservable(ContentPresenter.ChildProperty)
                                                 .Subscribe(SetTemplatedParentAndApplyChildTemplates);

            _openState.SetPresenterSubscription(presenterSubscription);
        }
    }

    private void SetTemplatedParentAndApplyChildTemplates(Control? control)
    {
        if (control != null)
        {
            TemplatedControlUtils.ApplyTemplatedParent(control, _target.TemplatedParent);
        }
    }

    private static IDisposable SubscribeToEventHandler<T, TEventHandler>(
        T target, TEventHandler handler, Action<T, TEventHandler> subscribe, Action<T, TEventHandler> unsubscribe)
    {
        subscribe(target, handler);

        return Disposable.Create((unsubscribe, target, handler),
            state => state.unsubscribe(state.target, state.handler));
    }

    private class ShadowLayerPopupOpenState : IDisposable
    {
        public IPopupHost PopupHost { get; }

        private readonly IDisposable _cleanup;
        private IDisposable? _presenterCleanup;

        public ShadowLayerPopupOpenState(IPopupHost popupHost, IDisposable cleanup)
        {
            PopupHost = popupHost;
            _cleanup  = cleanup;
        }

        public void SetPresenterSubscription(IDisposable? presenterCleanup)
        {
            _presenterCleanup?.Dispose();
            _presenterCleanup = presenterCleanup;
        }

        public void Dispose()
        {
            _presenterCleanup?.Dispose();
            _cleanup.Dispose();
        }
    }
    
    private class DependencyResolver : IAvaloniaDependencyResolver
    {
        /// <summary>
        /// Gets the default instance of <see cref="DependencyResolver"/>.
        /// </summary>
        public static readonly DependencyResolver Instance = new DependencyResolver();

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>A service of the requested type.</returns>
        public object? GetService(Type serviceType)
        {
            return AvaloniaLocator.Current.GetService(serviceType);
        }
    }
}