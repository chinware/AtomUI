using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal static class ItemCollectionReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(ItemCollection))]
    private static readonly Lazy<MethodInfo> SetItemsSourceMethodInfo = new Lazy<MethodInfo>(() => 
        typeof(ItemCollection).GetMethodInfoOrThrow("SetItemsSource",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    #endregion

    public static void SetItemsSource(this ItemCollection items, IEnumerable? value)
    {
        SetItemsSourceMethodInfo.Value.Invoke(items, [value]);
    }
}