using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Input;

namespace AtomUI.Input;

internal static class IInputRootReflectionExtensions
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(IInputRoot))]
    private static readonly Lazy<PropertyInfo> RootElementPropertyInfo = new(() =>
        typeof(IInputRoot).GetPropertyInfoOrThrow("RootElement",
            BindingFlags.Instance | BindingFlags.NonPublic));

    public static Visual? GetRootElement(this IInputRoot inputRoot)
    {
        return RootElementPropertyInfo.Value.GetValue(inputRoot) as Visual;
    }
}
