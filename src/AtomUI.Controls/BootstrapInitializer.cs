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
      var harmony = AvaloniaLocator.Current.GetService<Harmony>()!;
      PopupInterceptorsRegister.Register(harmony);
   }
}