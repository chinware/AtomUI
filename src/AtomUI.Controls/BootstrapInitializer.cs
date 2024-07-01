using AtomUI.Controls.Interceptors;
using AtomUI.Utils;
using Avalonia;
using HarmonyLib;

namespace AtomUI.Controls;

public class BootstrapInitializer : IBootstrapInitializer
{
   public void Init()
   {
      AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService());
      InitInterceptors();
   }

   private void InitInterceptors()
   {
      Harmony harmony = new Harmony("net.atomui.controls");
      PopupInterceptorsRegister.Register(harmony);
   }
}