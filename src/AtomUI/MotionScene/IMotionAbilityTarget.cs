using Avalonia;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public interface IMotionAbilityTarget
{
   public void NotifyMotionPreStart();
   public void NotifyMotionStarted();
   public void NotifyMotionFinished();

   public Point MotionOriginPoint { get; set; }
   public Rect MotionOriginBounds { get; set; }
   public Rect MotionRequestBounds { get; set; }

   public bool IsSupportMotionProperty(AvaloniaProperty property);
   public bool IsTopLevel(); // 如果不是 TopLevel 就不用那么复杂，直接上就可以了
   public Control? BuildMotionGhost(); // 在对顶层控件运行动效的时候需要创建影子控件
}