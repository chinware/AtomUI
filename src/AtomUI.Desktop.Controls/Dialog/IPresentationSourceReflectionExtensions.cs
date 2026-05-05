using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Input;
using Avalonia.Rendering;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Reflection wrapper for accessing internal members of IPresentationSource.
/// </summary>
internal static class IPresentationSourceReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(IPresentationSource))]
    private static readonly Lazy<PropertyInfo> InputRootPropertyInfo = new(() =>
        typeof(IPresentationSource).GetPropertyInfoOrThrow("InputRoot",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

    #endregion

    /// <summary>
    /// Gets the InputRoot property value from IPresentationSource.
    /// </summary>
    public static IInputRoot? GetInputRoot(this IPresentationSource presentationSource)
    {
        return InputRootPropertyInfo.Value.GetValue(presentationSource) as IInputRoot;
    }
}
