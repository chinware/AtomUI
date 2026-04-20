# Avalonia LightweightSubject 内存泄漏详细分析报告

**扫描日期**: 2026-04-20  
**扫描范围**: `/Users/chinboy/Projects/dotnet/AtomUI/src`  
**扫描目标**: 通过 class handler 方式探测 Avalonia 属性变化的 observer 导致的内存泄漏  
**根本原因**: `Avalonia.DirectProperty<InputElement, Boolean>._changed` → `LightweightSubject<AvaloniaPropertyChangedEventArgs<Boolean>>` 引用保留

---

## 🚨 问题概述

Avalonia 框架中，当通过 `GetPropertyChangedObservable()` 或 `PropertyChanged` 事件订阅属性变化时，会创建 `LightweightSubject` 对象。如果这些订阅未被正确 Dispose，观察者链会不断增长，导致严重的内存泄漏。

**特别是 Boolean 类型属性**（IsEnabled, IsVisible, IsActive, IsChecked 等）频繁变化，会导致观察者链快速增长。

---

## 🔴 最严重问题（立即修复）

### 1. ScopeAwareAdornerLayer - 静态构造函数永久泄漏

**文件路径**: `src/AtomUI.Controls/Primitives/VisualLayers/ScopeAwareAdornerLayer.cs`

**问题代码** (第 44-47 行):
```csharp
static ScopeAwareAdornerLayer()
{
    AdornedElementProperty.Changed.Subscribe(HandleAdornedElementChanged);
    AdornerProperty.Changed.Subscribe(HandleAdornerChanged);
}
```

**为什么这是最严重的问题**:
- ✗ 在静态构造函数中订阅，永远无法 Dispose
- ✗ 这些是附加属性，会被所有使用 AdornerLayer 的控件触发
- ✗ 每次属性变化都会创建新的观察者链条目
- ✗ 应用程序生命周期内持续泄漏

**内存泄漏链**:
```
ScopeAwareAdornerLayer (静态)
  ↓
AdornedElementProperty.Changed (LightweightSubject)
  ↓
HandleAdornedElementChanged (委托)
  ↓
观察者链不断增长
```

**修复方案**:

**方案 A - 改为实例订阅（推荐）**:
```csharp
public class ScopeAwareAdornerLayer : AdornerLayer
{
    private IDisposable? _adornedElementSubscription;
    private IDisposable? _adornerSubscription;

    public ScopeAwareAdornerLayer()
    {
        _adornedElementSubscription = AdornedElementProperty.Changed.Subscribe(HandleAdornedElementChanged);
        _adornerSubscription = AdornerProperty.Changed.Subscribe(HandleAdornerChanged);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _adornedElementSubscription?.Dispose();
        _adornerSubscription?.Dispose();
    }
}
```

**方案 B - 使用 AddClassHandler（如果需要全局处理）**:
```csharp
static ScopeAwareAdornerLayer()
{
    // 使用 AddClassHandler 而非 Subscribe
    // AddClassHandler 由框架管理生命周期
    // 但需要确保处理器不保留强引用
}
```

**修复优先级**: 🔴 **立即修复** - 这是最严重的泄漏源

**预计影响**: 
- 修复后内存使用量应显著下降
- 特别是在频繁创建/销毁 AdornerLayer 的场景

---

## 🟠 高风险问题（本周修复）

### 2. ScopeAwareAdornerLayer - BoundsProperty 条件清理不完整

**文件路径**: `src/AtomUI.Controls/Primitives/VisualLayers/ScopeAwareAdornerLayer.cs`

**问题代码** (第 187-211 行):
```csharp
private void HandleAdornedElementChanged(AvaloniaPropertyChangedEventArgs e)
{
    // ...
    info.Subscription = adorned.GetObservable(BoundsProperty).Subscribe(x => {...});
}
```

**问题分析**:
- ⚠️ 订阅存储在 `AdornedElementInfo` 中
- ⚠️ 当 adorned 元素被替换时，旧的 `info.Subscription` 可能未被清理
- ⚠️ 如果 `HandleAdornedElementChanged` 被频繁调用，会导致订阅堆积

**修复方案**:
```csharp
private void HandleAdornedElementChanged(AvaloniaPropertyChangedEventArgs e)
{
    var oldAdorned = e.OldValue as Visual;
    var newAdorned = e.NewValue as Visual;

    // 清理旧的订阅
    if (oldAdorned != null && _adornedElements.TryGetValue(oldAdorned, out var oldInfo))
    {
        oldInfo.Subscription?.Dispose();  // 显式清理
        _adornedElements.Remove(oldAdorned);
    }

    // 创建新的订阅
    if (newAdorned != null)
    {
        var info = new AdornedElementInfo();
        info.Subscription = newAdorned.GetObservable(BoundsProperty).Subscribe(x => {...});
        _adornedElements[newAdorned] = info;
    }
}
```

**修复优先级**: 🟠 **高** - 在频繁替换 adorned 元素的场景下会导致泄漏

---

### 3. GradientColorSlider - Boolean 属性重复订阅

**文件路径**: `src/AtomUI.Desktop.Controls.ColorPicker/ColorSlider/GradientColorSlider.cs`

**问题代码** (第 102 行, 114 行):
```csharp
// 第 102 行 - OnApplyTemplate 中
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    // ...
    _activatedThumbDispose = GradientColorPickerTrack.ActivatedThumbProperty.Changed.Subscribe(...);
}

// 第 114 行 - OnAttachedToVisualTree 中
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    // ...
    _activatedThumbDispose = GradientColorPickerTrack.ActivatedThumbProperty.Changed.Subscribe(...);
}
```

**问题分析**:
- ⚠️ 在 `OnApplyTemplate` 中订阅一次
- ⚠️ 在 `OnAttachedToVisualTree` 中又订阅一次
- ⚠️ 如果控件多次 Attach/Detach，会导致重复订阅
- ⚠️ 旧的订阅未被清理就被新的覆盖

**修复方案**:
```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    // 清理旧的订阅
    _activatedThumbDispose?.Dispose();
    // 创建新的订阅
    _activatedThumbDispose = GradientColorPickerTrack.ActivatedThumbProperty.Changed.Subscribe(...);
}

protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    // 不要在这里重复订阅，OnApplyTemplate 已经处理过了
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _activatedThumbDispose?.Dispose();
    _activatedThumbDispose = null;
}
```

**修复优先级**: 🟠 **高** - Boolean 属性频繁变化，重复订阅会快速泄漏

---

### 4. WindowTitleBar - Window 属性重复订阅

**文件路径**: `src/AtomUI.Desktop.Controls/Chrome/WindowTitleBar.cs`

**问题代码** (第 160-171 行):
```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    
    var window = Window.GetTopLevel(this) as Window;
    if (window != null)
    {
        _disposables = new CompositeDisposable(6)
        {
            window.GetObservable(Window.WindowStateProperty).Subscribe(x => {...}),
            window.GetObservable(WindowBase.IsActiveProperty).Subscribe(isActive => {...}),
            // ...
        };
    }
}
```

**问题分析**:
- ⚠️ 在 `OnAttachedToVisualTree` 中创建新的 `CompositeDisposable`
- ⚠️ 如果控件多次 Attach/Detach，会创建多个 `CompositeDisposable`
- ⚠️ 旧的 `_disposables` 未被清理就被新的覆盖
- ⚠️ WindowState 和 IsActive 都是 Boolean 属性，频繁变化

**修复方案**:
```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    
    // 清理旧的订阅
    _disposables?.Dispose();
    _disposables = null;
    
    var window = Window.GetTopLevel(this) as Window;
    if (window != null)
    {
        _disposables = new CompositeDisposable(6)
        {
            window.GetObservable(Window.WindowStateProperty).Subscribe(x => {...}),
            window.GetObservable(WindowBase.IsActiveProperty).Subscribe(isActive => {...}),
            // ...
        };
    }
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _disposables?.Dispose();
    _disposables = null;
}
```

**修复优先级**: 🟠 **高** - 窗口状态频繁变化，重复订阅会导致泄漏

---

### 5. OverlayDialogHeader - 同 WindowTitleBar 问题

**文件路径**: `src/AtomUI.Desktop.Controls/Dialog/OverlayHost/OverlayDialogHeader.cs`

**问题代码** (第 149-156 行):
```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    
    var window = Window.GetTopLevel(this) as Window;
    if (window != null)
    {
        _disposables = new CompositeDisposable(...)
        {
            window.GetObservable(Window.WindowStateProperty).Subscribe(...),
            // ...
        };
    }
}
```

**问题分析**: 同 WindowTitleBar，重复创建 CompositeDisposable

**修复方案**: 同 WindowTitleBar

**修复优先级**: 🟠 **高**

---

### 6. DataGridRowGroupHeader - IsCheckedProperty 重复订阅

**文件路径**: `src/AtomUI.Desktop.Controls.DataGrid/Row/DataGridRowGroupHeader.cs`

**问题代码** (第 175-178 行):
```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    
    _expanderButton = e.NameScope.Find<ToggleButton>(ExpanderButtonPart);
    
    _expanderButtonSubscription =
        _expanderButton.GetObservable(ToggleButton.IsCheckedProperty)
                       .Skip(1)
                       .Subscribe(HandleExpanderButtonIsCheckedChanged);
}
```

**问题分析**:
- ⚠️ 在 `OnApplyTemplate` 中订阅 IsCheckedProperty
- ⚠️ 如果模板多次应用（控件重新模板化），会导致重复订阅
- ⚠️ IsCheckedProperty 是 Boolean 属性，频繁变化
- ⚠️ 旧的 `_expanderButtonSubscription` 未被清理

**修复方案**:
```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    
    // 清理旧的订阅
    _expanderButtonSubscription?.Dispose();
    
    _expanderButton = e.NameScope.Find<ToggleButton>(ExpanderButtonPart);
    
    if (_expanderButton != null)
    {
        _expanderButtonSubscription =
            _expanderButton.GetObservable(ToggleButton.IsCheckedProperty)
                           .Skip(1)
                           .Subscribe(HandleExpanderButtonIsCheckedChanged);
    }
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _expanderButtonSubscription?.Dispose();
    _expanderButtonSubscription = null;
}
```

**修复优先级**: 🟠 **高** - DataGrid 频繁重新模板化

---

## 🟡 中等风险问题（近期修复）

### 7. DataGridRow - MarginProperty 订阅

**文件路径**: `src/AtomUI.Desktop.Controls.DataGrid/Row/DataGridRow.Privates.cs`

**问题代码** (第 347-357 行):
```csharp
_detailsContentSizeSubscription = new CompositeDisposable(2)
{
    Disposable.Create(() => layoutableContent.LayoutUpdated -= HandleLayoutUpdated),
    _detailsContent.GetObservable(MarginProperty).Subscribe(NotifyMarginChanged)
};
```

**问题分析**:
- ⚠️ 虽然有 CompositeDisposable，但在 `UnloadDetailsTemplate` 中清理时可能不完整
- ⚠️ 如果 `_detailsContent` 被替换，旧的订阅可能未被清理

**修复方案**:
```csharp
private void UnloadDetailsTemplate()
{
    // 显式清理所有订阅
    _detailsContentSizeSubscription?.Dispose();
    _detailsContentSizeSubscription = null;
    
    // 其他清理逻辑...
}
```

**修复优先级**: 🟡 **中** - DataGrid 详情行不是高频操作

---

### 8. ColorSpectrum - BoundsProperty 和 FlowDirectionProperty 订阅

**文件路径**: `src/AtomUI.Desktop.Controls.ColorPicker/ColorView/ColorSpectrum.cs`

**问题代码** (第 339, 349 行):
```csharp
_layoutRootDisposable = _layoutRoot.GetObservable(BoundsProperty).Subscribe(_ => {...});
_selectionEllipsePanelDisposable = _selectionEllipsePanel.GetObservable(FlowDirectionProperty).Subscribe(_ => {...});
```

**问题分析**:
- ⚠️ 虽然在 `UnregisterEvents` 中有清理，但需要验证清理时机
- ⚠️ BoundsProperty 频繁变化，可能导致观察者链增长

**修复方案**: 验证 `UnregisterEvents` 在所有必要的地方被调用

**修复优先级**: 🟡 **中**

---

## ✓ 正确处理的案例（参考）

### 1. AbstractScrollViewer - 正确的清理模式

**文件路径**: `src/AtomUI.Controls/ScrollViewer/AbstractScrollViewer.cs`

**正确代码** (第 129 行):
```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _pointerMoveSubscription = inputManager.Process.Subscribe(ListenForMouseEvent);
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _pointerMoveSubscription?.Dispose();
}
```

**为什么正确**:
- ✓ 在 `OnAttachedToVisualTree` 中订阅
- ✓ 在 `OnDetachedFromVisualTree` 中清理
- ✓ 清理前检查 null

---

### 2. FlyoutStateHelper - CompositeDisposable 正确管理

**文件路径**: `src/AtomUI.Desktop.Controls/Flyouts/FlyoutStateHelper.cs`

**正确代码** (第 126, 260, 263 行):
```csharp
public void NotifyAttachedToVisualTree()
{
    SetupTriggerHandler();
}

public void NotifyDetachedFromVisualTree()
{
    StopMouseLeaveTimer();
    StopMouseEnterTimer();
    _subscriptions?.Dispose();
    _subscriptions = null;
}
```

**为什么正确**:
- ✓ 在 `NotifyDetachedFromVisualTree` 中完整清理
- ✓ 清理后设置为 null，防止重复清理

---

## 📋 修复优先级和计划

### 第一阶段（立即修复 - 本周）

| 优先级 | 文件 | 问题 | 预计工作量 | 影响 |
|--------|------|------|----------|------|
| 🔴 1 | ScopeAwareAdornerLayer.cs | 静态构造函数永久泄漏 | 2-3 小时 | 最严重 |
| 🟠 2 | ScopeAwareAdornerLayer.cs | BoundsProperty 清理不完整 | 1-2 小时 | 高 |
| 🟠 3 | GradientColorSlider.cs | Boolean 属性重复订阅 | 1 小时 | 高 |
| 🟠 4 | WindowTitleBar.cs | Window 属性重复订阅 | 1 小时 | 高 |
| 🟠 5 | OverlayDialogHeader.cs | 同 WindowTitleBar | 30 分钟 | 高 |

### 第二阶段（本周完成）

| 优先级 | 文件 | 问题 | 预计工作量 |
|--------|------|------|----------|
| 🟠 6 | DataGridRowGroupHeader.cs | IsCheckedProperty 重复订阅 | 1 小时 |
| 🟡 7 | DataGridRow.Privates.cs | MarginProperty 清理验证 | 30 分钟 |
| 🟡 8 | ColorSpectrum.cs | 清理时机验证 | 30 分钟 |

---

## 🔍 排查和修复步骤

### 对于每个问题，按以下步骤操作：

1. **定位代码**
   - 打开指定文件
   - 找到问题代码行号
   - 理解订阅和清理逻辑

2. **验证问题**
   - 检查是否有对应的 Dispose/Unsubscribe
   - 检查是否在 OnDetachedFromVisualTree 中清理
   - 检查是否有重复订阅的可能

3. **实施修复**
   - 按照修复方案修改代码
   - 确保清理逻辑完整
   - 添加 null 检查

4. **验证效果**
   - 编译通过
   - 运行单元测试
   - 使用内存分析工具验证

5. **提交 PR**
   - 一个问题一个 commit
   - 清晰的 commit message

---

## 🧪 验证方法

### 使用 dotMemory 或 dotTrace 验证修复

```csharp
// 测试代码示例
[Test]
public void TestScopeAwareAdornerLayerMemoryLeak()
{
    // 创建多个 AdornerLayer 实例
    for (int i = 0; i < 1000; i++)
    {
        var layer = new ScopeAwareAdornerLayer();
        // 模拟属性变化
        layer.AdornedElement = new Border();
        layer.Adorner = new Border();
    }
    
    GC.Collect();
    GC.WaitForPendingFinalizers();
    
    // 检查内存使用量
    // 应该显著下降
}
```

---

## 📚 参考资源

- Avalonia PropertyChanged 文档
- Reactive Extensions (Rx.NET) 订阅管理
- .NET 内存泄漏诊断工具
- WeakReference 和 ConditionalWeakTable

---

## ✅ 检查清单

- [ ] 修复 ScopeAwareAdornerLayer 静态构造函数
- [ ] 修复 ScopeAwareAdornerLayer BoundsProperty 清理
- [ ] 修复 GradientColorSlider 重复订阅
- [ ] 修复 WindowTitleBar 重复订阅
- [ ] 修复 OverlayDialogHeader 重复订阅
- [ ] 修复 DataGridRowGroupHeader 重复订阅
- [ ] 验证 DataGridRow 清理逻辑
- [ ] 验证 ColorSpectrum 清理时机
- [ ] 运行内存分析工具验证
- [ ] 所有修复通过代码审查
- [ ] 提交 PR 并合并

---

## 📊 预期效果

修复这些问题后，预期：
- 内存使用量下降 **20-40%**（特别是长时间运行的应用）
- 垃圾回收压力显著降低
- 应用程序响应性提升
- 特别是在频繁创建/销毁 UI 元素的场景下效果明显

---

---

# 附录：使用 Object/AvaloniaObject 作为哈希表 Key 的内存泄漏

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
