using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Media;
using AtomUI.Native;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Controls;

internal class WindowBuddyLayer : AvaloniaWindow
{
    protected override Type StyleKeyOverride { get; } = typeof(WindowBuddyLayer);
    
    public static readonly StyledProperty<BoxShadows> FrameShadowsProperty =
        Window.FrameShadowsProperty.AddOwner<WindowBuddyLayer>();
    
    public BoxShadows FrameShadows
    {
        get => GetValue(FrameShadowsProperty);
        set => SetValue(FrameShadowsProperty, value);
    }
    
    private WeakReference<Window>? _targetWindow;
    private PixelRect _logicalBounds;
    private Panel? _shadowRendererLayout;
    private CompositeDisposable? _disposables;
    
    public WindowBuddyLayer()
    {
        IsHitTestVisible                  = false;
        Topmost                           = false;
        Focusable                         = false;
        SystemDecorations                 = SystemDecorations.None;
        WindowState                       = WindowState.Normal;
        this.SetWindowIgnoreMouseEvents(true);
        Width      = 1;
        Height     = 1;
    }

    public void Attach(Window targetWindow)
    {
        _targetWindow                =  new WeakReference<Window>(targetWindow);
        targetWindow.SizeChanged     += HandleTargetWindowSizeChanged;
        targetWindow.PositionChanged += HandleTargetWindowPositionChanged;
        this[!FrameShadowsProperty]  =  targetWindow[!FrameShadowsProperty];
        this[!CornerRadiusProperty]  =  targetWindow[!CornerRadiusProperty];
        Screens.Changed              += HandleScreensChanged;
        _logicalBounds               =  CalculateLogicalBounds();
        _disposables = new CompositeDisposable(3)
        {
            targetWindow.GetObservable(WindowStateProperty).Subscribe(x =>
            {
                if (x == WindowState.Maximized || x == WindowState.FullScreen)
                {
                    Opacity = 0.0;
                }
                else
                {
                    Opacity     = 1.0;
                    WindowState = x;
                }
            })
        };
        Show();
    }

    public void Detach()
    {
        if (_targetWindow != null && _targetWindow.TryGetTarget(out var targetWindow))
        {
            targetWindow.SizeChanged     -= HandleTargetWindowSizeChanged;
            targetWindow.PositionChanged -= HandleTargetWindowPositionChanged;
            Screens.Changed              -= HandleScreensChanged;
        }
        _disposables?.Dispose();
        Close();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _shadowRendererLayout = e.NameScope.Find<Panel>(WindowBuddyLayerThemeConstants.ShadowRendererLayoutPart);
        if (_shadowRendererLayout != null)
        {
            var shadowControls = BuildShadowRenderers(FrameShadows);
            _shadowRendererLayout.Children.AddRange(shadowControls);
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
                BorderThickness = new Thickness(0),
                BoxShadow       = new BoxShadows(shadows[i]),
                CornerRadius    = CornerRadius,
            };
            renderers.Add(renderer);
        }

        return renderers;
    }

    private void HandleTargetWindowSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        var shadowThickness = FrameShadows.Thickness();
        var size = args.NewSize.Inflate(shadowThickness);
        Width = size.Width;
        Height = size.Height;
        if (_targetWindow != null && _targetWindow.TryGetTarget(out var targetWindow))
        {
            var offset          = new Point(targetWindow.Position.X, targetWindow.Position.Y);
            var layerOffset = new Point(offset.X - shadowThickness.Left * DesktopScaling,
                offset.Y - shadowThickness.Top * DesktopScaling);
            Dispatcher.UIThread.Post(() =>
            {
                Position   = new PixelPoint((int)Math.Round(layerOffset.X), (int)Math.Floor(layerOffset.Y + 0.5));
            });
        }
    }

    private void HandleTargetWindowPositionChanged(object? sender, PixelPointEventArgs args)
    {
        var shadowThickness = FrameShadows.Thickness();
        var offset          = new Point(args.Point.X, args.Point.Y);
        var layerOffset = new Point(offset.X - shadowThickness.Left * DesktopScaling,
            offset.Y - shadowThickness.Top * DesktopScaling);
        Dispatcher.UIThread.Post(() =>
        {
            Position   = new PixelPoint((int)Math.Round(layerOffset.X), (int)Math.Floor(layerOffset.Y + 0.5));
        });
    }

    private PixelRect CalculateLogicalBounds()
    {
        if (Screens.ScreenCount == 0)
        {
            return new PixelRect();
        }
        var screens = Screens.All;
        // 计算虚拟桌面的边界
        int minX = screens.Min(s => s.WorkingArea.X);
        int minY = screens.Min(s => s.WorkingArea.Y);
        int maxX = screens.Max(s => s.WorkingArea.X + s.WorkingArea.Width);
        int maxY = screens.Max(s => s.WorkingArea.Y + s.WorkingArea.Height);
        
        return new PixelRect(minX, minY, maxX - minX, maxY - minY);
    }

    private void HandleScreensChanged(object? sender, EventArgs args)
    {
        _logicalBounds = CalculateLogicalBounds();
        InvalidateVisual();
    }
}