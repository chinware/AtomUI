using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public class FlyoutPresenter : ArrowDecoratedBox
{
   // 我们在这里并没有增加任何元素或者样式
   protected override Type StyleKeyOverride => typeof(ArrowDecoratedBox);
   protected override void OnKeyDown(KeyEventArgs e)
   {
      if (e.Key == Key.Escape)
      {
         var host = this.FindLogicalAncestorOfType<Popup>();
         if (host != null)
         {
            host.IsOpen = false;
            e.Handled = true;
         }
      }
      base.OnKeyDown(e);
   }

   public FlyoutPresenter()
   {
      SetValue(CursorProperty, new Cursor(StandardCursorType.Arrow), BindingPriority.Template);
   }
}