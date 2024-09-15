using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls.Utils;

internal class CircleWavePainter : AbstractWavePainter
{
    public double OriginRadius;

    public CircleWavePainter(double radius = default, Point originPoint = default)
        : base(originPoint)
    {
        OriginRadius = radius;
        WaveType     = WaveType.CircleWave;
    }

    public override void Paint(DrawingContext context, object newSize, double newOpacity)
    {
        if (newSize is not double)
        {
            throw new ArgumentException("newSize argument must be double type.");
        }

        var newRadius = (double)newSize;

        var originGeometry = new EllipseGeometry
        {
            Center  = OriginPoint,
            RadiusX = OriginRadius,
            RadiusY = OriginRadius
        };

        var newGeometry = new EllipseGeometry
        {
            Center  = OriginPoint,
            RadiusX = newRadius,
            RadiusY = newRadius
        };
        var targetGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, newGeometry, originGeometry);
        var geometryDrawing = new GeometryDrawing
        {
            Brush    = new SolidColorBrush(WaveColor, newOpacity),
            Geometry = targetGeometry
        };
        geometryDrawing.Draw(context);
    }

    public override CircleWavePainter Clone()
    {
        return new CircleWavePainter(OriginRadius, OriginPoint);
    }

    public override void NotifyBuildSizeAnimation(Animation animation, AvaloniaProperty targetProperty)
    {
        animation.Duration = SizeMotionDuration;
        animation.Easing   = SizeEasingCurve;
        animation.Children.Add(new KeyFrame
        {
            Setters =
            {
                new Setter
                {
                    Property = targetProperty,
                    Value    = OriginRadius
                }
            },
            KeyTime = TimeSpan.FromMilliseconds(0)
        });
        animation.Children.Add(new KeyFrame
        {
            Setters =
            {
                new Setter
                {
                    Property = targetProperty,
                    Value    = OriginRadius + WaveRange
                }
            },
            KeyTime = SizeMotionDuration
        });
    }
}