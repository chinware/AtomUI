using System.Diagnostics;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace AtomUI.Utils;

internal static class ManagedPopupPositionerReflectionExtensions
{
    #region 反射信息定义

    private static readonly Lazy<FieldInfo> ManagedPopupPositionerPopupInfo = new Lazy<FieldInfo>(
        () => typeof(ManagedPopupPositioner).GetFieldInfoOrThrow("_popup",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion

    public static IManagedPopupPositionerPopup GetManagedPopupPositionerPopup(this ManagedPopupPositioner managedPopupPositioner)
    {
        var popup = ManagedPopupPositionerPopupInfo.Value.GetValue(managedPopupPositioner) as IManagedPopupPositionerPopup;
        Debug.Assert(popup != null);
        return popup;
    }
}