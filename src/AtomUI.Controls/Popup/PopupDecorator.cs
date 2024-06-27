using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class PopupDecorator : AvaloniaObject
{
   private Popup _popup;
   
   public PopupDecorator(Popup popup)
   {
      _popup = popup;
   }
}