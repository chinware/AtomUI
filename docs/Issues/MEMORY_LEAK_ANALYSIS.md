# AtomUI 内存泄漏风险分析报告

**扫描日期**: 2026-04-20  
**扫描范围**: `/Users/chinboy/Projects/dotnet/AtomUI/src`  
**扫描目标**: 使用 object/AvaloniaObject 作为哈希表 key 的情况

---

## 📊 风险等级汇总

| 文件 | 类 | 风险等级 | 问题类型 |
|------|-----|--------|--------|
| IconProviderCache.cs | IconProviderCache | 🔴 高 | 全局静态缓存，枚举值作为key |
| ListBox.cs | ListBox | 🔴 高 | ListBoxItem对象作为key |
| CompactSpace.cs | CompactSpace | 🟡 中 | Control对象作为key |
| SplitterPanel.cs | SplitterPanel | 🟡 中 | Control对象作为key |
| VirtualizingCarouselPanel.cs | VirtualizingCarouselPanel | 🟡 中 | 回收池key生命周期 |
| BaseTabControl.cs | BaseTabControl | 🟡 中 | TabItem对象作为key |
| ListView.cs | ListView | 🟢 低 | int作为key（安全） |
| CascaderViewLevelList.cs | CascaderViewLevelList | 🟢 低 | int作为key（安全） |

---

## 🔴 高风险问题详情

### 1. IconProviderCache.cs - 全局静态缓存泄漏

**文件路径**: `src/AtomUI.Core/Controls/Icon/IconProviderCache.cs`

**问题代码**:
```csharp
// 第12-16行
private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, Type>> TypeCache
    = new();

private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, Func<Icon>>> CreatorCache
    = new();
```

**使用位置**:
- **第41行**: `cache.GetOrAdd(enumValue, typeFactory)` - 直接使用枚举值作为key
- **第52行**: `cache.GetOrAdd(enumValue, creatorFactory)` - 同样使用枚举值

**风险分析**:
- ✗ 全局静态缓存，生命周期与应用程序相同
- ✗ 虽然有 `MaxCacheSize = 256` 限制，但 FIFO 清理策略可能不够高效
- ✗ 枚举值虽然实现了 GetHashCode/Equals，但缓存无限增长风险

**缓存清理逻辑** (第60-70行):
```csharp
private static void TrimCache<TKey, TValue>(
    ConcurrentDictionary<TKey, TValue> cache,
    int maxSize)
{
    if (cache.Count > maxSize)
    {
        var keysToRemove = cache.Keys.Take(cache.Count - maxSize).ToList();
        foreach (var key in keysToRemove)
        {
            cache.TryRemove(key, out _);
        }
    }
}
```

**修复建议**:
1. 考虑使用 `WeakReference<T>` 包装缓存值
2. 或改进缓存策略为 LRU（最近最少使用）而非 FIFO
3. 添加缓存命中率监控

---

### 2. ListBox.cs - ListBoxItem 对象作为 Key

**文件路径**: `src/AtomUI.Desktop.Controls/ListBox/ListBox.cs`

**问题代码**:
```csharp
// 第231行
private Dictionary<object, bool> _filterContext = new();

// 第232行
private Dictionary<object, IDictionary<object, object?>> _virtualRestoreContext = new();
```

**使用位置**:
- **第441行**: `_filterContext[listBoxItem] = listBoxItem.IsVisible` - ListBoxItem 对象作为 key
- **第572行**: `_virtualRestoreContext.Add(listItem.VirtualIndex, context)` - VirtualIndex(int) 作为 key

**风险分析**:
- 🔴 `_filterContext` 使用 ListBoxItem 对象作为 key
- ListBoxItem 继承自 Control → AvaloniaObject
- 虚拟化场景下，ListBoxItem 可能被回收但仍在字典中
- ListBoxItem 未重写 GetHashCode/Equals，使用默认引用相等性

**清理逻辑**:
- 第 441 行添加到 `_filterContext`
- **缺少对应的清理代码** - 当 ListBoxItem 被移除时，`_filterContext` 中的条目未被删除

**修复建议**:
1. 改用 `int` 索引（如 VirtualIndex）替代 ListBoxItem 对象
2. 或在 ListBoxItem 移除时显式调用 `_filterContext.Remove(listBoxItem)`
3. 建议改为: `Dictionary<int, bool> _filterContext` 使用 VirtualIndex

---

## 🟡 中风险问题详情

### 3. CompactSpace.cs - Control 对象作为 Key

**文件路径**: `src/AtomUI.Desktop.Controls/Space/CompactSpace.cs`

**问题代码**:
```csharp
// 第91行
private Dictionary<object, NotifyCollectionChangedEventHandler> _childClassesChangedHandlers = new();
```

**使用位置**:
- **第205行**: `_childClassesChangedHandlers.Add(target, childClassesChangedHandler)` - target 是 Control 对象
- **第234-237行** (清理逻辑):
```csharp
if (_childClassesChangedHandlers.TryGetValue(target, out var handler))
{
    target.Classes.CollectionChanged -= handler;
    _childClassesChangedHandlers.Remove(target);
}
```

**风险分析**:
- 🟡 Control 对象作为 key（继承自 AvaloniaObject）
- ✓ 有正确的 Remove 清理逻辑
- ⚠ 需要验证所有 Control 移除时都调用了清理代码

**验证清理的调用点**:
- 第 234-237 行: `OnChildClassesChanged` 中的清理
- 需要检查是否所有移除 target 的地方都调用了清理

**修复建议**:
1. 添加日志记录，验证所有 Control 都被正确清理
2. 考虑使用 WeakDictionary 或 ConditionalWeakTable
3. 在 OnDetachedFromVisualTree 中添加清理逻辑

---

### 4. SplitterPanel.cs - Control 对象作为 Key

**文件路径**: `src/AtomUI.Desktop.Controls/Splitter/SplitterPanel.cs`

**问题代码**:
```csharp
// 第79行
private Dictionary<Control, SplitterPartContext> _partContexts = new();
```

**使用位置**:
- **第127行**: `_partContexts[panel] = new SplitterPartContext()` - 添加 Control 作为 key
- **第116行** (清理逻辑):
```csharp
_partContexts.Clear();
```

**风险分析**:
- 🟡 Control 对象作为 key
- ⚠ 只有 `Clear()` 清理，没有针对单个 Control 的移除逻辑
- 长期持有 Control 引用可能导致泄漏

**清理逻辑缺陷**:
- 只在 Reset() 时调用 Clear()
- 当单个 panel 被移除时，`_partContexts` 中的条目未被删除

**修复建议**:
1. 在 panel 移除时添加 `_partContexts.Remove(panel)` 逻辑
2. 在 OnChildrenChanged 或类似事件中检测移除并清理
3. 改用 int 索引（如 Children.IndexOf(panel)）替代 Control 对象

---

### 5. VirtualizingCarouselPanel.cs - 回收池 Key 生命周期

**文件路径**: `src/AtomUI.Desktop.Controls/Carousel/VirtualizingCarouselPanel.cs`

**问题代码**:
```csharp
// 第21行
private Dictionary<object, Stack<Control>>? _recyclePool;
```

**使用位置**:
- **第381行**: `_recyclePool.Add(recycleKey, pool)` - recycleKey 是 object
- **第368-375行** (回收逻辑):
```csharp
if (_recyclePool == null)
{
    _recyclePool = new Dictionary<object, Stack<Control>>();
}

var recycleKey = GetRecycleKey(item);
if (!_recyclePool.ContainsKey(recycleKey))
{
    _recyclePool.Add(recycleKey, pool);
}
```

**风险分析**:
- 🟡 回收池中的 key 可能是任意对象
- ⚠ 需要检查 `GetRecycleKey()` 返回的对象类型
- 如果 key 对象被 GC 但仍在字典中会导致泄漏

**需要检查**:
- `GetRecycleKey(item)` 的实现（第 ? 行）
- recycleKey 对象的生命周期管理
- 回收池的清理时机

**修复建议**:
1. 确保 recycleKey 是不可变的值类型（如 int、string）
2. 或使用 WeakReference 包装 key
3. 添加回收池大小限制和定期清理

---

### 6. BaseTabControl.cs - TabItem 对象作为 Key

**文件路径**: `src/AtomUI.Desktop.Controls/TabControl/BaseTabControl.cs`

**问题代码**:
```csharp
// 第265行
private Dictionary<TabItem, CompositeDisposable> ItemsBindingDisposables = new();
```

**使用位置**:
- **第545行**: `ItemsBindingDisposables.Add(tabItem, disposables)` - 添加 TabItem 作为 key
- **第381-384行** (清理逻辑):
```csharp
if (ItemsBindingDisposables.TryGetValue(tabItem, out var disposables))
{
    disposables.Dispose();
    ItemsBindingDisposables.Remove(tabItem);
}
```

**风险分析**:
- 🟡 TabItem 继承自 Control → AvaloniaObject
- ✓ 有 TryGetValue + Remove 的清理逻辑
- ⚠ 需要验证所有 TabItem 移除时都调用了清理

**清理调用点**:
- 第 381-384 行: 在某个事件处理中
- 需要确认这个清理逻辑在所有 TabItem 移除场景中都被调用

**修复建议**:
1. 添加日志验证清理完整性
2. 在 OnItemsChanged 中添加显式清理逻辑
3. 考虑使用 WeakDictionary

---

## 🟢 低风险问题详情

### 7. ListView.cs - 使用 int 作为 Key（安全）

**文件路径**: `src/AtomUI.Desktop.Controls/ListView/ListView.Virtualizing.cs`

**代码**:
```csharp
// 第10行
private Dictionary<object, IDictionary<object, object?>> _virtualRestoreContext = new();

// 第28行
_virtualRestoreContext.Add(listItem.VirtualIndex, context);
```

**风险分析**:
- ✓ 使用 `int` (VirtualIndex) 作为 key，相对安全
- ✓ int 是值类型，不会导致引用泄漏
- ✓ 虚拟化场景下，VirtualIndex 是稳定的标识符

**结论**: 此处相对安全，无需修改

---

### 8. CascaderViewLevelList.cs - 使用 int 作为 Key（安全）

**文件路径**: `src/AtomUI.Desktop.Controls/Cascader/CascaderViewLevelList.cs`

**代码**:
```csharp
// 第48行
private Dictionary<object, IDictionary<object, object?>> _virtualRestoreContext = new();

// 第232行
_virtualRestoreContext.Add(listItem.VirtualIndex, context);
```

**风险分析**:
- ✓ 使用 `int` (VirtualIndex) 作为 key，相对安全
- ✓ 虚拟化场景下的标准做法

**结论**: 此处相对安全，无需修改

---

## 📋 修复优先级建议

### 第一优先级（立即修复）

1. **ListBox._filterContext** (高风险)
   - 改用 `Dictionary<int, bool>` 使用 VirtualIndex
   - 预计改动: 5-10 行代码
   - 影响范围: ListBox 虚拟化逻辑

2. **IconProviderCache** (高风险)
   - 改进缓存策略为 LRU
   - 或使用 WeakReference
   - 预计改动: 20-30 行代码

### 第二优先级（近期修复）

3. **SplitterPanel._partContexts** (中风险)
   - 添加单个 panel 移除时的清理逻辑
   - 预计改动: 10-15 行代码

4. **CompactSpace._childClassesChangedHandlers** (中风险)
   - 验证清理完整性，添加日志
   - 预计改动: 5-10 行代码

5. **BaseTabControl.ItemsBindingDisposables** (中风险)
   - 验证清理完整性
   - 预计改动: 5-10 行代码

### 第三优先级（后续优化）

6. **VirtualizingCarouselPanel._recyclePool** (中风险)
   - 检查 GetRecycleKey() 实现
   - 添加回收池大小限制

---

## 🔍 排查步骤

### 对于每个问题，按以下步骤排查：

1. **定位代码**
   - 打开指定文件和行号
   - 查看问题代码片段

2. **理解风险**
   - 阅读"风险分析"部分
   - 理解为什么这是问题

3. **验证清理逻辑**
   - 搜索所有使用该字典的地方
   - 检查是否有对应的 Remove/Clear 逻辑

4. **实施修复**
   - 按照"修复建议"进行改动
   - 添加单元测试验证

5. **验证效果**
   - 运行内存分析工具
   - 检查是否解决泄漏

---

## 📝 检查清单

- [ ] 检查 IconProviderCache 缓存策略
- [ ] 修改 ListBox._filterContext 使用 int key
- [ ] 验证 SplitterPanel 清理逻辑
- [ ] 验证 CompactSpace 清理逻辑
- [ ] 验证 BaseTabControl 清理逻辑
- [ ] 检查 VirtualizingCarouselPanel 的 GetRecycleKey()
- [ ] 添加内存泄漏单元测试
- [ ] 运行内存分析工具验证修复

---

## 📚 相关资源

- Avalonia AvaloniaObject 文档
- .NET 内存泄漏诊断工具
- WeakReference 和 ConditionalWeakTable 用法
