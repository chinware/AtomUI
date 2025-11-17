using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Animation;

namespace AtomUI.Animations;

internal static class AnimatableReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(Animatable))]
    private static readonly Lazy<MethodInfo> EnableTransitionsMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(Animatable).GetMethodInfoOrThrow("EnableTransitions",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(Animatable))]
    private static readonly Lazy<MethodInfo> DisableTransitionsMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(Animatable).GetMethodInfoOrThrow("DisableTransitions",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion
    
    public static void EnableTransitions(this Animatable animatable)
    {
        EnableTransitionsMethodInfo.Value.Invoke(animatable, []);
    }
    
    public static void DisableTransitions(this Animatable animatable)
    {
        DisableTransitionsMethodInfo.Value.Invoke(animatable, []);
    }
}