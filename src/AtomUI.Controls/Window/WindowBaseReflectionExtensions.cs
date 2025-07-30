using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Controls.Utils;
using AtomUI.Reflection;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal static class WindowBaseReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(WindowBase))]
    private static readonly Lazy<MethodInfo> FreezeVisibilityChangeHandlingMethodInfo = new Lazy<MethodInfo>(() => 
        typeof(WindowBase).GetMethodInfoOrThrow("FreezeVisibilityChangeHandling",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(WindowBase))]
    private static readonly Lazy<MethodInfo> EnsureInitializedMethodInfo = new Lazy<MethodInfo>(() => 
        typeof(WindowBase).GetMethodInfoOrThrow("EnsureInitialized",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(WindowBase))]
    private static readonly Lazy<FieldInfo> HasExecutedInitialLayoutPassFieldInfo = new Lazy<FieldInfo>(() => 
        typeof(WindowBase).GetFieldInfoOrThrow("_hasExecutedInitialLayoutPass",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(WindowBase))]
    private static readonly Lazy<MethodInfo> StartRenderingMethodInfo = new Lazy<MethodInfo>(() => 
        typeof(WindowBase).GetMethodInfoOrThrow("StartRendering",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(WindowBase))]
    private static readonly Lazy<MethodInfo> OnOpenedMethodInfo = new Lazy<MethodInfo>(() => 
        typeof(WindowBase).GetMethodInfoOrThrow("OnOpened",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    #endregion
    
    public static void ShowWithoutActive(this WindowBase window)
    {
        using (window.FreezeVisibilityChangeHandling())
        {
            window.EnsureInitialized();
            window.ApplyStyling();
            window.IsVisible = true;

            if (!window.HasExecutedInitialLayoutPass())
            {
                window.GetLayoutManager().ExecuteInitialLayoutPass();
                window.SetHasExecutedInitialLayoutPass(true);
            }

            window.PlatformImpl?.Show(false, false);
            window.StartRendering();
            window.OnOpened(EventArgs.Empty);
        }
    }

    private static IDisposable FreezeVisibilityChangeHandling(this WindowBase window)
    {
        var value = FreezeVisibilityChangeHandlingMethodInfo.Value.Invoke(window, null) as IDisposable;
        Debug.Assert(value != null, "FreezeVisibilityChangeHandling() returned null.");
        return value;
    }

    private static void EnsureInitialized(this WindowBase window)
    {
        EnsureInitializedMethodInfo.Value.Invoke(window, null);
    }

    private static bool HasExecutedInitialLayoutPass(this WindowBase window)
    {
        var value = HasExecutedInitialLayoutPassFieldInfo.Value.GetValue(window) as bool?;
        Debug.Assert(value != null, "HasExecutedInitialLayoutPass() returned null.");
        return value.Value;
    }

    private static void SetHasExecutedInitialLayoutPass(this WindowBase window, bool value)
    {
        HasExecutedInitialLayoutPassFieldInfo.Value.SetValue(window, value);
    }

    private static void StartRendering(this WindowBase window)
    {
        StartRenderingMethodInfo.Value.Invoke(window, null);
    }

    private static void OnOpened(this WindowBase window, EventArgs eventArgs)
    {
        OnOpenedMethodInfo.Value.Invoke(window, [eventArgs]);
    }
}