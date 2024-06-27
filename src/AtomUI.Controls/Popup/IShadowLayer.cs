using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

public interface IShadowLayer
{
   public void AttachToTarget(Control control);
   public void SetShadowMaskGeometry(Geometry geometry);
   public void SetShadows(BoxShadows shadows);

   public void ShowShadows();
   public void HideShadows();
}