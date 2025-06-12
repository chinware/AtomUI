using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;

namespace AtomUI.Controls;

using AvaloniaButtonSpinner = Avalonia.Controls.ButtonSpinner;
using AvaloniaButton = Avalonia.Controls.Button;

internal static class ButtonSpinnerReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AvaloniaButtonSpinner))]
    private static readonly Lazy<PropertyInfo> IncreaseButtonPropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(AvaloniaButtonSpinner).GetPropertyInfoOrThrow("IncreaseButton",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AvaloniaButtonSpinner))]
    private static readonly Lazy<PropertyInfo> DecreaseButtonPropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(AvaloniaButtonSpinner).GetPropertyInfoOrThrow("DecreaseButton",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion
    
    public static void SetIncreaseButton(this AvaloniaButtonSpinner buttonSpinner, AvaloniaButton? increaseButton)
    {
        IncreaseButtonPropertyInfo.Value.SetValue(buttonSpinner, increaseButton);
    }
    
    public static void SetDecreaseButton(this AvaloniaButtonSpinner buttonSpinner, AvaloniaButton? decreaseButton)
    {
        DecreaseButtonPropertyInfo.Value.SetValue(buttonSpinner, decreaseButton);
    }
}