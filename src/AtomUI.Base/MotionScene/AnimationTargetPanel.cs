using Avalonia;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

internal class AnimationTargetPanel : Panel
{
    private Size _cacheMeasureSize;
    public bool InAnimation { get; set; }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (InAnimation && _cacheMeasureSize != default)
        {
            return _cacheMeasureSize;
        }

        _cacheMeasureSize = base.MeasureOverride(availableSize);
        return _cacheMeasureSize;
    }
}