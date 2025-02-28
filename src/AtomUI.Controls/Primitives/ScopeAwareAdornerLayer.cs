using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Primitives;

public class ScopeAwareAdornerLayer : Canvas
{
    private const int ScopeAwareAdornerLayerZIndex = int.MaxValue - 1000;

    #region 公共属性

    public static readonly AttachedProperty<Control?> AdornedElementProperty =
        AvaloniaProperty.RegisterAttached<ScopeAwareAdornerLayer, Visual, Control?>("AdornedElement");

    public static readonly AttachedProperty<Control?> AdornerProperty =
        AvaloniaProperty.RegisterAttached<ScopeAwareAdornerLayer, Visual, Control?>("Adorner");

    public static readonly StyledProperty<Visual?> LayerHostProperty = AvaloniaProperty
        .Register<ScopeAwareAdornerLayer, Visual?>(nameof(LayerHost));

    public Visual? LayerHost
    {
        get => GetValue(LayerHostProperty);
        set => SetValue(LayerHostProperty, value);
    }

    #endregion

    #region 内部属性定义

    private static readonly AttachedProperty<AdornedElementInfo?> AdornedElementInfoProperty =
        AvaloniaProperty.RegisterAttached<ScopeAwareAdornerLayer, Visual, AdornedElementInfo?>("AdornedElementInfo");

    private static readonly AttachedProperty<ScopeAwareAdornerLayer?> SavedAdornerLayerProperty =
        AvaloniaProperty.RegisterAttached<Visual, Visual, ScopeAwareAdornerLayer?>("SavedAdornerLayer");

    #endregion

    static ScopeAwareAdornerLayer()
    {
        AdornedElementProperty.Changed.Subscribe(HandleAdornedElementChanged);
        AdornerProperty.Changed.Subscribe(HandleAdornerChanged);
    }

    public ScopeAwareAdornerLayer()
    {
        Children.CollectionChanged += ChildrenCollectionChanged;
    }

    public static Visual? GetAdornedElement(Visual control)
    {
        return control.GetValue(AdornedElementProperty);
    }

    public static void SetAdornedElement(Visual control, Visual? adorned)
    {
        control.SetValue(AdornedElementProperty, adorned);
    }

    public static ScopeAwareAdornerLayer? GetLayer(Visual visual)
    {
        Layoutable? layerHost = visual.FindAncestorOfType<ScrollContentPresenter>(true);
        var         adorned   = GetAdornedElement(visual);
        if (layerHost != null && adorned != null)
        {
            while (layerHost != null && layerHost.IsVisualAncestorOf(adorned) == false)
            {
                layerHost = layerHost.FindAncestorOfType<ScrollContentPresenter>();
            }
        }

        layerHost ??= visual.FindAncestorOfType<VisualLayerManager>();
        layerHost ??= TopLevel.GetTopLevel(visual);

        if (layerHost == null)
        {
            return null;
        }

        var layer =
            layerHost.GetVisualChildren().FirstOrDefault(c => c is ScopeAwareAdornerLayer) as ScopeAwareAdornerLayer;
        layer ??= InjectLayer(layerHost);

        return layer;
    }

    public static Control? GetAdorner(Visual visual)
    {
        return visual.GetValue(AdornerProperty);
    }

    public static void SetAdorner(Visual visual, Control? adorner)
    {
        visual.SetValue(AdornerProperty, adorner);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            if (child is AvaloniaObject ao)
            {
                var info = ao.GetValue(AdornedElementInfoProperty);

                if (info != null && info.Bounds.HasValue)
                {
                    child.Measure(info.Bounds.Value.Bounds.Size);
                }
                else
                {
                    child.Measure(availableSize);
                }
            }
        }

        return default;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in Children)
        {
            if (child is AvaloniaObject ao)
            {
                var info = ao.GetValue(AdornedElementInfoProperty);

                if (info != null && info.Bounds.HasValue)
                {
                    child.RenderTransform       = new MatrixTransform(info.Bounds.Value.Transform);
                    child.RenderTransformOrigin = new RelativePoint(new Point(0, 0), RelativeUnit.Absolute);
                    UpdateClip(child, info.Bounds.Value);
                    child.Arrange(info.Bounds.Value.Bounds);
                }
                else
                {
                    ArrangeChild(child, finalSize);
                }
            }
        }

        return finalSize;
    }

    private void ChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (Visual i in e.NewItems!)
                {
                    UpdateAdornedElement(i, i.GetValue(AdornedElementProperty));
                }

                break;
        }

        InvalidateArrange();
    }

    private void UpdateAdornedElement(Visual adorner, Control? adorned)
    {
        var info = adorner.GetValue(AdornedElementInfoProperty);

        if (info != null)
        {
            info.Subscription!.Dispose();

            if (adorned == null)
            {
                adorner.ClearValue(AdornedElementInfoProperty);
            }
        }

        if (adorned != null)
        {
            if (info == null)
            {
                info = new AdornedElementInfo();
                adorner.SetValue(AdornedElementInfoProperty, info);
            }

            info.Subscription = adorned.GetObservable(BoundsProperty).Subscribe(x =>
            {
                Debug.Assert(LayerHost != null);
                double offsetX = 0d;
                double offsetY = 0d;

                var relateToHostPoint = adorned.TranslatePoint(new Point(0, 0), LayerHost) ?? new Point(0, 0);
                if (LayerHost is ScrollContentPresenter scrollContentPresenter)
                {
                    offsetX = scrollContentPresenter.Offset.X;
                    offsetY = scrollContentPresenter.Offset.Y;
                }

                offsetX += relateToHostPoint.X;
                offsetY += relateToHostPoint.Y;
                var translationMatrix = LayerHost != null
                    ? Matrix.CreateTranslation(offsetX, offsetY)
                    : Matrix.Identity;
                var adornedWidth  = Math.Max(adorned.Bounds.Width, adorned.DesiredSize.Width);
                var adornedHeight = Math.Max(adorned.Bounds.Height, adorned.DesiredSize.Height);
                info.Bounds = new TransformedBounds(new Rect(new Size(adornedWidth, adornedHeight)),
                    new Rect(new Size(adornedWidth, adornedHeight)),
                    translationMatrix);
                InvalidateMeasure();
            });
        }
    }

    private void UpdateClip(Control control, TransformedBounds bounds)
    {
        if (!(control.Clip is RectangleGeometry clip))
        {
            clip         = new RectangleGeometry();
            control.Clip = clip;
        }

        var clipBounds = bounds.Bounds;

        clip.Rect = clipBounds;
    }

    private static void HandleAdornedElementChanged(AvaloniaPropertyChangedEventArgs<Control?> e)
    {
        var adorner = (Visual)e.Sender;
        var adorned = e.NewValue.GetValueOrDefault();
        var layer   = adorner.GetVisualParent<ScopeAwareAdornerLayer>();
        layer?.UpdateAdornedElement(adorner, adorned);
    }

    private static void HandleAdornerChanged(AvaloniaPropertyChangedEventArgs<Control?> e)
    {
        if (e.Sender is Visual visual)
        {
            var oldAdorner = e.OldValue.GetValueOrDefault();
            var newAdorner = e.NewValue.GetValueOrDefault();

            if (Equals(oldAdorner, newAdorner))
            {
                return;
            }

            if (oldAdorner is { })
            {
                visual.AttachedToVisualTree   -= VisualOnAttachedToVisualTree;
                visual.DetachedFromVisualTree -= VisualOnDetachedFromVisualTree;
                Detach(visual, oldAdorner);
            }

            if (newAdorner is { })
            {
                visual.AttachedToVisualTree   += VisualOnAttachedToVisualTree;
                visual.DetachedFromVisualTree += VisualOnDetachedFromVisualTree;
                Attach(visual, newAdorner);
            }
        }
    }

    private static void Attach(Visual visual, Control adorner)
    {
        var layer = ScopeAwareAdornerLayer.GetLayer(visual);
        AddVisualAdorner(visual, adorner, layer);
        visual.SetValue(SavedAdornerLayerProperty, layer);
    }

    private static void Detach(Visual visual, Control adorner)
    {
        var layer = visual.GetValue(SavedAdornerLayerProperty);
        RemoveVisualAdorner(visual, adorner, layer);
        visual.ClearValue(SavedAdornerLayerProperty);
    }

    private static void AddVisualAdorner(Visual visual, Control? adorner, ScopeAwareAdornerLayer? layer)
    {
        if (adorner is null || layer == null || layer.Children.Contains(adorner))
        {
            return;
        }

        SetAdornedElement(adorner, visual);

        adorner.SetLogicalParent(visual);
        layer.Children.Add(adorner);
    }

    private static void RemoveVisualAdorner(Visual visual, Control? adorner, ScopeAwareAdornerLayer? layer)
    {
        if (adorner is null || layer is null || !layer.Children.Contains(adorner))
        {
            return;
        }

        layer.Children.Remove(adorner);
        adorner.SetLogicalParent(null);
    }

    private static void VisualOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Visual visual)
        {
            var adorner = GetAdorner(visual);
            if (adorner is { })
            {
                Attach(visual, adorner);
            }
        }
    }

    private static void VisualOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Visual visual)
        {
            var adorner = GetAdorner(visual);
            if (adorner is { })
            {
                Detach(visual, adorner);
            }
        }
    }

    private static ScopeAwareAdornerLayer InjectLayer(Layoutable layerHost)
    {
        var layer = FindAdornerLayer(layerHost);
        if (layer != null)
        {
            return layer;
        }

        layer = new ScopeAwareAdornerLayer
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            ZIndex              = ScopeAwareAdornerLayerZIndex
        };

        layer.LayerHost = layerHost;
        if (layerHost is VisualLayerManager visualLayerManager)
        {
            visualLayerManager.AddLayer(layer, visualLayerManager.ZIndex);
        }
        else if (layerHost is ScrollContentPresenter scrollContentPresenter)
        {
            if (scrollContentPresenter.Content is Control controlContent)
            {
                var oldOffset = scrollContentPresenter.Offset;
                // 直接内容控件
                scrollContentPresenter.Content = null;
                scrollContentPresenter.UpdateChild();
                var panel = new Panel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment   = VerticalAlignment.Stretch,
                };
                panel.Children.Add(controlContent);
                panel.Children.Add(layer);
                scrollContentPresenter.Content = panel;
                var scrollViewer = scrollContentPresenter.FindAncestorOfType<ScrollViewer>();
                if (scrollViewer != null)
                {
                    scrollViewer.Offset = oldOffset;
                }
            }
            else if (scrollContentPresenter.Content != null && scrollContentPresenter.ContentTemplate != null)
            {
                // 模版处理
                var injectTemplate = new FuncDataTemplate<object?>((o, scope) =>
                {
                    var originControl = scrollContentPresenter.ContentTemplate.Build(o);
                    var panel = new Panel
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment   = VerticalAlignment.Stretch,
                    };
                    Debug.Assert(originControl != null);
                    panel.Children.Add(originControl);
                    panel.Children.Add(layer);
                    return panel;
                });
                scrollContentPresenter.ContentTemplate = injectTemplate;
            }
        }

        return layer;
    }

    private static ScopeAwareAdornerLayer? FindAdornerLayer(Layoutable layerHost)
    {
        if (layerHost is ScrollContentPresenter scrollContentPresenter)
        {
            // 在 Panel 下面
            var panel = scrollContentPresenter.FindChildOfType<Panel>();
            if (panel is not null)
            {
                return panel.FindChildOfType<ScopeAwareAdornerLayer>();
            }
        }
        else if (layerHost is VisualLayerManager visualLayerManager)
        {
            // 直接就在下面
            visualLayerManager.FindChildOfType<ScopeAwareAdornerLayer>();
        }

        return null;
    }

    private class AdornedElementInfo
    {
        public IDisposable? Subscription { get; set; }

        public TransformedBounds? Bounds { get; set; }
    }
}