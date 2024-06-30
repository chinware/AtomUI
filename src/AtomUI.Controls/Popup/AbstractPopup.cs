using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace AtomUI.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

public abstract class AbstractPopup : AvaloniaPopup
{
   public event EventHandler<EventArgs>? PopupHostCreated;
   public event EventHandler<EventArgs>? AboutToClosing;
   private static readonly FieldInfo ManagedPopupPositionerPopupInfo;
   protected IManagedPopupPositionerPopup? _managedPopupPositioner; // 在弹窗有效期获取
   
   protected internal WeakReference<IPopupHost>? _popupHost;
   
   static AbstractPopup()
   {
      ManagedPopupPositionerPopupInfo = typeof(ManagedPopupPositioner).GetField("_popup",
         BindingFlags.Instance | BindingFlags.NonPublic)!;
   }
   
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

   protected internal virtual void NotifyHostPositionUpdated(IPopupHost popupHost, Control placementTarget)
   {
      if (_popupHost is null) {
         _popupHost = new WeakReference<IPopupHost>(popupHost);
         NotifyPopupHostCreated(popupHost);
      }
   }
   
   // 开始定位 Host 窗口
   protected internal virtual void NotifyAboutToUpdateHostPosition(IPopupHost popupHost, Control placementTarget)
   {
      if (popupHost is PopupRoot popupRoot) {
         if (popupRoot.PlatformImpl?.PopupPositioner is ManagedPopupPositioner managedPopupPositioner) {
            _managedPopupPositioner =
               ManagedPopupPositionerPopupInfo.GetValue(managedPopupPositioner) as IManagedPopupPositionerPopup;
         }
      }
   }

   protected virtual void NotifyPopupHostCreated(IPopupHost popupHost)
   {
      PopupHostCreated?.Invoke(this, new PopupHostCreatedEventArgs(popupHost));
   }

   protected internal virtual void NotifyAboutToClosing()
   {
      AboutToClosing?.Invoke(this, EventArgs.Empty);
   }
   
   protected virtual void NotifyClosed() {}
}

public class PopupHostCreatedEventArgs : EventArgs
{
   public IPopupHost PopupHost { get; }
   
   public PopupHostCreatedEventArgs(IPopupHost host)
   {
      PopupHost = host;
   }
}