using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;

namespace AtomUI.Controls;

internal interface IWaveSpiritPainter
{
    public void Paint(DrawingContext context, object size, double opacity);
    public AbstractWavePainter Clone();
    public WaveSpiritType WaveType { get; }
    public void NotifyBuildSizeAnimation(Animation animation, AvaloniaProperty targetProperty);
    public void NotifyBuildOpacityAnimation(Animation animation, AvaloniaProperty targetProperty);
}