using Avalonia;
using Avalonia.Animation;

namespace AtomUI.Media;

public class RectTransition : InterpolatingTransitionBase<Rect>
{
    protected override Rect Interpolate(double progress, Rect from, Rect to)
    {
        var deltaPos  = to.Position - from.Position;
        var deltaSize = to.Size - from.Size;

        var newPos  = (deltaPos * progress) + from.Position;
        var newSize = (deltaSize * progress) + from.Size;

        return new Rect(newPos, newSize);
    }
}