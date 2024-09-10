using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AtomUI.Reflection;

public static class TypeExtension
{
    public static bool TryGetPropertyInfo(
        this Type type,
        string name,
        [NotNullWhen(true)] out PropertyInfo? info,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
    {
        info = type.GetProperty(name, flags);
        return info is not null;
    }

    public static bool TryGetFieldInfo(
        this Type type,
        string name,
        [NotNullWhen(true)] out FieldInfo? info,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
    {
        info = type.GetField(name, flags);
        return info is not null;
    }

    public static bool TryGetMethodInfo(
        this Type type,
        string name,
        [NotNullWhen(true)] out MethodInfo? info,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                             BindingFlags.FlattenHierarchy)
    {
        info = type.GetMethod(name, flags);
        return info is not null;
    }

    public static PropertyInfo GetPropertyInfoOrThrow(
        this Type type,
        string name,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                             BindingFlags.FlattenHierarchy)
    {
        PropertyInfo? info;

        if (!type.TryGetPropertyInfo(name, out info, flags))
        {
            throw new NotSupportedException($"Can not find the '{name}' from type '{type}'. We can not reflect it.");
        }

        return info;
    }

    public static FieldInfo GetFieldInfoOrThrow(this Type type, string name,
                                                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic |
                                                                     BindingFlags.Public |
                                                                     BindingFlags.FlattenHierarchy)
    {
        FieldInfo? info;
        if (!type.TryGetFieldInfo(name, out info, flags))
        {
            throw new NotSupportedException($"Can not find the '{name}' from type '{type}'. We can not reflect it.");
        }

        return info;
    }

    public static MethodInfo GetMethodInfoOrThrow(this Type type, string name,
                                                  BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic |
                                                                       BindingFlags.Public |
                                                                       BindingFlags.FlattenHierarchy)
    {
        MethodInfo? info;
        if (!type.TryGetMethodInfo(name, out info, flags))
        {
            throw new NotSupportedException($"Can not find the '{name}' from type '{type}'. We can not reflect it.");
        }

        return info;
    }
}