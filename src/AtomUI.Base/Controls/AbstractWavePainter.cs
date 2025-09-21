using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class AbstractWavePainter : IWaveSpiritPainter
{
    public Point OriginPoint { get; set; }
    public IBrush? WaveBrush { get; set; }
    public WaveSpiritType WaveType { get; protected set; }
    public double WaveRange { get; set; }
    public TimeSpan SizeMotionDuration { get; set; }
    public TimeSpan OpacityMotionDuration { get; set; }
    public Easing? SizeEasingCurve { get; set; }
    public Easing? OpacityEasingCurve { get; set; }
    public double OriginOpacity { get; set; }

    public AbstractWavePainter(Point originPoint)
    {
        WaveType  = WaveSpiritType.RoundRectWave;
        WaveRange = 3;
        OriginPoint = originPoint;
    }

    public abstract void Paint(DrawingContext context, object newSize, double newOpacity);
    public abstract AbstractWavePainter Clone();
    public abstract void NotifyBuildSizeAnimation(Animation animation, AvaloniaProperty targetProperty);

    public virtual void NotifyBuildOpacityAnimation(Animation animation, AvaloniaProperty targetProperty)
    {
        animation.Duration = OpacityMotionDuration;
        animation.Easing   = OpacityEasingCurve ?? new LinearEasing();
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