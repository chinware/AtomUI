using Avalonia.Input;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public class FlyoutPresenter : ArrowDecoratedBox
{ 
   string TokenId => FlyoutPresenterToken.ID;
   
   protected override void OnKeyDown(KeyEventArgs e)
   {
      if (e.Key == Key.Escape)
      {
         var host = this.FindLogicalAncestorOfType<AbstractPopup>();
         if (host != null)
         {
            host.IsOpen = false;
            e.Handled = true;
         }
      }
      base.OnKeyDown(e);
   }
}