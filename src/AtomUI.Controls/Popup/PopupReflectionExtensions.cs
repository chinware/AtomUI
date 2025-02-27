using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace AtomUI.Controls;

internal static class PopupReflectionExtensions
{
    #region 反射信息定义
    private static readonly Lazy<MethodInfo> SetPopupParentMethodInfo = new Lazy<MethodInfo>(
        typeof(Popup).GetMethodInfoOrThrow("SetPopupParent",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion
    
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

    public static void SetPopupParent(this Popup popup, Control? newParent)
    {
        SetPopupParentMethodInfo.Value.Invoke(popup, [newParent]);
    }
}