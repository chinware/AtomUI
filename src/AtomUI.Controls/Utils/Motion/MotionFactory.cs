using Avalonia;
using Avalonia.Animation;

namespace AtomUI.Controls.Utils;

public record MotionConfig
{
    public MotionConfig(RelativePoint renderTransformOrigin, IList<IAnimation> animations)
    {
        RenderTransformOrigin = renderTransformOrigin;
        Animations            = animations;
    }

    public RelativePoint RenderTransformOrigin { get; }
    public IList<IAnimation> Animations { get; }
}