# TreeView 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 1 #1
> 状态：本轮已完成 Gallery 实测。

---

## 0. 结论

本轮优化 `NodeSwitcherButton` 的模板结构：每个树节点的 switcher 原来固定创建 5 个 `IconPresenter`（expand/collapse/rotation/loading/leaf），实际同一时刻只显示一个。现在模板只保留 1 个 `IconPresenter`，由 `NodeSwitcherButton.CurrentIcon` 和 `IsCurrentIconVisible` 选择当前图标。

这不是 axaml 节点搬到 C# 动态创建；功能视觉仍在 `ControlTheme` 中，C# 只维护运行态属性。

本轮追加优化 `TreeViewItem` 的连接线渲染路径：显示树线时不再每次 `Render()` 都创建 `Pen`，改为按 item 缓存 line pen，并在 `BorderBrush` 或 `BorderThickness.Top` 变化时重建。随后继续收敛 TreeView 拖拽指示线：拖拽过程中不再每帧创建 indicator `Pen`，按 `DragIndicatorBrush` / `DragIndicatorLineWidth` 复用缓存。

后续追加优化 checkbox 父节点三态聚合：子节点 checked 状态变化后，父链原先分别用 `All(...)` 和 `Any(...)` 两次 LINQ 扫描子项，并最多调用两轮 `TreeContainerFromItem`。现在改为一次手写扫描，同时得出 `isAllChecked` / `isAnyChecked`，在已确定混合态时提前退出。该项是交互路径 structural-only 收益，不声明页面导航 timing 提升。

| 指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 1049.62 ms | 776.51 ms | 26.0% |
| Cold alloc mean | 54136.18 KB | 37558.07 KB | 30.6% |
| Repeated navigation mean | 332.44 ms | 213.41 ms | 35.8% |
| Repeated median | 323.32 ms | 206.68 ms | 36.1% |
| Repeated P95 | 403.63 ms | 243.17 ms | 39.8% |
| Repeated alloc mean | 49964.64 KB | 33732.33 KB | 32.5% |
| Runtime visuals | 2108 | 1646 | 21.9% |
| `IconPresenter` | 332 | 112 | 66.3% |
| `Icon` / `PathIcon` | 302 | 60 | 80.1% |

| 追加指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| TreeViewItem leaf line `Pen` allocations / repeated render | 2 pens | 0 pens after first render | 100.00% |
| TreeViewItem branch line `Pen` allocations / repeated render | 1 pen | 0 pens after first render | 100.00% |
| TreeView drag indicator `Pen` allocations / dragging render frame | 1 pen | 0 pens after first render | 100.00% |
| TreeView checkbox parent aggregation child scans / parent update | 2 passes | 1 pass | 50.00% |
| TreeView checkbox parent aggregation container lookups / parent update with N children | up to `2N` | up to `N` | 50.00% |
| TreeView checkbox parent aggregation LINQ pipelines / parent update | 2 | 0 | 100.00% |

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:TreeView>` | 15 |
| declared `<atom:TreeViewItem>` | 87 |
| `ShowCaseItem` | 10 |
| Runtime `MotionActor` / tree rows | 55 |

真实会话操作频率：

- 页面导航：至少 1 次。
- 展开/折叠：基础示例、line 示例、自定义 switcher、搜索结果、右键菜单示例都会触发。
- 过滤搜索：2 个 `SearchEdit` 示例。
- 拖拽：`IsDraggable=True` 示例。
- 右键菜单：`ItemContextMenuRequest` 示例。

结论：实例数 > 5，操作 > 1/session，并已有 Gallery 数字，满足 SKILL Tier 1 §13。

---

## 2. 根因

`NodeSwitcherButtonTheme.axaml` 原模板为每个 switcher 固定创建 5 个图标 presenter：

- `ExpandIconPresenter`
- `CollapseIconPresenter`
- `RotationIconPresenter`
- `LoadingIconPresenter`
- `LeafIconPresenter`

Gallery baseline 显示 `IconPresenter=332`、`Icon/PathIcon=302`，远高于页面直接声明的图标数量。TreeView 当前真实稳定形态有 55 个节点 header，5 个 switcher presenter 会放大成约 275 个额外 icon presenter 结构。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/TemplatedControl.cs:333`：`template.Build(this)` 创建模板树。
- `.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/TemplatedControl.cs:338`：模板根加入 visual children。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:546` / `:624`：`IsVisible=false` 的元素会跳过 measure，但前面的模板创建成本已经发生。
- `src/AtomUI.Controls/Icon/IconPresenter.cs:50`：`IconProperty` 影响 measure。
- `src/AtomUI.Controls/Icon/IconPresenter.cs:95` / `:96`：`IconPresenter` 会把当前 `PathIcon` 加入 visual/logical children。

主导子系统：Templates / visual tree structure，其次是 IconPresenter 绑定与子图标结构。

可证伪假设：如果把 `NodeSwitcherButton` 的 5 个固定 `IconPresenter` 收敛到 1 个，同时保持所有图标状态和 selector 行为，`TreeViewShowCase` 的 `IconPresenter` 与 visual count 应显著下降，并带来 repeated navigation >= 5% 的时间或分配改善；否则回退。

---

## 3. 改动

### 3.1 `NodeSwitcherButton` 增加运行态只读属性

新增：

- `CurrentIcon`：当前应该交给模板 presenter 显示的 `PathIcon?`。
- `IsCurrentIconVisible`：Leaf 模式且 `IsLeafIconVisible=false` 时隐藏 presenter。

状态映射：

| `IconMode` | 当前图标 |
| --- | --- |
| `Default` + unchecked | `ExpandIcon` |
| `Default` + checked | `CollapseIcon` |
| `Rotation` | `RotationIcon` |
| `Loading` | `LoadingIcon` |
| `Leaf` + visible | `LeafIcon` |
| `Leaf` + hidden | `null` |

触发更新的属性：`IconMode`、`IsChecked`、`IsLeafIconVisible`、`ExpandIcon`、`CollapseIcon`、`RotationIcon`、`LoadingIcon`、`LeafIcon`。

### 3.2 模板保留单个 presenter

`NodeSwitcherButtonTheme.axaml` 将 5 个 presenter 替换成：

```xml
<atom:IconPresenter Name="CurrentIconPresenter"
                    RenderTransform="{TemplateBinding RotationIconRenderTransform}"
                    Icon="{TemplateBinding CurrentIcon}"
                    IsVisible="{TemplateBinding IsCurrentIconVisible}" />
```

保留的样式行为：

- 普通尺寸：`IconSize`。
- Rotation 模式：`IconSizeXS`。
- Loading 模式：`ColorPrimary` icon brush。
- checked + Rotation：`RotationIconRenderTransform=rotate(90deg)`。
- motion：`Background` 与 `RotationIconRenderTransform` transition 不变。

### 3.3 TreeViewItem 连接线 Pen 缓存

`TreeViewItem.RenderTreeNodeLine()` 原先在每条连接线绘制处直接 `new Pen(BorderBrush, penWidth)`：

- 非最后一个展开分支节点：1 条竖线，1 个 `Pen`。
- 叶子节点：竖线 + 横线，2 个 `Pen`。

现在每个 `TreeViewItem` 缓存一个 `_treeNodeLinePen`，同一个 item 在后续 render 中复用；当 `BorderBrush` 引用或 `BorderThickness.Top` 变化时重建。`BorderBrushProperty` / `BorderThicknessProperty` 已在 `AffectsRender<TreeViewItem>` 中注册，因此 token / theme 变化仍会触发重绘。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Base/Media/Pen.cs:17-40`：`Pen` 是 mutable `AvaloniaObject`，`Brush` / `Thickness` 是 styled properties；render 热路径不应按帧创建。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:446-500`：`AffectsRender` 会在相关属性变化时 invalidate render。

### 3.4 TreeView 拖拽指示线 Pen 缓存

`TreeView.Render()` 在拖拽期间会绘制 drop indicator。原实现每个 dragging render frame 都执行：

```csharp
new Pen(DragIndicatorBrush, DragIndicatorLineWidth)
```

现在 TreeView 实例缓存 `_dragIndicatorPen`，当 `DragIndicatorBrush` 引用或 `DragIndicatorLineWidth` 变化时重建。`DragIndicatorRenderInfoProperty`、`DragIndicatorBrushProperty`、`DragIndicatorLineWidthProperty` 已在 `ConfigureDragAndDrop()` 中注册 `AffectsRender<TreeView>`，因此拖拽位置和主题 token 变化仍按原路径触发重绘。

### 3.5 Checkbox 父节点状态聚合一次扫描

`SetupParentNodeCheckedStatus()` 原先在每个父节点上分别执行：

```csharp
parentTreeItem.Items.All(...)
parentTreeItem.Items.Any(...)
```

两次 LINQ 扫描都需要把 child item 映射到 realized `TreeViewItem`。现在 `GetChildCheckStatus()` 在一次循环内同时计算：

- `isAllChecked`：所有可勾选子项均为 checked，非可勾选子项按原语义不阻止 all checked。
- `isAnyChecked`：存在 checked 或 indeterminate 的可勾选子项。

当 `!isAllChecked && isAnyChecked` 已成立时提前退出，因为父节点最终一定是 indeterminate。验证覆盖单子项 checked -> parent indeterminate、全 checked -> parent checked、再取消一个子项 -> parent indeterminate，并检查 `CheckedItems` 同步。

---

## 4. 验证

### 4.1 构建

```bash
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --framework net10.0
```

结果：0 Warning(s), 0 Error(s)。

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --framework net10.0
```

结果：构建通过；保留既有 warning `DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

后续追加复测：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-treeview-states
```

结果：构建 `0 Warning(s), 0 Error(s)`；`TreeView state verification passed.`

### 4.2 Gallery 基线与优化后对比

命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase treeview --label baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/treeview-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase treeview --label optimized \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/treeview-showcase-optimized.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | IconPresenter | PathIcon |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 1049.62 ms | 1056.51 ms | 1189.91 ms | 54136.18 KB | 2108 | 332 | 302 |
| Cold optimized | 776.51 ms | 770.78 ms | 830.95 ms | 37558.07 KB | 1646 | 112 | 60 |
| Repeated baseline | 332.44 ms | 323.32 ms | 403.63 ms | 49964.64 KB | 2108 | 332 | 302 |
| Repeated optimized | 213.41 ms | 206.68 ms | 243.17 ms | 33732.33 KB | 1646 | 112 | 60 |

### 4.3 Regression matrix

Matrix：`tools/performances/AtomUI.Performance/Suites/TreeView/Regression.md`

自动状态验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-treeview-states
```

结果：`TreeView state verification passed.`

本轮覆盖：

- 默认 expand/collapse 图标切换。
- rotation switcher checked transform。
- loading async node icon。
- leaf icon visible / hidden。
- search/filter page load path。
- context menu page load path。

追加 TreeView render pen 缓存验证：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-treeview-states
```

说明：该追加项是结构性 render 分配优化；当前 headless harness 没有逐帧 render allocation 计数器，因此只用 build / state verifier 做正确性回归，不使用单次 timing 证明速度收益。

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 try/finally 标志位 | 0 |
| 新增 disposable 字段 | 0 |
| 新增订阅 / timer / reparented element | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| Theme Static Rule | 未触发 |
| 生产文件范围 | 2 个文件，均在 `TreeView` |

新增 `CurrentIcon` 是 direct runtime state，模板仍使用 `{TemplateBinding}`，没有引入 `RelayBind` 或同生命周期 disposable plumbing。

---

## 6. 后续

TreeView filter 高亮的 per-character `Run` 模式已在 §12 追加结构优化中处理。后续如果继续深入 TreeView，应优先建立 filter action 专项基线，而不是用页面导航 timing 证明 filter 交互收益。

---

## 7. 追加结构优化：TreeView 过滤高亮无命中 fast path

`TreeViewItemHeader.BuildFilterHighlightRuns()` 在 `HighlightedMatch` 模式下先检查首个命中；无命中时不再创建空 ranges list。同时补齐一个正确性短路：`FilterHighlightWords == ""` 时清空高亮 runs 并返回，避免旧逻辑在 `IndexOf("")` 上无法推进。`HighlightedWhole` 和 `BoldedMatch` 的现有行为保持不变：whole 仍整段着色，bold 仍按旧逻辑作用到所有字符。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| 无命中过滤高亮 ranges list / header rebuild | 1 list | 0 list | `(1 - 0) / 1` | 100.00% | TreeView filter 无命中路径少一次临时集合分配 |
| 命中高亮首个命中查找 / header rebuild | 2 次 | 1 次 | `(2 - 1) / 2` | 50.00% | 首个匹配位置复用，不重复搜索 |
| 空过滤词死循环风险 | 1 条 | 0 条 | `(1 - 0) / 1` | 100.00% | 正确性修复，不作为性能收益 |

说明：这是 filter 交互路径的结构性收益；本轮不声称 `TreeViewShowCase` 页面导航 timing 因此提升。

---

## 8. 追加结构优化：filter 选择闭包启动列表

`TreeView.HandleSelectionChanged()` 在 filter 模式下不再先把 selected containers 放进 `startupItems` 临时列表，而是在找到 container 时直接计算并合并闭包。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| filter selection startup list / selection changed | 1 list | 0 list | `(1 - 0) / 1` | 100.00% | 结构收益；filter 选择变化少一次临时集合分配 |
| selected container second pass / selection changed | 1 pass | 0 pass | `(1 - 0) / 1` | 100.00% | 结构收益；闭包在发现 container 时直接合并 |

说明：这是 filter 交互路径的结构性收益；不声明页面导航 timing 提升。

---

## 9. 追加结构优化：TreeNodePath 路径列表容量

`ExpandTreeViewPath()`、`CollapseTreeViewPath()`、`TraverTreeViewPath()` 的路径列表按 `TreeNodePath.Segments.Count` 预分配，临时展开状态列表也按同一长度预分配。路径内容、展开/恢复顺序不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeView pathNodes list growth / path traversal | dynamic capacity | exact capacity | structural | 分配更紧 | 路径节点数已知时避免 List 增长 |
| TreeView path expand-status list growth / temporary traversal | dynamic capacity | exact capacity | structural | 分配更紧 | 展开状态数量与路径长度一致 |
| TreeView checked reset added list growth / checked items reset | dynamic capacity | exact capacity | structural | 分配更紧 | reset 通知 added 列表按 `CheckedItems.Count` 预分配 |

说明：这是默认展开/选中路径的结构性收益；不声明页面导航 timing 提升。

---

## 10. 追加结构优化：TreeViewItem 子节点枚举去 LINQ

`TreeViewItem` 通过 `ITreeNode<ITreeItemNode>.Children` 暴露子节点时，旧实现返回 `Items.OfType<ITreeItemNode>()`。本轮改为本地显式枚举，保持只返回 `ITreeItemNode` 的语义。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeViewItem child node filter LINQ operators / child enumeration | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 结构收益；树路径 / 递归访问子节点时不再走 `OfType` LINQ operator |

说明：这是 tree data traversal 路径的结构性收益；不声明页面导航 timing 提升。

---

## 11. 追加结构优化：collection changed items indexed traversal

`TreeView` / `TreeViewItem` / `TreeItemNode` 在处理 `NotifyCollectionChangedEventArgs.NewItems/OldItems` 时，旧路径使用 `foreach` 遍历 collection changed item 列表。本轮改为直接读取 `IList.Count` 并按 index 访问，保持 remove / replace / parent-node update 顺序不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeView checked-items remove old-items enumerators / remove or replace | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；checked item 同步按 `OldItems.Count/indexer` 处理 |
| TreeView checked-items replace old/new items enumerators / replace | 2 enumerators | 0 enumerators | `(2 - 0) / 2` | 100.00% | 有效；replace 的 old/new item 标记都不再创建 enumerator |
| TreeItemNode parent update item enumerators / add or remove | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；新增 / 移除 child parent 更新按 index 处理 |

说明：这是树节点集合变化路径的结构性收益；不声明页面导航 timing 提升。

---

## 12. 追加结构优化：filter 高亮段级 Run

`TreeViewItemHeader.BuildFilterHighlightRuns()` 原先在 filter match 时按 header 每个字符创建一个 `Run`，再逐字符判断是否需要高亮。此前已先做无命中 fast path；本轮继续把命中 / 整段高亮 / 无匹配样式都改为段级 `Run`：

- `HighlightedMatch`：按匹配段和普通段切分，例如 `alpha beta alpha` + `alpha` 从 16 个字符 Run 降为 3 个段 Run。
- `HighlightedWhole`：整段文本只创建 1 个 Run。
- 只有 `BoldedMatch` 或无高亮样式时，也只创建 1 个普通段 Run，同时保留旧行为：`BoldedMatch` 仍作用到所有输出 Run。
- `FilterHighlightStrategy` / `FilterHighlightForeground` 变化后会重建 runs，避免已有 runs 保留旧样式。
- `FilterHighlightWords = null / ""` 或 header 文本为空时清空 runs，避免 stale 高亮残留。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| HighlightedMatch Run objects / `alpha beta alpha` header | 16 | 3 | `(16 - 3) / 16` | 81.25% | 有效；按匹配段创建 Run，不再逐字符创建 |
| HighlightedWhole Run objects / 16-char header | 16 | 1 | `(16 - 1) / 16` | 93.75% | 有效；整段高亮合并为单 Run |
| No-match styled Run objects / 16-char header | 16 | 1 | `(16 - 1) / 16` | 93.75% | 有效；无命中但仍需要 Inlines 时使用单 Run |
| Stale highlight runs on words/style/foreground change | possible | cleared/rebuilt | structural | 100.00% stale risk removed | 正确性收益；状态验证覆盖 |

说明：这是 TreeView / TreeSelect filter 交互路径 structural-only 收益；没有新增页面导航 timing 对比，不声明页面加载速度提升。

---

## 13. 追加结构优化：filter 选择闭包容量

`TreeView.CalculateSelectItemClosure()` 在 filter selection changed 路径中会构造当前 item 到根节点的闭包集合。父链深度可在构造前计算，本轮将临时 `HashSet<TreeViewItem>` 改为按实际深度预分配；闭包内容和父链遍历顺序不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| filter selection closure HashSet growth / selected item | dynamic growth | exact parent depth | structural | 结构收益 | 父链深度已知时避免 HashSet 增长 |
| filter selection closure semantics | same nodes | same nodes | n/a | 0.00% | 行为保持 |

说明：这是 filter 交互路径 structural-only 收益；没有新增页面导航 timing 对比，不声明页面加载速度提升。

验证补充：当时 `--verify-treeview-states` 失败在 highlighted-match foreground 断言；该断言口径已在第 18 节按 Avalonia inherited foreground 语义修正并通过。

---

## 14. 追加结构优化：勾选父链和 item path 容量

`SetupParentNodeCheckedStatus()` 会在勾选 / 取消勾选子树后收集父级 checked / unchecked 结果集；父链深度可在进入循环前计算，因此本轮将两个临时 `HashSet<object>` 按父链深度预分配。`GetTreePathFromItem()` 的返回路径列表也按 `ITreeItemNode` 或 `TreeViewItem` 父链深度预分配。勾选状态传播、路径内容和反转顺序不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| parent checked HashSet growth / checkbox cascade | dynamic growth | exact parent depth | structural | 结构收益 | checked / unchecked 父级结果集按父链深度预分配 |
| item path list growth / `GetTreePathFromItem` | dynamic growth | exact path depth | structural | 结构收益 | 路径列表按 item 父链深度预分配 |
| checkbox/path semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 TreeView 勾选联动和路径选择路径 structural-only 收益；没有新增页面导航 timing 对比，不声明页面加载速度提升。

---

## 15. 追加结构优化：勾选子树结果集容量

`DoCheckedSubTree()` / `DoUnCheckedSubTree()` 会先展开并记录 realized subtree，再递归收集 checked / unchecked 结果。本轮用 `expandedStates.Count` 加父链深度预估结果集上界，给临时 `HashSet<object>` 预分配容量。展开、递归勾选、父级状态更新和 restore 顺序不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| subtree checked result HashSet growth / check subtree | dynamic growth | realized subtree + parent depth capacity | structural | 结构收益 | 子树勾选结果集按已 realized 数预分配 |
| subtree unchecked result HashSet growth / uncheck subtree | dynamic growth | realized subtree + parent depth capacity | structural | 结构收益 | 子树取消勾选结果集按已 realized 数预分配 |
| subtree check semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 TreeView 勾选联动路径 structural-only 收益；没有新增页面导航 timing 对比，不声明页面加载速度提升。

---

## 16. 追加结构优化：filter highlight strategy flag check

`TreeViewItemHeader.BuildFilterHighlightRuns()` rebuild filter 高亮 runs 时需要判断 `HighlightedMatch`、`HighlightedWhole` 和 `BoldedMatch`。旧实现使用 enum `HasFlag()`；本轮改为 bitwise check，匹配段切分、整段高亮和加粗策略保持不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeView filter highlight strategy enum `HasFlag` callsites / header rebuild | 3 | 0 | `(3 - 0) / 3` | 100.00% | structural-only；filter 高亮策略判断不再走 enum helper |
| HighlightedMatch / HighlightedWhole / BoldedMatch semantics | unchanged | unchanged | n/a | 0.00% | 行为保持；Run 分段输出不变 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

验证补充：当时 `--verify-treeview-states` 失败在 highlighted-match foreground 断言；该断言口径已在第 18 节按 Avalonia inherited foreground 语义修正并通过。

---

## 17. 追加结构优化：item path 去 Reverse

`TreeView.GetTreePathFromItem()` 原先按 leaf-to-root 追加 `ITreeItemNode` / `TreeViewItem` 路径，再调用 `Reverse()`。本轮保留 path depth 预分配，先填充占位，再从后往前按 index 写入 root-to-leaf 路径；`SelectTreeItemByPath()` 输入顺序保持不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeView item path reverse passes / path lookup | 1 | 0 | `(1 - 0) / 1` | 100.00% | structural-only；按 path depth 倒序填充 |
| TreeView item path list capacity / path lookup | exact path depth | exact path depth | n/a | 0.00% | 已有容量收益保持 |
| Path order / selection semantics | root-to-leaf after Reverse | root-to-leaf direct fill | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

---

## 18. 验证补充：filter foreground 断言口径

`Run.Foreground` 在 Avalonia 中是 inherited attached property，默认有效值不是 `null`。本轮把 `--verify-treeview-states` 中的 highlighted-match 断言改为检查普通段没有本地设置 `TextElement.ForegroundProperty`，更准确覆盖“只给命中段设置高亮色”的语义。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Unmatched highlight segment local foreground sets / run | 0 | 0 | n/a | 0.00% | 生产行为不变；验证口径修正 |
| TreeView highlighted-match state verification | false failure | passed | structural | 100.00% false failure removed | 状态验证现在匹配 Avalonia inherited foreground 语义 |
| Page-load timing claim | none | none | n/a | n/a | 验证修正不声明性能收益 |

验证：`--verify-treeview-states` 通过。

---

## 19. 追加结构优化：filter / form flag 判断

本轮把 TreeView filter 与 FormItem 路径里的 enum `HasFlag()` 改为 bitwise check。这里特别处理了 Avalonia `SelectionMode.Single = 0`：单选判断改为“不含 Multiple”，多选判断改为“包含 Multiple”，避免 `HasFlag(Single)` 对 zero-value flag 永远为 true 的陷阱。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeView filter highlight strategy `HasFlag` callsites / filter pass | 6 | 0 | `(6 - 0) / 6` | 100.00% | structural-only；filter 策略判断改为 bitwise |
| TreeView selection-mode `HasFlag` callsites / filter selection change | 2 | 0 | `(2 - 0) / 2` | 100.00% | 正确性 + 结构；Multiple filter closure 不再落入 Single 分支 |
| TreeView form selection-mode `HasFlag` callsites / form get/set/clear | 3 | 0 | `(3 - 0) / 3` | 100.00% | structural-only；form value 路径直接判断 Multiple bit |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

验证：`--verify-treeview-states` 通过。
