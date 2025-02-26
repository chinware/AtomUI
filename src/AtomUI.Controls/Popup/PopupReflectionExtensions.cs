using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace AtomUI.Controls;

internal static class PopupReflectionExtensions
{
    public static void MoveAndResize(this Popup popup, Point devicePoint, Size virtualSize)
    {
        if (popup.Host is PopupRoot popupRoot)
        {
            if (popupRoot.PlatformImpl?.PopupPositioner is ManagedPopupPositioner managedPopupPositioner)
            {
                var managedPopupPositionerPopup = managedPopupPositioner.GetManagedPopupPositionerPopup();
                managedPopupPositionerPopup.MoveAndResize(devicePoint, virtualSize);
            }
        }
    }
}