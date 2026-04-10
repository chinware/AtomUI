# AtomUI 集合相关类

## 概述

集合系统位于 `src/AtomUI.Core/Collections/` 目录，为 AtomUI 控件提供了高性能的数据集合基础设施。该系统解决了 UI 框架中集合管理的三个核心问题：

1. **数据源抽象** — 统一包装不同类型的集合为只读视图，支持变更通知
2. **内存效率** — 基于 `ArrayPool<T>` 的池化集合，减少 GC 压力
3. **内存安全** — 弱事件管理器，防止集合事件订阅导致的内存泄漏

### 在整体架构中的位置

```
UI 控件层（ComboBox, TreeView 等）
    ↓ 使用
Collection System（本文档 — 集合抽象与池化）
    ↓ 基于
.NET 集合框架 + System.Buffers.ArrayPool<T>
```

## 目录结构

```
Collections/
├── CollectionChangedEventManager.cs   # 弱事件管理器（核心）
├── ItemsSourceView.cs                 # 只读集合视图
├── ItemsSourceView{T}.cs             # 泛型集合视图
├── ItemCollection.cs                  # 可写双模式集合
└── Pooled/
    ├── IReadOnlyPooledList.cs         # 池化列表只读接口
    ├── PooledList.cs                  # 池化列表实现（核心）
    └── PooledStack.cs                 # 池化栈实现
```

## 核心组件

### 1. ItemsSourceView — 只读集合视图

`ItemsSourceView` 将任意 `IEnumerable` 包装为实现 `IReadOnlyList<object?>` 的只读视图，提供统一的集合访问接口和三阶段变更通知。

#### 三阶段变更通知

```csharp
// 变更前 → 变更中 → 变更后
PreCollectionChanged → CollectionChanged → PostCollectionChanged
```

- **PreCollectionChanged**：在集合实际变更前触发，允许控件做准备（如保存滚动位置）
- **CollectionChanged**：标准的 `NotifyCollectionChangedEventArgs` 通知
- **PostCollectionChanged**：在集合变更后触发，允许控件做后续处理（如恢复选中状态）

#### 弱事件订阅

通过 `CollectionChangedEventManager` 自动管理对源集合的 `CollectionChanged` 事件订阅，避免视图持有源集合的强引用导致内存泄漏。

#### 智能包装

```csharp
// 对 IList 直接索引访问
// 对非 IList 的 IEnumerable 自动物化为 List<object?>
public static ItemsSourceView GetOrCreate(IEnumerable items)
```

### 2. ItemsSourceView&lt;T&gt; — 泛型集合视图

继承 `ItemsSourceView`，增加类型安全的泛型访问：

```csharp
public class ItemsSourceView<T> : ItemsSourceView, IReadOnlyList<T>
{
    public new T this[int index] { get; }
}
```

### 3. ItemCollection — 可写双模式集合

`ItemCollection` 实现了一个**双模式集合**，支持在"直接项目模式"和"数据源绑定模式"之间切换：

#### 两种模式

| 模式 | 触发条件 | 行为 |
|------|----------|------|
| **Items 模式** | 默认或通过 `Add/Remove` 操作 | 集合可读可写，直接存储项目 |
| **ItemsSource 模式** | 设置 `ItemsSource` 属性 | 集合只读，代理到外部数据源 |

#### 模式互斥

```csharp
// 设置 ItemsSource 后，Add/Remove 等操作将抛出异常
collection.ItemsSource = someList;     // 切换到 ItemsSource 模式
collection.Add(item);                  // ← 抛出 InvalidOperationException
```

### 4. CollectionChangedEventManager — 弱事件管理器

这是集合系统的**内存安全核心**，采用单例模式管理所有 `CollectionChanged` 事件订阅。

#### 设计要点

- **弱引用**：使用 `WeakReference` 持有订阅者，不阻止 GC 回收
- **UI 线程调度**：自动将事件通知调度到 UI 线程
- **集中管理**：单例 `Current` 统一管理所有订阅的生命周期
- **自动清理**：当订阅者被 GC 回收后，自动移除对应的事件处理

```csharp
// 典型用法
CollectionChangedEventManager.Current.AddListener(source, handler);
CollectionChangedEventManager.Current.RemoveListener(source, handler);
```

## 池化集合（Pooled Collections）

### 5. PooledList&lt;T&gt; — 高性能池化列表

`PooledList<T>` 是对标准 `List<T>` 的高性能替代，核心思想是从 `ArrayPool<T>` 租借内部数组而非频繁分配。

#### 核心特性

| 特性 | 说明 |
|------|------|
| **ArrayPool 支持** | 内部数组从 `ArrayPool<T>.Shared` 租借，归还时返回池中 |
| **Span&lt;T&gt; 访问** | 提供 `Span` 属性实现零拷贝、零分配的数据访问 |
| **ClearMode** | `Auto`/`Always`/`Never` 控制归还数组时是否清零 |
| **15 种构造方式** | 从数组、Span、IEnumerable、容量预分配等多种初始化路径 |
| **struct Enumerator** | 值类型枚举器，避免装箱和 GC 分配 |
| **完整 List&lt;T&gt; API** | Add、Insert、Remove、Sort、BinarySearch、Find 等全部支持 |

#### ClearMode 策略

```csharp
public enum ClearMode
{
    Auto,    // 包含引用类型时清零，纯值类型不清零（默认）
    Always,  // 总是清零（安全优先）
    Never    // 从不清零（性能优先，仅用于值类型场景）
}
```

#### 使用模式

```csharp
// 推荐使用 using 确保归还数组
using var list = new PooledList<int>(capacity: 1024);
list.Add(42);
ReadOnlySpan<int> span = list.Span;  // 零拷贝访问
// Dispose 时自动归还数组到 ArrayPool
```

### 6. PooledStack&lt;T&gt; — 池化栈

基于 `ArrayPool<T>` 的栈实现，与 `PooledList<T>` 共享相同的内存管理策略：

- Push/Pop/Peek 标准栈操作
- `TryPop`、`TryPeek` 安全访问
- 同样支持 `ClearMode` 策略
- `Dispose` 时归还内部数组

### 7. IReadOnlyPooledList&lt;T&gt; — 池化列表只读接口

```csharp
public interface IReadOnlyPooledList<T> : IReadOnlyList<T>
{
    ReadOnlySpan<T> Span { get; }  // 零分配访问底层数据
}
```

提供只读访问契约，同时保留 `Span` 属性用于高性能场景。

## 设计模式

| 模式 | 应用 |
|------|------|
| **适配器模式** | `ItemsSourceView` 将不同集合类型适配为统一的只读接口 |
| **观察者模式** | 三阶段 `CollectionChanged` 通知机制 |
| **策略模式** | `ItemCollection` 的双模式（Items/ItemsSource）策略切换 |
| **单例模式** | `CollectionChangedEventManager.Current` 全局弱事件管理 |
| **对象池模式** | `PooledList<T>` / `PooledStack<T>` 基于 ArrayPool 的内存复用 |
| **RAII 模式** | 池化集合通过 `IDisposable` 确保资源归还 |

## 性能考量

1. **GC 压力**：池化集合显著减少大数组的频繁分配/回收，降低 Gen2 GC 频率
2. **零拷贝**：`Span<T>` 属性允许直接访问底层数组，无需复制
3. **值类型枚举器**：`struct Enumerator` 避免 `foreach` 循环中的装箱分配
4. **弱引用**：事件管理器使用弱引用，避免长生命周期对象持有短生命周期对象

## 相关文档

- [架构概览](./Architecture.md) — AtomUI.Core 整体架构
- [控件基础设施](./ControlInfrastructure.md) — 使用集合的控件基础
