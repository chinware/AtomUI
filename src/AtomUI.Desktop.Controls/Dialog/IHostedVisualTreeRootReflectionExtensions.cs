using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Reflection wrapper for accessing internal IHostedVisualTreeRoot interface.
/// </summary>
internal static class IHostedVisualTreeRootReflectionExtensions
{
    #region 反射信息定义

    private static readonly Lazy<Type?> IHostedVisualTreeRootType = new(() =>
        typeof(Visual).Assembly.GetType("Avalonia.VisualTree.IHostedVisualTreeRoot"));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, "Avalonia.VisualTree.IHostedVisualTreeRoot", "Avalonia.Base")]
    private static readonly Lazy<PropertyInfo?> HostPropertyInfo = new(() =>
    {
        var type = IHostedVisualTreeRootType.Value;
        return type?.GetPropertyInfoOrThrow("Host", BindingFlags.Instance | BindingFlags.Public);
    });

    #endregion

    /// <summary>
    /// Checks if a visual is an IHostedVisualTreeRoot.
    /// </summary>
    public static bool IsHostedVisualTreeRoot(this Visual visual)
    {
        var type = IHostedVisualTreeRootType.Value;
        return type != null && type.IsInstanceOfType(visual);
    }

    /// <summary>
    /// Gets the Host property value from IHostedVisualTreeRoot.
    /// </summary>
    public static Visual? GetHost(this Visual visual)
    {
        if (!visual.IsHostedVisualTreeRoot())
        {
            return null;
        }

        return HostPropertyInfo.Value?.GetValue(visual) as Visual;
    }
}
