using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls.Primitives;

internal static class TopLevelReflectionExtensions
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(TopLevel))]
    private static readonly Lazy<PropertyInfo> LastPointerPositionPropertyInfo = new(() =>
        typeof(TopLevel).GetPropertyInfoOrThrow("LastPointerPosition",
            BindingFlags.Instance | BindingFlags.NonPublic));

    public static PixelPoint? GetLastPointerPosition(this TopLevel topLevel)
    {
        return LastPointerPositionPropertyInfo.Value.GetValue(topLevel) as PixelPoint?;
    }
}
