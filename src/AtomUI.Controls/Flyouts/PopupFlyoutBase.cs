using AtomUI.Data;
using AtomUI.Styling;
using Avalonia.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

namespace AtomUI.Controls;

using AvaloniaPopupFlyoutBase = Avalonia.Controls.Primitives.PopupFlyoutBase;

public abstract class PopupFlyoutBase : AvaloniaPopupFlyoutBase
{
   private PopupShadowDecorator? _popupDecorator;
   private GlobalTokenBinder _globalTokenBinder;

   public PopupFlyoutBase()
   {
      _globalTokenBinder = new GlobalTokenBinder();
   }
   
   protected override bool ShowAtCore(Control placementTarget, bool showAtPointer = false)
   {
      if (_popupDecorator is null) {
         // Popup.PlacementTarget = placementTarget;
         // _popupDecorator = new PopupShadowDecorator(Popup);
      
        // _globalTokenBinder.AddGlobalBinding(_popupDecorator, PopupShadowDecorator.MaskShadowsProperty, GlobalResourceKey.BoxShadowsSecondary);
      }

      return base.ShowAtCore(placementTarget, showAtPointer);
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