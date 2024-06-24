using AtomUI.Utils;

namespace AtomUI.Controls;

public class BootstrapInitializer : IBootstrapInitializer
{
   public void Init()
   {
      SimpleServiceLocator.CurrentMutable.BindToSelf(new ToolTipService());
   }
}