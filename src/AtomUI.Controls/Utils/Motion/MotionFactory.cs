using Avalonia;
using Avalonia.Animation;

namespace AtomUI.Controls.Utils;

public record MotionConfig
{
   public RelativePoint RenderTransformOrigin { get; }
   public IList<IAnimation> Animations { get; }

   public MotionConfig(RelativePoint renderTransformOrigin, IList<IAnimation> animations)
   {
      RenderTransformOrigin = renderTransformOrigin;
      Animations = animations;
   }
}

