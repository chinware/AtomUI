using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;

using AvaloniaComboBox = Avalonia.Controls.ComboBox;

namespace AtomUI.Controls;

internal static class ComboBoxReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaComboBox))]
    private static readonly Lazy<FieldInfo> PopupFieldInfo = new Lazy<FieldInfo>(() => 
        typeof(AvaloniaComboBox).GetFieldInfoOrThrow("_popup",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion
    
    public static void SetPopup(this AvaloniaComboBox comboBox, Popup? popup)
    {
        PopupFieldInfo.Value.SetValue(comboBox, popup);
    }

}