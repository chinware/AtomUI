using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls.Presenters;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

internal static class TextBlockReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaTextBlock))]
    private static readonly Lazy<MethodInfo> GetMaxSizeFromConstraintMethodInfo = new Lazy<MethodInfo>(() => 
        typeof(AvaloniaTextBlock).GetMethodInfoOrThrow("GetMaxSizeFromConstraint",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AvaloniaTextBlock))]
    private static readonly Lazy<PropertyInfo> HasComplexContentPropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(AvaloniaTextBlock).GetPropertyInfoOrThrow("HasComplexContent",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion

    public static Size GetMaxSizeFromConstraint(this AvaloniaTextBlock textBlock)
    {
        var size = GetMaxSizeFromConstraintMethodInfo.Value.Invoke(textBlock, []) as Size?;
        Debug.Assert(size != null);
        return size.Value;
    }

    public static bool GetHasComplexContent(this AvaloniaTextBlock textBlock)
    {
        var hasComplexContent = HasComplexContentPropertyInfo.Value.GetValue(textBlock, null) as bool?;
        Debug.Assert(hasComplexContent != null);
        return hasComplexContent.Value;
    }
}