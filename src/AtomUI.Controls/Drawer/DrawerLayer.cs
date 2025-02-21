using System.Collections.Specialized;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class DrawerLayer : Canvas
{
    private const int DrawerLayerZIndex = int.MaxValue - 1000;

    #region 公共属性

    public static readonly AttachedProperty<Control?> AttachTargetElementProperty =
        AvaloniaProperty.RegisterAttached<DrawerLayer, Visual, Control?>("AttachTargetElement");

    public static readonly AttachedProperty<bool> IsClipEnabledProperty =
        AvaloniaProperty.RegisterAttached<DrawerLayer, Visual, bool>("IsClipEnabled", true);

    public static readonly AttachedProperty<Control?> DrawerContainerProperty =
        AvaloniaProperty.RegisterAttached<DrawerLayer, Visual, Control?>("DrawerContainer");

    #endregion

    #region 内部属性定义

    private static readonly AttachedProperty<AttachTargetElementInfo?> AttachTargetElementInfoProperty =
        AvaloniaProperty.RegisterAttached<DrawerLayer, Visual, AttachTargetElementInfo?>("AttachTargetElementInfo");

    private static readonly AttachedProperty<DrawerLayer?> SavedDrawerLayerProperty =
        AvaloniaProperty.RegisterAttached<Visual, Visual, DrawerLayer?>("SavedDrawerLayer");

    private static readonly MethodInfo AddLayerMethodInfo;

    #endregion

    static DrawerLayer()
    {
        AttachTargetElementProperty.Changed.Subscribe(HandleAttachTargetElementChanged);
        DrawerContainerProperty.Changed.Subscribe(HandleDrawerContainerChanged);
        AddLayerMethodInfo =
            typeof(VisualLayerManager).GetMethod("AddLayer", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    public DrawerLayer()
    {
        Children.CollectionChanged += ChildrenCollectionChanged;
    }

    public static Visual? GetAttachTargetElement(Visual drawerContainer)
    {
        return drawerContainer.GetValue(AttachTargetElementProperty);
    }

    public static void SetAttachTargetElement(Visual drawerContainer, Visual? attachTarget)
    {
        drawerContainer.SetValue(AttachTargetElementProperty, attachTarget);
    }

    public static DrawerLayer GetDrawerLayer(Visual visual)
    {
        var layer = visual.FindAncestorOfType<VisualLayerManager>()?.GetVisualChildren()
                          .FirstOrDefault(c => c is DrawerLayer) as DrawerLayer;
        if (layer == null)
        {
            layer = new DrawerLayer();
            AddLayerToVisualLayerManager(visual, layer, DrawerLayerZIndex);
        }

        return layer;
    }

    public static bool GetIsClipEnabled(Visual drawerContainer)
    {
        return drawerContainer.GetValue(IsClipEnabledProperty);
    }

    public static void SetIsClipEnabled(Visual drawerContainer, bool isClipEnabled)
    {
        drawerContainer.SetValue(IsClipEnabledProperty, isClipEnabled);
    }

    public static Control? GetDrawerContainer(Visual visual)
    {
        return visual.GetValue(DrawerContainerProperty);
    }

    public static void SetDrawerContainer(Visual visual, Control? drawerContainer)
    {
        visual.SetValue(DrawerContainerProperty, drawerContainer);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            if (child is AvaloniaObject ao)
            {
                var info = ao.GetValue(AttachTargetElementInfoProperty);

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
                var info          = ao.GetValue(AttachTargetElementInfoProperty);
                var isClipEnabled = ao.GetValue(IsClipEnabledProperty);

                if (info != null && info.Bounds.HasValue)
                {
                    child.RenderTransform       = new MatrixTransform(info.Bounds.Value.Transform);
                    child.RenderTransformOrigin = new RelativePoint(new Point(0, 0), RelativeUnit.Absolute);
                    UpdateClip(child, info.Bounds.Value, isClipEnabled);
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
                    UpdateAttachTargetElement(i, i.GetValue(AttachTargetElementProperty));
                }

                break;
        }

        InvalidateArrange();
    }

    private void UpdateAttachTargetElement(Visual drawerContainer, Control? attachTarget)
    {
        var info = drawerContainer.GetValue(AttachTargetElementInfoProperty);

        if (info != null)
        {
            info.Subscription!.Dispose();

            if (attachTarget == null)
            {
                drawerContainer.ClearValue(AttachTargetElementInfoProperty);
            }
        }

        if (attachTarget != null)
        {
            if (info == null)
            {
                info = new AttachTargetElementInfo();
                drawerContainer.SetValue(AttachTargetElementInfoProperty, info);
            }
            info.Subscription = attachTarget.GetObservable(BoundsProperty).Subscribe(x =>
            {
                var attachTargetToplevel = TopLevel.GetTopLevel(attachTarget);
                var translationMatrix = attachTargetToplevel != null
                    ? attachTarget.TransformToVisual(attachTargetToplevel) ?? Matrix.Identity
                    : Matrix.Identity;
                info.Bounds = new TransformedBounds(new Rect(attachTarget.DesiredSize), new Rect(attachTarget.DesiredSize),
                    translationMatrix);
                InvalidateMeasure();
            });
        }
    }

    private void UpdateClip(Control control, TransformedBounds bounds, bool isEnabled)
    {
        if (!isEnabled)
        {
            control.Clip = null;

            return;
        }

        if (!(control.Clip is RectangleGeometry clip))
        {
            clip         = new RectangleGeometry();
            control.Clip = clip;
        }

        var clipBounds = bounds.Bounds;
        
        clip.Rect = clipBounds;
    }

    private static void HandleAttachTargetElementChanged(AvaloniaPropertyChangedEventArgs<Control?> e)
    {
        var drawerContainer = (Visual)e.Sender;
        var attachTarget    = e.NewValue.GetValueOrDefault();
        var layer           = drawerContainer.GetVisualParent<DrawerLayer>();
        layer?.UpdateAttachTargetElement(drawerContainer, attachTarget);
    }

    private static void HandleDrawerContainerChanged(AvaloniaPropertyChangedEventArgs<Control?> e)
    {
        if (e.Sender is Visual visual)
        {
            var oldDrawerContainer = e.OldValue.GetValueOrDefault();
            var newDrawerContainer = e.NewValue.GetValueOrDefault();

            if (Equals(oldDrawerContainer, newDrawerContainer))
            {
                return;
            }

            if (oldDrawerContainer is { })
            {
                visual.AttachedToVisualTree   -= VisualOnAttachedToVisualTree;
                visual.DetachedFromVisualTree -= VisualOnDetachedFromVisualTree;
                Detach(visual, oldDrawerContainer);
            }

            if (newDrawerContainer is { })
            {
                visual.AttachedToVisualTree   += VisualOnAttachedToVisualTree;
                visual.DetachedFromVisualTree += VisualOnDetachedFromVisualTree;
                Attach(visual, newDrawerContainer);
            }
        }
    }

    private static void Attach(Visual visual, Control drawerContainer)
    {
        var layer = DrawerLayer.GetDrawerLayer(visual);
        AddVisualDrawerContainer(visual, drawerContainer, layer);
        visual.SetValue(SavedDrawerLayerProperty, layer);
    }

    private static void Detach(Visual visual, Control drawerContainer)
    {
        var layer = visual.GetValue(SavedDrawerLayerProperty);
        RemoveVisualDrawerContainer(visual, drawerContainer, layer);
        visual.ClearValue(SavedDrawerLayerProperty);
    }

    private static void AddVisualDrawerContainer(Visual visual, Control? drawerContainer, DrawerLayer? layer)
    {
        if (drawerContainer is null || layer == null || layer.Children.Contains(drawerContainer))
        {
            return;
        }

        SetAttachTargetElement(drawerContainer, visual);

        ((ISetLogicalParent)drawerContainer).SetParent(visual);
        layer.Children.Add(drawerContainer);
    }

    private static void RemoveVisualDrawerContainer(Visual visual, Control? drawerContainer, DrawerLayer? layer)
    {
        if (drawerContainer is null || layer is null || !layer.Children.Contains(drawerContainer))
        {
            return;
        }

        layer.Children.Remove(drawerContainer);
        ((ISetLogicalParent)drawerContainer).SetParent(null);
    }

    private static void VisualOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Visual visual)
        {
            var drawerContainer = GetDrawerContainer(visual);
            if (drawerContainer is { })
            {
                Attach(visual, drawerContainer);
            }
        }
    }

    private static void VisualOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Visual visual)
        {
            var drawerContainer = GetDrawerContainer(visual);
            if (drawerContainer is { })
            {
                Detach(visual, drawerContainer);
            }
        }
    }

    private static void AddLayerToVisualLayerManager(Visual visual, Control layer, int zindex)
    {
        var visualLayerManager = visual.FindAncestorOfType<VisualLayerManager>();
        if (visualLayerManager != null)
        {
            AddLayerMethodInfo.Invoke(visualLayerManager, [layer, zindex]);
        }
    }

    private class AttachTargetElementInfo
    {
        public IDisposable? Subscription { get; set; }

        public TransformedBounds? Bounds { get; set; }
    }
}