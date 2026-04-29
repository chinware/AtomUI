using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace AtomUI.Controls.Commons;

internal static class ScrollBarReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(ScrollBar))]
    private static readonly Lazy<FieldInfo> TimerFieldInfo = new Lazy<FieldInfo>(() => 
        typeof(ScrollBar).GetFieldInfoOrThrow("_timer",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(ScrollBar))]
    private static readonly Lazy<PropertyInfo> IsExpandedPropertyInfo = new Lazy<PropertyInfo>(() =>
        typeof(ScrollBar).GetPropertyInfoOrThrow("IsExpanded",
            BindingFlags.Instance | BindingFlags.Public));
    #endregion
    
    public static DispatcherTimer? GetTimer(this ScrollBar scrollBar)
    {
        return TimerFieldInfo.Value.GetValue(scrollBar) as DispatcherTimer;
    }
    
    public static void SetIsExpanded(this ScrollBar scrollBar, bool value)
    {
        var isExpandedSetter = IsExpandedPropertyInfo.Value.GetSetMethod(true);
        Debug.Assert(isExpandedSetter != null);
        isExpandedSetter.Invoke(scrollBar, [value]);
    }
}