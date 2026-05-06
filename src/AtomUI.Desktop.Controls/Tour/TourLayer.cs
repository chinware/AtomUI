using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class TourLayer : Control
{
    private const int TourLayerZIndex = int.MaxValue - 98;
    
    #region 公共属性定义
    public static readonly StyledProperty<Rect> TargetRegionProperty = 
        AvaloniaProperty.Register<TourLayer, Rect>(nameof(TargetRegion));
    
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<TourLayer, IBrush?>(nameof(Background));
    
    public static readonly StyledProperty<double> TargetRegionCornerRadiusProperty =
        AvaloniaProperty.Register<TourLayer, double>(nameof(TargetRegionCornerRadius));
    
    public Rect TargetRegion
    {
        get => GetValue(TargetRegionProperty);
        set => SetValue(TargetRegionProperty, value);
    }
    
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }
    
    public double TargetRegionCornerRadius
    {
        get => GetValue(TargetRegionCornerRadiusProperty);
        set => SetValue(TargetRegionCornerRadiusProperty, value);
    }
    #endregion

    static TourLayer()
    {
        AffectsRender<TourLayer>(BackgroundProperty, TargetRegionProperty, TargetRegionCornerRadiusProperty);
    }

    public sealed override void Render(DrawingContext context)
    {
        var combinedGeometry = new CombinedGeometry
        {
            GeometryCombineMode = GeometryCombineMode.Exclude,
            Geometry1           = new RectangleGeometry(new Rect(Bounds.Size)),
            Geometry2           = new RectangleGeometry(TargetRegion, TargetRegionCornerRadius, TargetRegionCornerRadius),
        };
        
        context.DrawGeometry(Background, null, combinedGeometry);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        e.Handled = true;
    }
    
    public static TourLayer? GetTourLayer(Visual visual)
    {
        VisualLayerManager? manager;
        if (visual is TopLevel topLevel)
        {
            manager = topLevel.GetTemplateDescendants()
                              .OfType<VisualLayerManager>()
                              .FirstOrDefault();
        }
        else
        {
            manager = visual.FindAncestorOfType<VisualLayerManager>();
        }

        if (manager == null)
        {
            return null;
        }

        var tourLayer = VisualLayerManagerUtils.FindLayer<TourLayer>(manager);
        if (tourLayer == null)
        {
            tourLayer = new TourLayer()
            {
                IsVisible = false
            };

            manager.AddLayer(tourLayer, TourLayerZIndex);
        }
        return tourLayer;
    }
}