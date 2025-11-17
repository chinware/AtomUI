using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Input;
using Avalonia.Input.Raw;

namespace AtomUI.Controls.Utils;

internal static class RawPointerEventTypeReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(RawPointerEventArgs))]
    private static readonly Lazy<PropertyInfo> InputHitTestResultPropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(RawPointerEventArgs).GetPropertyInfoOrThrow("InputHitTestResult",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion

    public static (IInputElement? element, IInputElement? firstEnabledAncestor) GetInputHitTestResult(this RawPointerEventArgs rawPointerEventArgs)
    {
        return rawPointerEventArgs.GetPropertyOrThrow<(IInputElement?, IInputElement?)>(InputHitTestResultPropertyInfo.Value);
    }
}