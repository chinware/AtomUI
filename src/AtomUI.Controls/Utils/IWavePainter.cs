using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;

namespace AtomUI.Controls.Utils;

internal interface IWavePainter
{
    public void Paint(DrawingContext context, object size, double opacity);
    public AbstractWavePainter Clone();
    public WaveType WaveType { get; }
    public void NotifyBuildSizeAnimation(Animation animation, AvaloniaProperty targetProperty);
    public void NotifyBuildOpacityAnimation(Animation animation, AvaloniaProperty targetProperty);
}