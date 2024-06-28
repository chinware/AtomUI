using Avalonia.Media;

namespace AtomUI.Controls;

public interface IShadowDecorator
{
   public BoxShadows MaskShadows { get; set; }
   public void AttachToTarget(Popup host);
   public void DetachedFromTarget(Popup host);
   public void ShowShadows();
   public void HideShadows();
}