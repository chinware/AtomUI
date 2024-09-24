using Avalonia;
using Avalonia.Animation;

namespace AtomUI.Controls.Utils;

public record MotionConfig
{
    public RelativePoint RenderTransformOrigin { get; }
    public IList<Animation> Animations { get; }

    public MotionConfig(RelativePoint renderTransformOrigin, IList<Animation> animations)
    {
        RenderTransformOrigin = renderTransformOrigin;
        Animations            = animations;
    }
}