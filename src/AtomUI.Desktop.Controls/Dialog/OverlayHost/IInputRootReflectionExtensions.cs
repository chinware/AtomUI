using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Input;
using Avalonia.Input.TextInput;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Reflection wrapper for accessing internal members of Avalonia's IInputRoot.
/// </summary>
internal static class IInputRootReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(IInputRoot))]
    private static readonly Lazy<PropertyInfo> PointerOverElementPropertyInfo = new(() =>
        typeof(IInputRoot).GetPropertyInfoOrThrow("PointerOverElement",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(IInputRoot))]
    private static readonly Lazy<PropertyInfo> InputMethodPropertyInfo = new(() =>
        typeof(IInputRoot).GetPropertyInfoOrThrow("InputMethod",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(IInputRoot))]
    private static readonly Lazy<PropertyInfo> RootElementPropertyInfo = new(() =>
        typeof(IInputRoot).GetPropertyInfoOrThrow("RootElement",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

    #endregion

    /// <summary>
    /// Gets the PointerOverElement property value.
    /// </summary>
    public static IInputElement? GetPointerOverElement(this IInputRoot inputRoot)
    {
        return PointerOverElementPropertyInfo.Value.GetValue(inputRoot) as IInputElement;
    }

    /// <summary>
    /// Sets the PointerOverElement property value.
    /// </summary>
    public static void SetPointerOverElement(this IInputRoot inputRoot, IInputElement? value)
    {
        PointerOverElementPropertyInfo.Value.SetValue(inputRoot, value);
    }

    /// <summary>
    /// Gets the InputMethod property value.
    /// </summary>
    public static ITextInputMethodImpl? GetInputMethod(this IInputRoot inputRoot)
    {
        return InputMethodPropertyInfo.Value.GetValue(inputRoot) as ITextInputMethodImpl;
    }

    /// <summary>
    /// Gets the RootElement property value.
    /// </summary>
    public static InputElement GetRootElement(this IInputRoot inputRoot)
    {
        var value = RootElementPropertyInfo.Value.GetValue(inputRoot) as InputElement;
        Debug.Assert(value != null);
        return value;
    }
}
