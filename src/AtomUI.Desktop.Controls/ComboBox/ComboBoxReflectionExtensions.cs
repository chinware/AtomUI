using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;

using AvaloniaComboBox = Avalonia.Controls.ComboBox;
using AvaloniaTextBox = Avalonia.Controls.TextBox;

namespace AtomUI.Desktop.Controls;

internal static class ComboBoxReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaComboBox))]
    private static readonly Lazy<FieldInfo> PopupFieldInfo = new Lazy<FieldInfo>(() => 
        typeof(AvaloniaComboBox).GetFieldInfoOrThrow("_popup",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaComboBox))]
    private static readonly Lazy<FieldInfo> InputTextBoxFieldInfo = new Lazy<FieldInfo>(() =>
        typeof(AvaloniaComboBox).GetFieldInfoOrThrow("_inputTextBox",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion
    
    public static void SetPopup(this AvaloniaComboBox comboBox, Popup? popup)
    {
        PopupFieldInfo.Value.SetValue(comboBox, popup);
    }

    public static void SetInputTextBox(this AvaloniaComboBox comboBox, AvaloniaTextBox? textBox)
    {
        InputTextBoxFieldInfo.Value.SetValue(comboBox, textBox);
    }
}
