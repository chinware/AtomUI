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

   protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
   {
      base.PrepareContainerForItemOverride(container, item, index);
      if (container is TabStripItem tabStripItem) {
         tabStripItem.Shape = TabSharp.Card;
      }
   }
}