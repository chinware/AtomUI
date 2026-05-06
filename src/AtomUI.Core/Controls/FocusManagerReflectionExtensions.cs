using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal static class FocusManagerReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(FocusManager))]
    private static readonly Lazy<MethodInfo> GetFocusManagerMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(FocusManager).GetMethodInfoOrThrow("GetFocusManager",
            BindingFlags.Static | BindingFlags.NonPublic));

    #endregion

    /// <summary>
    /// Calls internal FocusManager.GetFocusManager(IInputElement? element)
    /// </summary>
    public static FocusManager? GetFocusManager(IInputElement? element)
    {
        return GetFocusManagerMethodInfo.Value.Invoke(null, [element]) as FocusManager;
    }
}
