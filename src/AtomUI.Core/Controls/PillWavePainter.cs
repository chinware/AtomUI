using AtomUI.Controls;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class PillWavePainter : AbstractWavePainter
{
    public Size OriginSize { get; set; }

    public PillWavePainter(Size size = default, Point originPoint = default)
        : base(originPoint)
    {
        OriginSize = size;
        WaveType   = WaveSpiritType.PillWave;
    }

    public override void Paint(DrawingContext context, object newSize, double newOpacity)
    {
        var newSizeTyped = (Size)newSize;
        if (newSize is null)
        {
            throw new ArgumentException("newSize argument must be Size type.");
        }
        using var state      = context.PushOpacity(newOpacity);
        var       pillRadius = OriginSize.Height / 2;
        var originGeometry = new RectangleGeometry(
            new Rect(OriginPoint.X, OriginPoint.Y, OriginSize.Width, OriginSize.Height),
            pillRadius, pillRadius);
        var deltaSize     = newSizeTyped - OriginSize;
        var newPoint      = OriginPoint - new Point(deltaSize.Width / 2, deltaSize.Height / 2);
        var newPillRadius = newSizeTyped.Height / 2;
        var newGeometry = new RectangleGeometry(
            new Rect(newPoint.X, newPoint.Y, newSizeTyped.Width, newSizeTyped.Height),
            newPillRadius, newPillRadius);
        var targetGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, newGeometry, originGeometry);
        var geometryDrawing = new GeometryDrawing
        {
            Brush    = WaveBrush,
            Geometry = targetGeometry
        };
        geometryDrawing.Draw(context);
    }

    public override RoundRectWavePainter Clone()
    {
        return new RoundRectWavePainter(OriginSize, OriginPoint);
    }

    public override void NotifyBuildSizeAnimation(Animation animation, AvaloniaProperty targetProperty)
    {
        animation.Duration = SizeMotionDuration;
        animation.Easing   = SizeEasingCurve ?? new LinearEasing();
        animation.Children.Add(new KeyFrame
        {
            Setters =
            {
                new Setter
                {
                    Property = targetProperty,
                    Value    = OriginSize
                }
            },
            KeyTime = TimeSpan.FromMilliseconds(0)
        });
        var endValue = OriginSize + new Size(WaveRange * 2, WaveRange * 2);
        animation.Children.Add(new KeyFrame
        {
            Setters =
            {
                new Setter
                {
                    Property = targetProperty,
                    Value    = endValue
                }
            },
            KeyTime = SizeMotionDuration
        });
    }
}