using Avalonia.Controls;

namespace AtomUI.Controls;

public class CardTabStrip : BaseTabStrip
{
   protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
   {
      return new TabStripItem()
      {
         Shape = TabSharp.Card
      };
   }
}