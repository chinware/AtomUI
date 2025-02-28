using System.Collections;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;

namespace AtomUI.Controls.Utils;

internal static class ItemCollectionReflectionExtensions
{
    #region 反射信息定义
    
    private static readonly Lazy<MethodInfo> SetItemsSourceMethodInfo = new Lazy<MethodInfo>(() => 
        typeof(ItemCollection).GetMethodInfoOrThrow("SetItemsSource",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    #endregion

    public static void SetItemsSource(this ItemCollection items, IEnumerable? value)
    {
        SetItemsSourceMethodInfo.Value.Invoke(items, [value]);
    }
}