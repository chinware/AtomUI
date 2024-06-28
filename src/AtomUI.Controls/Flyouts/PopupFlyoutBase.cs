using AtomUI.Data;
using Avalonia.Controls;


namespace AtomUI.Controls;

using AvaloniaPopupFlyoutBase = Avalonia.Controls.Primitives.PopupFlyoutBase;

public abstract class PopupFlyoutBase : AvaloniaPopupFlyoutBase
{
   private GlobalTokenBinder _globalTokenBinder;

   public PopupFlyoutBase()
   {
      _globalTokenBinder = new GlobalTokenBinder();
   }
   
   internal static void SetPresenterClasses(Control? presenter, Classes classes)
   {
      if (presenter is null) {
         return;
      }

      //Remove any classes no longer in use, ignoring pseudo classes
      for (int i = presenter.Classes.Count - 1; i >= 0; i--) {
         if (!classes.Contains(presenter.Classes[i]) &&
             !presenter.Classes[i].Contains(':')) {
            presenter.Classes.RemoveAt(i);
         }
      }

      //Add new classes
      presenter.Classes.AddRange(classes);
   }
}