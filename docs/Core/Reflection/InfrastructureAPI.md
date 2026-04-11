# 基础设施 API 参考

> 本文档介绍 `AtomUI.Core/Reflection/` 命名空间中提供的反射工具方法，这些是所有反射扩展类的底层基础设施。

---

## 1. TypeMemberExtension

**位置**：`src/AtomUI.Core/Reflection/TypeMemberExtension.cs`  
**命名空间**：`AtomUI.Reflection`  
**可见性**：`public static class`

提供基于 `Type` 的成员查找方法，分为两类：`TryGet` 系列（安全查找）和 `OrThrow` 系列（快速失败）。

### 1.1 TryGet 系列

尝试查找成员，返回 `bool` 表示是否成功，通过 `out` 参数返回结果。

```csharp
// 查找属性
bool TryGetPropertyInfo(this Type type, string name, out PropertyInfo? info, BindingFlags flags = ...)

// 查找字段
bool TryGetFieldInfo(this Type type, string name, out FieldInfo? info, BindingFlags flags = ...)

// 查找方法
bool TryGetMethodInfo(this Type type, string name, out MethodInfo? info, BindingFlags flags = ...)

// 查找事件
bool TryGetEventInfo(this Type type, string name, out EventInfo? info, BindingFlags flags = ...)
```

**使用场景**：当成员可能不存在时（如兼容多版本 Avalonia 的可选功能）。

### 1.2 OrThrow 系列

查找成员，找不到时抛出描述性异常。**这是反射扩展类中的首选方法**。

```csharp
// 查找属性，失败抛 MissingMemberException
PropertyInfo GetPropertyInfoOrThrow(this Type type, string name, BindingFlags flags = ...)

// 查找字段，失败抛 MissingFieldException
FieldInfo GetFieldInfoOrThrow(this Type type, string name, BindingFlags flags = ...)

// 查找方法，失败抛 MissingMethodException
MethodInfo GetMethodInfoOrThrow(this Type type, string name, BindingFlags flags = ...)

// 查找事件，失败抛 MissingMemberException
EventInfo GetEventInfoOrThrow(this Type type, string name, BindingFlags flags = ...)
```

**使用场景**：反射扩展类中所有 `Lazy<XxxInfo>` 的初始化。

### 1.3 默认 BindingFlags

所有方法的默认 `BindingFlags` 均为：

```csharp
BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy
```

这意味着默认会搜索实例成员、公私有成员、以及继承链上的成员。如需精确控制，可显式传入 flags。

---

## 2. ObjectExtension

**位置**：`src/AtomUI.Core/Reflection/ObjectExtension.cs`  
**命名空间**：`AtomUI.Reflection`  
**可见性**：`public static class`

提供基于对象实例的反射操作方法，支持直接在对象上进行属性、字段的读写和方法调用。

### 2.1 属性操作

```csharp
// 安全读取
bool TryGetProperty<T>(this object source, string name, out T? result, BindingFlags flags = ...)
bool TryGetProperty<T>(this object source, Type declareType, string name, out T? result, BindingFlags flags = ...)
bool TryGetProperty<T>(this object source, PropertyInfo info, out T? result)

// 快速失败读取
T? GetPropertyOrThrow<T>(this object source, string name, BindingFlags flags = ...)
T? GetPropertyOrThrow<T>(this object source, Type declareType, string name, BindingFlags flags = ...)
T? GetPropertyOrThrow<T>(this object source, PropertyInfo info)

// 安全写入
bool TrySetProperty<T>(this object source, Type declareType, string name, T? value, BindingFlags flags = ...)
```

### 2.2 字段操作

```csharp
// 安全读取
bool TryGetField<T>(this object source, string name, out T? result, BindingFlags flags = ...)
bool TryGetField<T>(this object source, Type declareType, string name, out T? result, BindingFlags flags = ...)
bool TryGetField<T>(this object source, FieldInfo info, out T? result)

// 快速失败读取
T? GetFieldOrThrow<T>(this object source, string name, BindingFlags flags = ...)
T? GetFieldOrThrow<T>(this object source, Type declareType, string name, BindingFlags flags = ...)

// 安全写入
bool TrySetField<T>(this object source, Type declareType, string name, T? value, BindingFlags flags = ...)
```

### 2.3 方法调用

```csharp
// 安全调用
bool TryInvokeMethod(this object source, Type declareType, string name, out object? result, params object[] parameters)

// 快速失败调用
object? InvokeMethodOrThrow(this object source, Type declareType, string name, params object[] parameters)
```

### 2.4 使用建议

`ObjectExtension` 中的方法适用于 **一次性的、临时的** 反射操作。对于需要反复调用的反射访问（即反射扩展类中的场景），**应使用 `TypeMemberExtension` 获取 `XxxInfo` 并缓存到 `Lazy<T>` 字段中**，然后通过 `XxxInfo.GetValue()` / `XxxInfo.SetValue()` / `XxxInfo.Invoke()` 直接操作，以获得更好的性能。

---

## 3. 方法选择决策树

```
需要反射访问 Avalonia 内部成员？
├─ 是否会被多次调用（如控件生命周期内反复触发）？
│  ├─ 是 → 创建 XxxReflectionExtensions 类
│  │        使用 TypeMemberExtension.GetXxxInfoOrThrow 获取反射信息
│  │        缓存到 Lazy<XxxInfo> 静态字段
│  │        封装为扩展方法
│  └─ 否 → 是否是一次性初始化操作？
│     ├─ 是 → 可使用 ObjectExtension.GetPropertyOrThrow<T> 等
│     │        但仍建议封装到扩展类中以保持一致性
│     └─ 否 → 封装到 XxxReflectionExtensions 扩展类
└─ 所有情况都必须标注 [DynamicDependency]
```

