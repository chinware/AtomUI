using Avalonia;

namespace AtomUI.Controls.Utils;

internal interface IWaveAdornerInfoProvider
{
   public Rect WaveGeometry();
   public CornerRadius WaveBorderRadius();
}