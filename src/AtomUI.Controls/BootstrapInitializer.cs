using AtomUI.Controls.Interceptors;
using AtomUI.Utils;
using HarmonyLib;

namespace AtomUI.Controls;

public class BootstrapInitializer : IBootstrapInitializer
{
   public void Init()
   {
      SimpleServiceLocator.CurrentMutable.BindToSelf(new ToolTipService());
      InitInterceptors();
   }

   private void InitInterceptors()
   {
      Harmony harmony = new Harmony("net.atomui.controls");
      PopupFlyoutBaseInterceptorRegister.Register(harmony);
   }
}