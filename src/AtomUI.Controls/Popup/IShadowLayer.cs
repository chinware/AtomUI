using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

public interface IShadowDecorator
{
   public BoxShadows MaskShadows { get; set; }
   public void AttachToTarget(AbstractPopup host);
   public void DetachedFromTarget(AbstractPopup host);
   public void ShowShadows();
   public void HideShadows();
}