using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class TabStripOverflowMenuItem : BaseOverflowMenuItem
{
   protected override Type StyleKeyOverride => typeof(BaseOverflowMenuItem);
   public TabStripItem? TabStripItem { get; set; }

   protected override void NotifyCloseRequest()
   {
      if (Parent is MenuBase menu) {
         var eventArgs = new CloseTabRequestEventArgs(CloseTabEvent, TabStripItem!);
         RaiseEvent(eventArgs);
         Dispatcher.UIThread.Post(() =>
         {
            menu.Close();
         });
      }
   }
}
