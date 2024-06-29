using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using HarmonyLib;

namespace AtomUI.Controls.Interceptors;

using AvaloniaPopupFlyoutBase = Avalonia.Controls.Primitives.PopupFlyoutBase;
using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

internal static class PopupFlyoutBaseInterceptor
{
   private static readonly EventInfo OpenedEventInfo;
   private static readonly EventInfo ClosingEventInfo;
   private static readonly EventInfo ClosedEventInfo;
   private static readonly EventInfo KeyUpEventInfo;
   private static readonly MethodInfo OnPopupOpenedMemberInfo;
   private static readonly MethodInfo OnPopupClosedMemberInfo;
   private static readonly MethodInfo OnPopupClosingMethodInfo;
   private static readonly MethodInfo OnPlacementTargetOrPopupKeyUpMethodInfo;
   
   static PopupFlyoutBaseInterceptor()
   {
      var popupType = typeof(AvaloniaPopup);
      var popupFlyoutBaseType = typeof(AvaloniaPopupFlyoutBase);
      OpenedEventInfo = popupType.GetEvent("Opened")!;
      ClosingEventInfo = popupType.GetEvent("Closing", BindingFlags.NonPublic | BindingFlags.Instance)!;
      ClosedEventInfo = popupType.GetEvent("Closed")!;
      KeyUpEventInfo = popupType.GetEvent("KeyUp")!;
      OnPopupOpenedMemberInfo = popupFlyoutBaseType.GetMethod("OnPopupOpened", BindingFlags.Instance | BindingFlags.NonPublic)!;
      OnPopupClosingMethodInfo = popupFlyoutBaseType.GetMethod("OnPopupClosing", BindingFlags.Instance | BindingFlags.NonPublic)!;
      OnPopupClosedMemberInfo = popupFlyoutBaseType.GetMethod("OnPopupClosed", BindingFlags.Instance | BindingFlags.NonPublic)!;
      OnPlacementTargetOrPopupKeyUpMethodInfo = popupFlyoutBaseType.GetMethod("OnPlacementTargetOrPopupKeyUp", BindingFlags.Instance | BindingFlags.NonPublic)!;
   }
   
   public static bool CreatePopupPrefixInterceptor(PopupFlyoutBase __instance, ref AvaloniaPopup __result)
   {
      var popup = new Popup
      {
         WindowManagerAddShadowHint = false,
         IsLightDismissEnabled = false,
      };
      OpenedEventInfo.AddEventHandler(popup, Delegate.CreateDelegate(typeof(EventHandler), __instance, OnPopupOpenedMemberInfo));
      EventHandler<CancelEventArgs> closingDelegate = (object? sender, CancelEventArgs e) =>
      {
         OnPopupClosingMethodInfo.Invoke(__instance, new object?[] { sender, e });
         if (!e.Cancel) {
            if (popup._popupHost is not null) {
               popup.NotifyAboutToClosing();
            }
         }
      };
      ClosingEventInfo.AddMethod!.Invoke(popup, new object?[]
      {
         closingDelegate
      });
      ClosedEventInfo.AddEventHandler(popup, Delegate.CreateDelegate(typeof(EventHandler<EventArgs>), __instance, OnPopupClosedMemberInfo));
      KeyUpEventInfo.AddEventHandler(popup, Delegate.CreateDelegate(typeof(EventHandler<KeyEventArgs>), __instance, OnPlacementTargetOrPopupKeyUpMethodInfo));
      __result = popup;
      __instance.NotifyPopupCreated(popup);
      return false;
   }

   public static void UpdateHostPositionInterceptor(AbstractPopup __instance, IPopupHost popupHost, Control placementTarget)
   {
      __instance.NotifyPopupHostPositionUpdated(popupHost, placementTarget);
   }

   public static bool PositionPopupInterceptor(PopupFlyoutBase __instance, bool showAtPointer)
   {
      __instance.NotifyPositionPopup(showAtPointer);
      return false;
   }
}

internal static class PopupFlyoutBaseInterceptorRegister
{
   public static void Register(Harmony harmony)
   {
      RegisterPopupFlyoutBaseCreatePopup(harmony);
      RegisterPopupUpdateHostPosition(harmony);
      RegisterPopupPositionPopup(harmony);
   }
   
   private static void RegisterPopupFlyoutBaseCreatePopup(Harmony harmony)
   {
      var origin = typeof(AvaloniaPopupFlyoutBase).GetMethod("CreatePopup", BindingFlags.Instance | BindingFlags.NonPublic);
      var prefixInterceptor = typeof(PopupFlyoutBaseInterceptor)
         .GetMethod(nameof(PopupFlyoutBaseInterceptor.CreatePopupPrefixInterceptor),
                    BindingFlags.Static | BindingFlags.Public);
      harmony.Patch(origin, prefix: new HarmonyMethod(prefixInterceptor));
   }

   private static void RegisterPopupUpdateHostPosition(Harmony harmony)
   {
      var origin = typeof(AvaloniaPopup).GetMethod("UpdateHostPosition", BindingFlags.Instance | BindingFlags.NonPublic);
      var postfixInterceptor = typeof(PopupFlyoutBaseInterceptor)
         .GetMethod(nameof(PopupFlyoutBaseInterceptor.UpdateHostPositionInterceptor),
                    BindingFlags.Static | BindingFlags.Public);
      harmony.Patch(origin, postfix: new HarmonyMethod(postfixInterceptor));
   }

   private static void RegisterPopupPositionPopup(Harmony harmony)
   {
      var origin = typeof(AvaloniaPopupFlyoutBase).GetMethod("PositionPopup", BindingFlags.Instance | BindingFlags.NonPublic);
      var prefixInterceptor = typeof(PopupFlyoutBaseInterceptor)
         .GetMethod(nameof(PopupFlyoutBaseInterceptor.PositionPopupInterceptor),
                    BindingFlags.Static | BindingFlags.Public);
      harmony.Patch(origin, prefix: new HarmonyMethod(prefixInterceptor));
   }
}