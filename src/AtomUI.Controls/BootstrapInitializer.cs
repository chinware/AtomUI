using AtomUI.Utils;
using Avalonia;

namespace AtomUI.Controls;

public class BootstrapInitializer : IBootstrapInitializer
{
   public void Init()
   {
      AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService());
   }
}