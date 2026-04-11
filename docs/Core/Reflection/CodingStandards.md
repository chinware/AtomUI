# 编码规范

> 本文档规定 AtomUI 中所有 Avalonia 内部成员反射访问代码的编写标准。

---

## 1. 文件命名与组织

### 1.1 命名规则

反射扩展类的文件和类名 **必须** 遵循以下格式：

```
{Avalonia目标类型名}ReflectionExtensions.cs
```

示例：

| Avalonia 目标类型 | 文件名 | 类名 |
|---|---|---|
| `Visual` | `VisualReflectionExtensions.cs` | `VisualReflectionExtensions` |
| `StyledElement` | `StyledElementReflectionExtensions.cs` | `StyledElementReflectionExtensions` |
| `TextBox` | `TextBoxReflectionExtensions.cs` | `TextBoxReflectionExtensions` |
| `WindowBase` | `WindowBaseReflectionExtensions.cs` | `WindowBaseReflectionExtensions` |
| `SelectionModel<>` | `SelectionModelReflectionExtensions.cs` | `SelectionModelReflectionExtensions` |

### 1.2 文件位置

- **通用反射扩展**（多个控件共用的 Avalonia 基础类型）放在 `AtomUI.Core` 的对应目录下：
  - `Reflection/` — `StyledElement`、`Visual` 等基础类型
  - `Controls/` — `ItemCollection`、`ILayoutRoot` 等控件相关类型
  - `Animations/` — `Animatable` 等动画相关类型
  - `Data/` — `DynamicResourceExtension` 等数据绑定类型
  - `Media/` — `TextParagraphProperties` 等媒体类型
  - `Utils/` — `ManagedPopupPositioner`、`AvaloniaProperty` 等工具类型
  - `Input/` — `DispatcherPriority` 等输入相关类型

- **特定控件的反射扩展** 放在 `AtomUI.Desktop.Controls`（或对应平台控件项目）中该控件的文件夹下：
  - `TextBlock/TextBlockReflectionExtensions.cs`
  - `Input/Utils/TextBoxReflectionExtensions.cs`
  - `Window/WindowBaseReflectionExtensions.cs`
  - `ComboBox/ComboBoxReflectionExtensions.cs`
  - `Menu/MenuReflectionExtensions.cs`
  - `Popup/PopupReflectionExtensions.cs`
  - `Primitives/SelectingItemsControlReflectionExtensions.cs`

### 1.3 就近放置原则

如果一个反射扩展只被某个特定控件使用，应放在该控件的文件夹内。如果被多个控件跨模块使用，应下沉到 `AtomUI.Core`。

---

## 2. 类结构标准模板

每个反射扩展类 **必须** 遵循以下结构：

```csharp
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
// ... 其他必要的 using

namespace AtomUI.Desktop.Controls;  // 或 AtomUI.Controls / AtomUI.Xxx

// ① 如有命名冲突，使用 using 别名
using AvaloniaTextBox = Avalonia.Controls.TextBox;

// ② 类声明：internal static class，命名为 {目标类型}ReflectionExtensions
internal static class TextBoxReflectionExtensions
{
    // ③ 反射信息定义区域（用 #region 包裹）
    #region 反射信息定义

    // ④ 每个反射缓存字段都必须标注 [DynamicDependency]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaTextBox))]
    private static readonly Lazy<FieldInfo> ScrollViewerFieldInfo = new Lazy<FieldInfo>(() =>
        typeof(AvaloniaTextBox).GetFieldInfoOrThrow("_scrollViewer",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaTextBox))]
    private static readonly Lazy<MethodInfo> HandleTextInputMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(AvaloniaTextBox).GetMethodInfoOrThrow("HandleTextInput",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion

    // ⑤ 公开的扩展方法（强类型包装）
    public static ScrollViewer? GetScrollViewer(this AvaloniaTextBox textBox)
    {
        return ScrollViewerFieldInfo.Value.GetValue(textBox) as ScrollViewer;
    }

    public static void HandleTextInput(this AvaloniaTextBox textBox, string? input)
    {
        HandleTextInputMethodInfo.Value.Invoke(textBox, [input]);
    }
}
```

---

## 3. 反射缓存字段规范

### 3.1 声明规则

| 规则 | 说明 |
|---|---|
| 访问修饰符 | `private static readonly`（仅在极少数跨类共享场景下使用 `internal static readonly`） |
| 类型 | `Lazy<FieldInfo>` / `Lazy<PropertyInfo>` / `Lazy<MethodInfo>` / `Lazy<EventInfo>` |
| 命名 | `{成员名}FieldInfo` / `{成员名}PropertyInfo` / `{成员名}MethodInfo` / `{成员名}EventInfo` |
| 注解 | **必须** 标注 `[DynamicDependency]` |
| 初始化 | 使用 `TypeMemberExtension` 中的 `GetXxxInfoOrThrow` 方法 |

### 3.2 `[DynamicDependency]` 标注规则

根据访问的成员类型选择正确的 `DynamicallyAccessedMemberTypes`：

| 反射访问目标 | `DynamicallyAccessedMemberTypes` 值 |
|---|---|
| 私有/内部字段 | `NonPublicFields` |
| 私有/内部属性 | `NonPublicProperties` |
| 私有/内部方法 | `NonPublicMethods` |
| 私有/内部事件 | `NonPublicEvents` |

```csharp
// ✅ 正确：访问私有字段，使用 NonPublicFields
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaComboBox))]
private static readonly Lazy<FieldInfo> PopupFieldInfo = ...;

// ✅ 正确：访问私有方法，使用 NonPublicMethods
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(Animatable))]
private static readonly Lazy<MethodInfo> EnableTransitionsMethodInfo = ...;

// ✅ 正确：访问私有事件，使用 NonPublicEvents
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicEvents, typeof(Popup))]
private static readonly Lazy<EventInfo> ClosingEventInfo = ...;

// ❌ 错误：缺少 [DynamicDependency] 注解
private static readonly Lazy<FieldInfo> SomeFieldInfo = new Lazy<FieldInfo>(() =>
    typeof(SomeType).GetFieldInfoOrThrow("_someField", ...));

// ❌ 错误：DynamicallyAccessedMemberTypes 与实际访问的成员类型不匹配
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(SomeType))]  // 访问的是字段！
private static readonly Lazy<FieldInfo> SomeFieldInfo = ...;
```

### 3.3 `BindingFlags` 选择指南

| 成员特征 | `BindingFlags` |
|---|---|
| 实例私有成员 | `BindingFlags.Instance \| BindingFlags.NonPublic` |
| 静态私有成员 | `BindingFlags.Static \| BindingFlags.NonPublic` |
| 公开成员（偶尔需要） | `BindingFlags.Instance \| BindingFlags.Public` |
| 需要搜索继承层次 | 追加 `BindingFlags.FlattenHierarchy` |

> **注意**：`GetXxxInfoOrThrow` 方法的默认 `flags` 已包含 `FlattenHierarchy`，通常只需显式指定 `Instance | NonPublic` 即可。但如果目标成员定义在基类中且需要明确搜索继承链，应显式添加 `FlattenHierarchy`。

### 3.4 命名约定

反射缓存字段命名遵循 `{Avalonia成员名}{Info类型}` 模式：

```csharp
// 字段：_scrollViewer → ScrollViewerFieldInfo
private static readonly Lazy<FieldInfo> ScrollViewerFieldInfo = ...;

// 属性：LogicalChildren → LogicalChildrenPropertyInfo
private static readonly Lazy<PropertyInfo> LogicalChildrenPropertyInfo = ...;

// 方法：SetVisualParent → SetVisualParentMethodInfo
private static readonly Lazy<MethodInfo> SetVisualParentMethodInfo = ...;

// 事件：Closing → ClosingEventInfo
private static readonly Lazy<EventInfo> ClosingEventInfo = ...;
```

字段名中 **不包含** 目标类型名称前缀（因为所属类名已经包含了目标类型信息）。  
例外：如果一个扩展类中有来自不同 Avalonia 类型的反射成员（极少见），可用类型名前缀区分。

---

## 4. 扩展方法封装规范

### 4.1 可见性

| 场景 | 可见性 |
|---|---|
| 被控件外部直接调用 | `public` 或 `internal` |
| 仅在扩展类内部组合使用 | `private` |
| 复杂流程中的多步骤方法 | 入口方法 `public`，子步骤 `private` |

参考 `WindowBaseReflectionExtensions`：
```csharp
// 入口方法：public
public static void ShowWithoutActive(this WindowBase window) { ... }

// 子步骤：private
private static IDisposable FreezeVisibilityChangeHandling(this WindowBase window) { ... }
private static void EnsureInitialized(this WindowBase window) { ... }
private static void StartRendering(this WindowBase window) { ... }
```

### 4.2 返回值处理

根据返回值类型选择不同的处理策略：

**值类型返回值 —— 使用 `as T?` + `Debug.Assert`**

```csharp
public static Size GetMaxSizeFromConstraint(this AvaloniaTextBlock textBlock)
{
    var size = GetMaxSizeFromConstraintMethodInfo.Value.Invoke(textBlock, []) as Size?;
    Debug.Assert(size != null);
    return size.Value;
}
```

**引用类型返回值（非空预期）—— 使用 `as T` + `Debug.Assert`**

```csharp
public static IAvaloniaList<ILogical> GetLogicalChildrenList(this StyledElement styledElement)
{
    var children = LogicalChildrenPropertyInfo.Value.GetValue(styledElement) as IAvaloniaList<ILogical>;
    Debug.Assert(children != null);
    return children;
}
```

**引用类型返回值（可空）—— 直接 `as T`**

```csharp
public static ScrollViewer? GetScrollViewer(this AvaloniaTextBox textBox)
{
    return ScrollViewerFieldInfo.Value.GetValue(textBox) as ScrollViewer;
}
```

**无返回值（方法调用 / 设值操作）—— 直接调用**

```csharp
public static void SetPopup(this AvaloniaComboBox comboBox, Popup? popup)
{
    PopupFieldInfo.Value.SetValue(comboBox, popup);
}

public static void HandleTextInput(this AvaloniaTextBox textBox, string? input)
{
    HandleTextInputMethodInfo.Value.Invoke(textBox, [input]);
}
```

### 4.3 `this` 参数类型

扩展方法的 `this` 参数类型 **必须** 是被 Hook 的 Avalonia 类型（或其别名），不能是 AtomUI 自定义子类：

```csharp
// ✅ 正确：this 参数是 Avalonia 类型
public static void SetVisualParent(this Visual visual, Control? parent) { ... }
public static void SetPopup(this AvaloniaComboBox comboBox, Popup? popup) { ... }

// ❌ 错误：this 参数不应该是 AtomUI 子类（除非扩展确实只适用于子类）
public static void SetPopup(this ComboBox comboBox, Popup? popup) { ... }
```

---

## 5. `using` 类型别名规范

当 AtomUI 自定义控件与 Avalonia 内置控件同名时，**必须** 在反射扩展类文件顶部声明 `using` 别名：

```csharp
using AvaloniaTextBox = Avalonia.Controls.TextBox;
using AvaloniaTextBlock = Avalonia.Controls.TextBlock;
using AvaloniaComboBox = Avalonia.Controls.ComboBox;
using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;
using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;
```

别名统一使用 `Avalonia` 前缀 + 原始类型名，即 `Avalonia{ClassName}`。

---

## 6. 泛型目标类型

当反射目标是泛型类型时，使用开放泛型 `typeof(SelectionModel<>)` 而非具体闭合类型：

```csharp
// ✅ 正确：使用开放泛型
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(SelectionModel<>))]
public static void SetSource<T>(this SelectionModel<T> model, IEnumerable? value)
{
    var targetType = typeof(SelectionModel<T>);
    var method = _setSourceMethodCache.GetOrAdd(targetType, t =>
        t.GetMethodInfoOrThrow("SetSource", BindingFlags.Instance | BindingFlags.NonPublic));
    method.Invoke(model, [value]);
}
```

对于泛型类型，由于闭合类型的 `MethodInfo` 各不相同，应使用 `ConcurrentDictionary<Type, MethodInfo>` 缓存替代 `Lazy<MethodInfo>`：

```csharp
private static readonly ConcurrentDictionary<Type, MethodInfo> _setSourceMethodCache = new();
```

---

## 7. 事件 Hook 规范

对于需要反射访问的私有事件，使用 `EventInfo` + `GetAddMethod()` / `GetRemoveMethod()`：

```csharp
#region 反射信息定义

[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicEvents, typeof(Popup))]
private static readonly Lazy<EventInfo> ClosingEventInfo = new Lazy<EventInfo>(() =>
    typeof(Popup).GetEventInfoOrThrow("Closing", BindingFlags.NonPublic | BindingFlags.Instance));

#endregion

public static void AddClosingEventHandler(this Popup popup, EventHandler<CancelEventArgs> handler)
{
    var closingEventAddMethod = ClosingEventInfo.Value.GetAddMethod();
    closingEventAddMethod?.Invoke(popup, [handler]);
}

public static void RemoveClosingEventHandler(this Popup popup, EventHandler<CancelEventArgs> handler)
{
    var closingEventRemoveMethod = ClosingEventInfo.Value.GetRemoveMethod();
    closingEventRemoveMethod?.Invoke(popup, [handler]);
}
```

---

## 8. 禁止事项

| ❌ 禁止 | 说明 |
|---|---|
| 在业务代码中直接使用 `typeof(X).GetMethod(...)` | 所有反射访问必须封装在 `XxxReflectionExtensions` 中 |
| 缺少 `[DynamicDependency]` 注解 | 会导致 Trimming 后运行时异常 |
| 使用 `GetMethod` / `GetField` / `GetProperty` 而非 `OrThrow` 版本 | 丢失快速失败能力，可能出现 `NullReferenceException` |
| 在反射缓存中使用非 `Lazy<T>` 的静态字段 | 丧失延迟初始化和线程安全性 |
| 将反射扩展类标记为 `public` | 反射 Hook 是内部实现细节，不应暴露给外部消费者 |
| 反射扩展类中包含非反射的业务逻辑 | 扩展类应保持纯粹，仅包含反射访问封装 |

---

## 9. 完整示例

以下是一个符合所有规范的反射扩展类完整示例：

```csharp
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

using AvaloniaComboBox = Avalonia.Controls.ComboBox;

/// <summary>
/// 提供对 Avalonia ComboBox 私有成员的反射访问。
/// </summary>
/// <remarks>
/// 此类 Hook 的成员不受 Avalonia 公开 API 契约保护，
/// 升级 Avalonia 版本时需要验证兼容性。
/// 
/// Hook 目标（Avalonia v11.3.x）：
/// - _popup (PrivateField): Avalonia.Controls.Primitives.Popup
/// </remarks>
internal static class ComboBoxReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaComboBox))]
    private static readonly Lazy<FieldInfo> PopupFieldInfo = new Lazy<FieldInfo>(() =>
        typeof(AvaloniaComboBox).GetFieldInfoOrThrow("_popup",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion

    public static void SetPopup(this AvaloniaComboBox comboBox, Popup? popup)
    {
        PopupFieldInfo.Value.SetValue(comboBox, popup);
    }
}
```

