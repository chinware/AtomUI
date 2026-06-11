using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AtomUI.Reflection;

public static class TypeMemberExtension
{
    public static bool TryGetPropertyInfo(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        this Type type,
        string name,
        [NotNullWhen(true)] out PropertyInfo? info,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
    {
        info = type.GetProperty(name, flags);
        return info is not null;
    }

    public static bool TryGetFieldInfo(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields |
                                    DynamicallyAccessedMemberTypes.NonPublicFields)]
        this Type type,
        string name,
        [NotNullWhen(true)] out FieldInfo? info,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
    {
        info = type.GetField(name, flags);
        return info is not null;
    }

    public static bool TryGetMethodInfo(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                                    DynamicallyAccessedMemberTypes.NonPublicMethods)]
        this Type type,
        string name,
        [NotNullWhen(true)] out MethodInfo? info,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                             BindingFlags.FlattenHierarchy)
    {
        info = type.GetMethod(name, flags);
        return info is not null;
    }
    
    public static bool TryGetEventInfo(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents)]
        this Type type,
        string name,
        [NotNullWhen(true)] out EventInfo? info,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                             BindingFlags.FlattenHierarchy)
    {
        info = type.GetEvent(name, flags);
        return info is not null;
    }

    public static PropertyInfo GetPropertyInfoOrThrow(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        this Type type,
        string name,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                             BindingFlags.FlattenHierarchy)
    {
        if (!type.TryGetPropertyInfo(name, out var info, flags))
        {
            throw new MissingMemberException($"Can not find the '{name}' from type '{type}'. We can not reflect it.");
        }

        return info;
    }

    public static FieldInfo GetFieldInfoOrThrow(
                                                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields |
                                                                            DynamicallyAccessedMemberTypes.NonPublicFields)]
                                                this Type type, string name,
                                                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic |
                                                                     BindingFlags.Public |
                                                                     BindingFlags.FlattenHierarchy)
    {
        if (!type.TryGetFieldInfo(name, out var info, flags))
        {
            throw new MissingFieldException($"Can not find the '{name}' from type '{type}'. We can not reflect it.");
        }

        return info;
    }

    public static MethodInfo GetMethodInfoOrThrow(
                                                  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                                                                              DynamicallyAccessedMemberTypes.NonPublicMethods)]
                                                  this Type type, string name,
                                                  BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic |
                                                                       BindingFlags.Public |
                                                                       BindingFlags.FlattenHierarchy)
    {
        if (!type.TryGetMethodInfo(name, out var info, flags))
        {
            throw new MissingMethodException($"Can not find the '{name}' from type '{type}'. We can not reflect it.");
        }

        return info;
    }

    public static EventInfo GetEventInfoOrThrow(
                                                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                                                            DynamicallyAccessedMemberTypes.NonPublicEvents)]
                                                this Type type, string name,
                                                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic |
                                                                     BindingFlags.Public |
                                                                     BindingFlags.FlattenHierarchy)
    {
        if (!type.TryGetEventInfo(name, out var info, flags))
        {
            throw new MissingMemberException($"Can not find the event '{name}' from type '{type}'. We can not reflect it.");
        }

        return info;
    }
}
