using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class PopupShadowDecorator : AvaloniaObject
{
   private Popup _popup;
   
   public PopupShadowDecorator(Popup popup)
   {
      _popup = popup;
   }

   public virtual void Open()
   {
      
   }

   public virtual void Close()
   {
      
   }
}