# ReflectionExtensions Pattern - AOT-Safe Wrapper for Internal APIs

## 概述

ReflectionExtensions 是 AtomUI 处理 Avalonia 12 中 private/internal 属性、字段、方法的标准化封装模式。这种模式的核心目的是：

1. **AOT 友好** - 使用 `[DynamicDependency]` 标记确保反射成员不被 trim
2. **类型安全** - 编译时检查，运行时反射
3. **性能优化** - 使用 Lazy<T> 缓存反射信息，避免重复反射
4. **易于维护** - 集中管理所有反射依赖，便于 AOT 分析

## 标准结构

### 1. 文件命名规范

```
{TargetClass}ReflectionExtensions.cs
```

**示例：**
- `ScrollBarReflectionExtensions.cs` - 包装 ScrollBar 的反射
- `TextParagraphPropertiesReflectionExtensions.cs` - 包装 TextParagraphProperties 的反射
- `PopupReflectionExtensions.cs` - 包装 Popup 的反射

### 2. 类定义规范

```csharp
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;  // 必须引入
using Avalonia.XXX;       // 目标类所在命名空间

namespace AtomUI.XXX;

/// 必须是 internal static 类
internal static class {TargetClass}ReflectionExtensions
{
    // 实现
}
```

### 3. 反射信息定义规范

#### 3.1 属性反射

```csharp
#region 反射信息定义

[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(TargetClass))]
private static readonly Lazy<PropertyInfo> PropertyNamePropertyInfo = new Lazy<PropertyInfo>(() => 
    typeof(TargetClass).GetPropertyInfoOrThrow("PropertyName",
        BindingFlags.Instance | BindingFlags.NonPublic));

#endregion
```

**关键点：**
- 使用 `[DynamicDependency]` 标记 - **必须**
- 指定 `DynamicallyAccessedMemberTypes.NonPublicProperties` - 告诉 AOT 这个属性需要保留
- 使用 `Lazy<PropertyInfo>` - 延迟初始化，避免启动时反射
- 命名规范：`{PropertyName}PropertyInfo`
- 使用 `GetPropertyInfoOrThrow()` - 如果找不到会抛异常，便于调试

#### 3.2 字段反射

```csharp
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(TargetClass))]
private static readonly Lazy<FieldInfo> FieldNameFieldInfo = new Lazy<FieldInfo>(() => 
    typeof(TargetClass).GetFieldInfoOrThrow("_fieldName",
        BindingFlags.Instance | BindingFlags.NonPublic));
```

**关键点：**
- 使用 `DynamicallyAccessedMemberTypes.NonPublicFields`
- 命名规范：`{FieldName}FieldInfo`
- 字段名通常以 `_` 开头

#### 3.3 方法反射

```csharp
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(TargetClass))]
private static readonly Lazy<MethodInfo> MethodNameMethodInfo = new Lazy<MethodInfo>(() =>
    typeof(TargetClass).GetMethodInfoOrThrow("MethodName",
        BindingFlags.Instance | BindingFlags.NonPublic));
```

**关键点：**
- 使用 `DynamicallyAccessedMemberTypes.NonPublicMethods`
- 命名规范：`{MethodName}MethodInfo`

#### 3.4 事件反射

```csharp
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicEvents, typeof(TargetClass))]
private static readonly Lazy<EventInfo> EventNameEventInfo = new Lazy<EventInfo>(() =>
    typeof(TargetClass).GetEventInfoOrThrow("EventName", 
        BindingFlags.NonPublic | BindingFlags.Instance));
```

**关键点：**
- 使用 `DynamicallyAccessedMemberTypes.NonPublicEvents`
- 命名规范：`{EventName}EventInfo`

### 4. 扩展方法规范

#### 4.1 属性 Getter

```csharp
public static PropertyType GetPropertyName(this TargetClass target)
{
    return PropertyNamePropertyInfo.Value.GetValue(target) as PropertyType;
}

// 或带 Debug.Assert 的版本（推荐）
public static PropertyType GetPropertyName(this TargetClass target)
{
    var value = PropertyNamePropertyInfo.Value.GetValue(target) as PropertyType;
    Debug.Assert(value != null);
    return value;
}

// 或值类型版本
public static double GetLineSpacing(this TextParagraphProperties properties)
{
    var lineSpacing = LineSpacingPropertyInfo.Value.GetValue(properties) as double?;
    Debug.Assert(lineSpacing != null);
    return lineSpacing.Value;
}
```

#### 4.2 属性 Setter

```csharp
public static void SetPropertyName(this TargetClass target, PropertyType value)
{
    PropertyNamePropertyInfo.Value.SetValue(target, value);
}
```

#### 4.3 字段 Getter

```csharp
public static FieldType GetFieldName(this TargetClass target)
{
    var value = FieldNameFieldInfo.Value.GetValue(target) as FieldType;
    Debug.Assert(value != null);
    return value;
}
```

#### 4.4 字段 Setter

```csharp
public static void SetFieldName(this TargetClass target, FieldType value)
{
    FieldNameFieldInfo.Value.SetValue(target, value);
}
```

#### 4.5 方法调用

```csharp
public static void InvokeMethodName(this TargetClass target, ParameterType param)
{
    MethodNameMethodInfo.Value.Invoke(target, [param]);
}

// 带返回值
public static ReturnType InvokeMethodName(this TargetClass target, ParameterType param)
{
    var result = MethodNameMethodInfo.Value.Invoke(target, [param]);
    Debug.Assert(result != null);
    return (ReturnType)result;
}
```

#### 4.6 事件处理

```csharp
public static void AddEventHandler(this TargetClass target, EventHandler<EventArgsType> handler)
{
    var addMethod = EventNameEventInfo.Value.GetAddMethod(true);
    addMethod?.Invoke(target, [handler]);
}

public static void RemoveEventHandler(this TargetClass target, EventHandler<EventArgsType> handler)
{
    var removeMethod = EventNameEventInfo.Value.GetRemoveMethod(true);
    removeMethod?.Invoke(target, [handler]);
}
```

## 完整示例

### 示例 1：简单属性包装

```csharp
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.Commons;

internal static class ScrollBarReflectionExtensions
{
    #region 反射信息定义
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(ScrollBar))]
    private static readonly Lazy<PropertyInfo> IsExpandedPropertyInfo = new Lazy<PropertyInfo>(() =>
        typeof(ScrollBar).GetPropertyInfoOrThrow("IsExpanded",
            BindingFlags.Instance | BindingFlags.Public));
    
    #endregion
    
    public static void SetIsExpanded(this ScrollBar scrollBar, bool value)
    {
        var isExpandedSetter = IsExpandedPropertyInfo.Value.GetSetMethod(true);
        Debug.Assert(isExpandedSetter != null);
        isExpandedSetter.Invoke(scrollBar, [value]);
    }
}
```

### 示例 2：混合字段和属性

```csharp
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal static class ItemsControlReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(ItemsControl))]
    private static readonly Lazy<PropertyInfo> WrapFocusPropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(ItemsControl).GetPropertyInfoOrThrow("WrapFocus",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(ItemsControl))]
    private static readonly Lazy<FieldInfo> ItemsFieldInfo = new Lazy<FieldInfo>(() => 
        typeof(ItemsControl).GetFieldInfoOrThrow("_items",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion
    
    public static bool GetWrapFocus(this ItemsControl itemsControl)
    {
        return WrapFocusPropertyInfo.Value.GetValue(itemsControl) as bool? ?? false;
    }

    public static void SetWrapFocus(this ItemsControl itemsControl, bool value)
    {
        WrapFocusPropertyInfo.Value.SetValue(itemsControl, value);
    }
    
    public static ItemCollection GetItems(this ItemsControl itemsControl)
    {
        var item = ItemsFieldInfo.Value.GetValue(itemsControl) as ItemCollection;
        Debug.Assert(item != null);
        return item;
    }
}
```

### 示例 3：事件和方法

```csharp
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

internal static class PopupReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicEvents, typeof(AvaloniaPopup))]
    private static readonly Lazy<EventInfo> ClosingEventInfo = new Lazy<EventInfo>(() =>
        typeof(AvaloniaPopup).GetEventInfoOrThrow("Closing", BindingFlags.NonPublic | BindingFlags.Instance));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaPopup))]
    private static readonly Lazy<MethodInfo> SetPopupParentMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(AvaloniaPopup).GetMethodInfoOrThrow("SetPopupParent",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion
    
    public static void AddClosingEventHandler(this AvaloniaPopup popup, EventHandler<CancelEventArgs> handler)
    {
        var closingEventAddMethod = ClosingEventInfo.Value.GetAddMethod(true);
        closingEventAddMethod?.Invoke(popup, [handler]);
    }

    public static void RemoveClosingEventHandler(this AvaloniaPopup popup, EventHandler<CancelEventArgs> handler)
    {
        var closingEventRemoveMethod = ClosingEventInfo.Value.GetRemoveMethod(true);
        closingEventRemoveMethod?.Invoke(popup, [handler]);
    }
    
    public static void SetPopupParent(this AvaloniaPopup popup, Control? newParent)
    {
        SetPopupParentMethodInfo.Value.Invoke(popup, [newParent]);
    }
}
```

## AOT 注意事项

### 1. DynamicDependency 属性映射

| 反射类型 | DynamicallyAccessedMemberTypes |
|---------|-------------------------------|
| 属性 (Property) | `NonPublicProperties` |
| 字段 (Field) | `NonPublicFields` |
| 方法 (Method) | `NonPublicMethods` |
| 事件 (Event) | `NonPublicEvents` |
| 构造函数 | `PublicConstructors` / `NonPublicConstructors` |

### 2. BindingFlags 组合

```csharp
// 最常用的组合
BindingFlags.Instance | BindingFlags.NonPublic

// 包含公开成员
BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public

// 包含继承的成员
BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy
```

### 3. 验证 AOT 安全性

在发布 AOT 应用前，检查：

```bash
# 1. 所有 ReflectionExtensions 类都有 [DynamicDependency] 标记
grep -r "DynamicDependency" src/

# 2. 所有反射成员都在 DynamicDependency 中声明
# 3. 运行 AOT 编译测试
dotnet publish -c Release -r win-x64 /p:PublishAot=true
```

## 性能考虑

### 1. Lazy<T> 缓存

```csharp
// ✅ 好 - 只反射一次
private static readonly Lazy<PropertyInfo> PropertyInfo = new Lazy<PropertyInfo>(() => 
    typeof(TargetClass).GetPropertyInfoOrThrow("Property", ...));

// ❌ 不好 - 每次都反射
public static void SetProperty(this TargetClass target, object value)
{
    typeof(TargetClass).GetProperty("Property", ...).SetValue(target, value);
}
```

### 2. 避免不必要的装箱

```csharp
// ✅ 好 - 值类型直接处理
public static double GetValue(this TargetClass target)
{
    var value = PropertyInfo.Value.GetValue(target) as double?;
    Debug.Assert(value != null);
    return value.Value;  // 拆箱一次
}

// ❌ 不好 - 多次装箱
public static object GetValue(this TargetClass target)
{
    return PropertyInfo.Value.GetValue(target);  // 返回装箱对象
}
```

## 维护清单

- [ ] 所有 ReflectionExtensions 类都在 `#region 反射信息定义` 中声明反射信息
- [ ] 每个反射成员都有 `[DynamicDependency]` 标记
- [ ] 使用 `GetXxxInfoOrThrow()` 方法确保找不到时抛异常
- [ ] 使用 `Lazy<T>` 缓存反射信息
- [ ] 扩展方法使用 `Debug.Assert()` 验证非空值
- [ ] 文件名遵循 `{TargetClass}ReflectionExtensions.cs` 规范
- [ ] 类是 `internal static` 的
- [ ] 反射信息字段是 `private static readonly` 的
- [ ] 命名规范：`{MemberName}{MemberType}Info` (e.g., `PropertyNamePropertyInfo`)

