using Avalonia;

namespace AtomUI.Media;

public interface IMotionAbilityTarget
{
   public void NotifyMotionPreStart();
   public void NotifyMotionStarted();
   public void NotifyMotionFinished();

   public Point MotionOriginPoint { get; set; }
   public Rect MotionOriginBounds { get; set; }
   public Rect MotionRequestBounds { get; set; }

   public bool IsSupportMotionProperty(AvaloniaProperty property);
}