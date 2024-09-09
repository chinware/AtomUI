using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls.Utils;

internal enum WaveType
{
    RoundRectWave,
    CircleWave,
    PillWave
}


internal abstract class AbstractWavePainter : IWavePainter
{
    public AbstractWavePainter(Point originPoint)
    {
        WaveType  = WaveType.RoundRectWave;
        WaveRange = 3;
    }

    public Point OriginPoint { get; set; }
    public Color WaveColor { get; set; }
    public double WaveRange { get; set; }
    public TimeSpan SizeMotionDuration { get; set; }
    public TimeSpan OpacityMotionDuration { get; set; }
    public Easing SizeEasingCurve { get; set; } = default!;
    public Easing OpacityEasingCurve { get; set; } = default!;
    public double OriginOpacity { get; set; }
    public WaveType WaveType { get; protected set; }

    public abstract void Paint(DrawingContext context, object newSize, double newOpacity);
    public abstract AbstractWavePainter Clone();
    public abstract void NotifyBuildSizeAnimation(Animation animation, AvaloniaProperty targetProperty);

    public virtual void NotifyBuildOpacityAnimation(Animation animation, AvaloniaProperty targetProperty)
    {
        animation.Duration = OpacityMotionDuration;
        animation.Easing   = OpacityEasingCurve;
        animation.Children.Add(new KeyFrame
        {
            Setters =
            {
                new Setter
                {
                    Property = targetProperty,
                    Value    = OriginOpacity
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
                    Value    = 0d
                }
            },
            KeyTime = OpacityMotionDuration
        });
    }
}