using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

public abstract class AbstractPopup : AvaloniaPopup
{
   protected internal WeakReference<IPopupHost>? _popupHost;
   public AbstractPopup()
   {
      (this as IPopupHostProvider).PopupHostChanged += HandlePopupChanged;
   }

   private void HandlePopupChanged(IPopupHost? host)
   {
      if (host is null) {
         _popupHost = null;
         NotifyClosed();
      }
   }

   internal virtual void NotifyPopupHostPositionUpdated(IPopupHost popupHost, Control placementTarget)
   {
      if (_popupHost is null) {
         _popupHost = new WeakReference<IPopupHost>(popupHost);
         NotifyPopupHostCreated(popupHost);
      }
   }

   protected virtual void NotifyPopupHostCreated(IPopupHost popupHost) {}
   protected internal virtual void NotifyAboutToClosing() {}
   protected virtual void NotifyClosed() {}
}