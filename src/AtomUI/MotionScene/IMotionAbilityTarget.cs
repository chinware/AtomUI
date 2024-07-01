using Avalonia;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public interface IMotionAbilityTarget
{
   public void NotifyMotionPreStart();
   public void NotifyMotionStarted();
   public void NotifyMotionFinished();

   public Point OriginPoint { get; set; }
   public Rect OriginBounds { get; set; }
   public Rect RequestBounds { get; set; }

   public bool IsSupportMotionProperty(AvaloniaProperty property);
   public bool IsTopLevel(); // 如果不是 TopLevel 就不用那么复杂，直接上就可以了
   public Control? BuildMotionGhost(); // 在对顶层控件运行动效的时候需要创建影子控件
   
   public MotionActor BuildMotionActor();
}