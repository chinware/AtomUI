# Avalonia.Controls.DataGrid 11.3.13 vs 12.0.0 差异分析

## 分析范围

- 仓库：`.referenceprojects/Avalonia.Controls.DataGrid`
- 源码目录：`src/Avalonia.Controls.DataGrid`
- 对比基线：
  - **11 分支最新小版本**：`11.3.13`
  - **12 分支最新小版本**：`12.0.0`

> 说明：当前本地仓库中没有 `12.0.1` / `12.0.2` tag，因此 12 分支的“最新小版本”实际是 `12.0.0`。

## 总体结论

从 `11.3.13` 到 `12.0.0`，`Avalonia.Controls.DataGrid` 的改动重点不是新增功能，而是**适配 Avalonia 12 的绑定体系、焦点/平台 API、事件 API、测试基础设施和若干内部实现清理**。

整体特征：

1. **绑定体系重构**
   - `IBinding` 全部切到 `BindingBase`
   - 编辑绑定不再依赖自定义 `CellEditBinding + InstancedBinding` 包装层
   - 直接使用 `BindingExpressionBase` 和 `element.Bind(...)`

2. **校验机制简化**
   - 删除 DataGrid 内部维护的 `_bindingValidationErrors`、`_validationSubscription`
   - 改为依赖 Avalonia 12 的 `DataValidationErrors` / `BindingExpressionBase.UpdateSource()`

3. **Avalonia 12 API 适配**
   - `TopLevel.GetTopLevel(...).FocusManager`
   - `GetPlatformSettings()`
   - `ScrollGesture += ...`
   - `GetVisualParent()`
   - `LayoutHelper.RoundLayoutSizeUp(..., scale)`

4. **测试与样例同步升级**
   - 单元测试迁移到 **xUnit v3**
   - 移除 `dotMemory`
   - 示例项目移除 `Avalonia.Diagnostics`

## 关键提交

`11.3.13..12.0.0` 范围内与升级最相关的提交：

- `149348c` - Update to .net10 and .net8.
- `148a904` - Update to Avalonia 12 alpha.
- `b4c1032` - Re-enable data validation support.
- `9e03649` - Use xunit 3.
- `6eb30eb` - Rewrite LeakTests without dotMemory
- `cf6cbeb` - Fix tests due to subtle size changes
- `7a3a957` - Update to Avalonia 12.0.0-preview1
- `5074c15` - Remove usages of internal Avalonia APIs
- `7fd1e70` - Update Avalonia to 12.0.0-preview2

## 文件统计

`git diff --stat 11.3.13 12.0.0 -- src`

- 变更文件：`24`
- 新增：`189` 行
- 删除：`367` 行

高层上是一次**“删多于增”的适配型升级**。

## 重点改动分类

### 1. 工程与目标框架

文件：`src/Avalonia.Controls.DataGrid/Avalonia.Controls.DataGrid.csproj`

```diff
- <TargetFrameworks>$(AvsCurrentTargetFramework);$(AvsLegacyTargetFrameworks);netstandard2.0</TargetFrameworks>
+ <TargetFrameworks>$(AvsCurrentTargetFramework);$(AvsLegacyTargetFrameworks)</TargetFrameworks>
```

结论：

- **移除了 `netstandard2.0`** 目标
- DataGrid 不再维持旧的跨平台低版本兼容层，直接跟随 Avalonia 12 的目标框架策略

对 AtomUI 的意义：

- 如果你在 `release/6.0` 中复制 DataGrid 代码，不要再按 11.x 的 `netstandard2.0` 假设处理

### 2. 绑定类型：`IBinding` → `BindingBase`

文件：

- `DataGridBoundColumn.cs`
- `DataGridColumn.cs`

核心变化：

```diff
- private IBinding _binding;
+ private BindingBase _binding;

- public virtual IBinding Binding
+ public virtual BindingBase Binding

- public override IBinding ClipboardContentBinding
+ public override BindingBase ClipboardContentBinding
```

结论：

- Avalonia 12 中 DataGrid 已完全按 `BindingBase` 工作
- 代码同时显式区分：
  - `CompiledBinding`
  - `ReflectionBinding`

这点很重要，因为 11.x 时代代码常用 `Binding` / `IBinding` 混用；12.x 后需要明确处理不同绑定实现。

### 3. 编辑绑定重构：删除 `CellEditBinding`

文件：

- `DataGridBoundColumn.cs`
- `DataGridColumn.cs`
- `Utils/CellEditBinding.cs`（整文件删除）

11.x：

- `GenerateEditingElement(..., out ICellEditBinding editBinding)`
- 通过 `binding.Initiate(...)`
- 包一层 `CellEditBinding`
- 再用 `InstancedBinding` / `BindingOperations.Apply(...)`

12.x：

```diff
- protected sealed override Control GenerateEditingElement(..., out ICellEditBinding editBinding)
+ protected sealed override Control GenerateEditingElement(..., out BindingExpressionBase editBinding)

- editBinding = BindEditingElement(element, BindingTarget, Binding);
+ editBinding = element.Bind(BindingTarget, Binding);
```

结论：

- **自定义编辑绑定桥接层被彻底删除**
- 直接使用 Avalonia 12 自带的 `BindingExpressionBase`
- `InstancedBinding` 方案不再保留

对 AtomUI 的意义：

- 这是迁移 `DataGridBoundColumn`、`DataGridColumn` 时最关键的结构变化之一
- 如果代码里还保留 “`Initiate + InstancedBinding + custom subject wrapper`” 这套 11.x 逻辑，基本都应视为旧写法

### 4. 数据校验逻辑切换到 Avalonia 12 原生流

文件：`DataGrid.cs`

删除内容：

- `_bindingValidationErrors`
- `_validationSubscription`
- 手动 `CommitEdit()` 校验状态订阅
- 手动 `DataValidationErrors.SetError(...)`

12.x 新逻辑：

```diff
- void SetValidationStatus(ICellEditBinding binding)
+ void SetValidationStatus(BindingExpressionBase binding)

- if (editBinding != null && !editBinding.CommitEdit())
+ if (editBinding != null && DataValidationErrors.GetHasErrors(editingElement))
{
+    editBinding.UpdateSource();
```

结论：

- DataGrid 不再自己维护绑定错误列表
- 改成：
  - 看 `DataValidationErrors.GetHasErrors(...)`
  - 需要提交时调用 `BindingExpressionBase.UpdateSource()`

对 AtomUI 的意义：

- 如果 AtomUI DataGrid 仍保留 11.x 的手工校验桥接代码，迁移到 Avalonia 12 时可以参考这里的做法进行收敛

### 5. Focus / TopLevel API 变化

文件：`DataGrid.cs`

典型变化：

```diff
- Visual focusedObject = FocusManager.GetFocusManager(this)?.GetFocusedElement() as Visual;
+ var focusedObject = TopLevel.GetTopLevel(this)?.FocusManager.GetFocusedElement() as Visual;
```

以及：

```diff
- if (FocusManager.GetFocusManager(this)?.GetFocusedElement() is TextBox focusedTextBox
+ if (TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() is TextBox focusedTextBox
```

同时还简化了焦点离开 DataGrid 的判定逻辑：

```diff
- 手动沿 Visual/Parent 向上回溯
+ var focusLeftDataGrid = !this.IsVisualAncestorOf(focusedObject);
```

结论：

- DataGrid 在 12.0.0 中显式转向 `TopLevel` 驱动的焦点获取方式
- 旧的焦点树遍历逻辑被压缩

### 6. 内部/旧 API 使用被清理

#### 6.1 `PlatformSettings`

文件：`Utils/KeyboardHelper.cs`

```diff
- TopLevel.GetTopLevel(target)!.PlatformSettings!.HotkeyConfiguration;
+ TopLevel.GetTopLevel(target)!.GetPlatformSettings()!.HotkeyConfiguration;
```

#### 6.2 `Gestures.ScrollGestureEvent`

文件：`Primitives/DataGridRowsPresenter.cs`

```diff
- AddHandler(Gestures.ScrollGestureEvent, OnScrollGesture);
+ ScrollGesture += OnScrollGesture;
```

#### 6.3 `VisualParent`

文件：`Utils/TreeHelper.cs`

```diff
- parent = childElement.VisualParent;
+ parent = childElement.GetVisualParent();
```

#### 6.4 `RoundLayoutSizeUp`

文件：`DataGridColumn.cs`

```diff
- LayoutHelper.RoundLayoutSizeUp(new Size(...), scale, scale);
+ LayoutHelper.RoundLayoutSizeUp(new Size(...), scale);
```

#### 6.5 `StringBuilderCache`

文件：`DataGrid.cs`

```diff
- var text = StringBuilderCache.Acquire();
+ var text = new StringBuilder();
```

结论：

- 这些都是典型的 Avalonia 12 API/实现边界调整
- `5074c15 Remove usages of internal Avalonia APIs` 基本就对应这类改动

### 7. `OnDataContextBeginUpdate/EndUpdate` 清理

文件：`DataGrid.cs`

12.0.0 删除了：

- `OnDataContextBeginUpdate()`
- `OnDataContextEndUpdate()`
- `NotifyDataContextPropertyForAllRowCells(...)`

说明：

- 11.x 中 DataGrid 还会主动通知所有 cell 的 `DataContextProperty`
- 12.0.0 直接移除这套处理

这通常意味着：

- Avalonia 12 的绑定/数据上下文更新路径已经足以覆盖这里的场景
- DataGrid 不再需要保留自己的额外传播逻辑

### 8. 小型实现清理

#### 8.1 标志判断

文件：`Collections/DataGridCollectionView.cs`

```diff
- return _flags.HasAllFlags(flags);
+ return (_flags & flags) == flags;
```

#### 8.2 自建轻量工具替代 `Avalonia.Reactive`

新增文件：

- `Utils/ActionDisposable.cs`
- `Utils/AnonymousObserver.cs`
- `Utils/CompositeDisposable.cs`
- `Utils/ObservableExtensions.cs`
- `Utils/SkipObservable.cs`

对应变化：

- `DataGridRow.cs` 不再依赖 `Avalonia.Reactive`
- 改用仓库内部的极简 `Disposable/Observer` 实现

结论：

- 这更像依赖收敛和内部实现瘦身
- 不属于公开功能变化，但会影响你迁移时对辅助类的选型

## 测试与样例变化

### 1. 单元测试升级到 xUnit v3

文件：`src/Avalonia.Controls.DataGrid.UnitTests/Avalonia.Controls.DataGrid.UnitTests.csproj`

关键变化：

- 删除 `JetBrains.dotMemoryUnit`
- `xunit 2.x` → `xunit.v3`
- `xunit.runner.visualstudio` 升级
- 新增 `xunit.analyzers`

### 2. LeakTests 重写

文件：`LeakTests.cs`

关键变化：

- 从 dotMemory 专用测试改成普通 headless 异步测试
- 使用 `WeakReference<DataGrid>` + `GC.Collect()` 断言释放

结论：

- 12.0.0 的测试策略更轻量，不再依赖外部分析器

### 3. 行实现数量断言变化

文件：`DataGridRowTests.cs`

明显变化：

- `GetLastRealizedRowIndex` 期望从 `4` 变为 `3`
- `ScrollIntoView` 后首个可见行从 `6` 变为 `7`

结论：

- Avalonia 12 下布局/实现行数量存在**细微变化**
- 这类变化更像渲染/测量行为差异，不代表 DataGrid 逻辑重写

### 4. 测试绑定写法更新

文件：`DataGridRowTests.cs`

```diff
- new Binding("IsSelected", BindingMode.TwoWay)
+ new ReflectionBinding("IsSelected") { Mode = BindingMode.TwoWay }
```

结论：

- 在 12.0.0 的测试里，显式把旧 `Binding` 写法替换为 `ReflectionBinding`

### 5. 示例项目移除 Diagnostics

文件：`src/DataGridSample/DataGridSample.csproj`

```diff
- <PackageReference Include="Avalonia.Diagnostics" Version="$(AvaloniaVersion)" />
```

这是 Avalonia 12 周边依赖调整的一部分。

## 对 AtomUI DataGrid 迁移最有参考价值的点

如果你要把 AtomUI 的 DataGrid 从 Avalonia 11 迁到 Avalonia 12，这个仓库的 diff 最值得直接借鉴的是：

1. **绑定类型统一改成 `BindingBase`**
2. **编辑绑定改为 `BindingExpressionBase`**
3. **删除 `CellEditBinding` / `InstancedBinding` 风格的自定义桥接层**
4. **校验逻辑改为 `DataValidationErrors + UpdateSource()`**
5. **`TopLevel.GetTopLevel(...).FocusManager` 取代旧焦点获取路径**
6. **`GetPlatformSettings()` / `GetVisualParent()` / `ScrollGesture +=` / 新 `RoundLayoutSizeUp` 签名**
7. **测试层的 `Binding` 改为 `ReflectionBinding`**

## 一句话总结

`Avalonia.Controls.DataGrid` 从 `11.3.13` 到 `12.0.0` 的核心变化，可以概括为：

> **围绕 Avalonia 12 的绑定模型、验证模型和顶层/输入 API 做兼容性重构，同时删掉 11.x 时代为 `IBinding`、`InstancedBinding`、内部 API 和旧测试基础设施准备的过渡代码。**
