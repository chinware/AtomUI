# DataGrid 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Phase F / Tier 4
> 状态：T4.2 column filter flyout lifecycle + filter indicator binding + column header hover handler cleanup partial done；T4.3 special column detach lifecycle partial done；T4.4 column reorder drag indicator render partial done；DataGrid core / row / cell / virtualization 仍待后续专项。

---

## 0. 结论

本轮优化有效，但不是大幅减少 DataGrid 主视觉树的优化。收益集中在关闭态过滤列头：

- 无过滤项的列头不再创建空 filter flyout shell。
- 有过滤项的列头只保留轻量 flyout shell，菜单/树过滤项延迟到首次打开前创建。
- filter indicator detach 时释放 flyout shell，并补齐 `CollectionView.FilterDescriptions` 订阅切换/释放。
- filter indicator 可见性不再为每个列头创建 3 路 `MultiBinding` + converter，改为列头 DirectProperty + `{TemplateBinding}`。
- column header hover 不再为每个 realized `DataGridColumnHeader` 注册本地 `PointerEntered` / `PointerExited` handler，改走 Avalonia 的 virtual override 路径。
- `Columns.Clear()` 现在和单列 remove 一样走 column about-to-detach，释放 special columns 缓存的 grid 事件订阅。
- column reorder dragging-over indicator 不再在每次 `Render()` 时创建 dashed `Pen`，改为按 foreground 缓存。
- 运行时增删 `DataGridColumn.Filters` 会同步刷新 filter indicator 可见性和 flyout shell。
- 修复 `DataGridColumnHeader.IsMiddleVisible` 错误 raise 到 `IsLastVisibleProperty` 的正确性问题。

Avalonia 依据：

- `PopupFlyoutBase.ShowAtCore` 只有在 `Popup.Child == null` 时才调用 `CreatePresenter()`：`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/PopupFlyoutBase.cs:292`。
- `MenuFlyout.CreatePresenter()` 把 `Items` 交给 presenter：`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/MenuFlyout.cs:85-92`。
- `ItemsControl.Items` 变更会进入 logical children / item count 路径：`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsControl.cs:639-656`。
- `TemplateBinding` 直接订阅 templated parent 的 `PropertyChanged`：`.referenceprojects/Avalonia/src/Avalonia.Base/Data/TemplateBindingExpression.cs:37-43`。
- `MultiBindingExpression` 在子 observable 变更后进入 converter 路径：`.referenceprojects/Avalonia/src/Avalonia.Base/Data/Core/MultiBindingExpression.cs:23-49`、`:86-98`。
- `PointerEntered` / `PointerExited` 是 `Direct` routed event：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:144-155`。
- Avalonia 已通过 class handler 调用 `OnPointerEnteredCore` / `OnPointerExitedCore`，再进入 virtual `OnPointerEntered` / `OnPointerExited`：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:242-243`、`:1019-1035`。
- 事件属性 `PointerEntered +=` / `PointerExited +=` 会走 `AddHandler`，本地订阅进入 `_eventHandlers` 字典：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:353-365`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/Interactive.cs:176-190`。
- `Pen` 是 mutable `AvaloniaObject`，`Brush` / `Thickness` / `DashStyle` 都是 styled properties：`.referenceprojects/Avalonia/src/Avalonia.Base/Media/Pen.cs:17-40`。
- `Visual.AffectsRender` 会在注册属性变更时 invalidate render：`.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:446-500`。

因此，过滤菜单项属于 popup-open-only 内容，延迟到 `FlyoutAboutToShow` 前创建符合 Popup Lazy Content Rule。

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:DataGrid>` | 65 |
| DataGrid column declarations | 210 |
| Columns with filter blocks | 6 |
| `DataGridFilterItem` declarations | 21 |

真实会话操作频率：

- 页面导航：至少 1 次。
- 过滤按钮打开：只有用户操作过滤列时发生，默认关闭态。

结论：DataGrid 页面实例量和列数量都明显超过 SKILL Tier 1 §13 门槛；关闭态过滤内容值得优化。

---

## 2. 收益表

### 2.1 结构收益

| 指标 | baseline | optimized | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Plain 3-column grid filter flyout shells | 4 | 0 | 100.00% removed | 有效；3 列 + top-left header 都不再创建空 shell |
| Filter 3-column grid closed flyout shells | 4 | 2 | 50.00% removed | 有效；只有 2 个带 filter items 的列保留 shell |
| Closed filter flyout materialized items | 7 | 0 | 100.00% deferred | 有效；首次打开前不创建菜单项 |
| Unopened sibling filter flyout materialized items | n/a | 0 | preserved lazy state | 有效；打开一个 filter 不牵连其他 filter |
| Detached filter indicator flyout shells | retained risk | 0 | release path added | 正确性/泄漏风险修复 |
| Filter indicator visibility bindings per header | 3 bindings + MultiBinding converter | 1 TemplateBinding | binding graph reduced | 有效；DataGridShowCase 210 个列声明受益 |
| Runtime `Filters` collection visibility refresh | binding-path dependent | explicit `CollectionChanged` refresh | correctness path added | 正确性修复；add/clear filter items 均验证 |
| Column header local hover routed handlers | 2 per realized header | 0 per realized header | 100.00% removed | 结构/分配优化；hover 语义保留在 `OnPointerEntered` / `OnPointerExited` override |
| Special column grid subscriptions after `Columns.Clear()` | retained via Reset path | released | leak path removed | 正确性/生命周期修复；覆盖 detail / reorder / selection / checkbox / operation columns |
| Column reorder drag indicator `Pen` allocations | 1 per render | 1 per foreground value | render allocation removed | 结构优化；只影响列拖拽重排交互 |

### 2.2 Gallery 同参数复测

命令：

```bash
dotnet run -c Debug -f net10.0 --no-build \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase datagrid --cold-iterations 3 --iterations 8 --warmup 3
```

| 指标 | baseline | optimized | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Cold navigation mean | 4230.37 ms | 3032.43 ms | 28.32% | 正向，但 DataGrid 页面波动较大 |
| Cold navigation median | 4110.94 ms | 3011.75 ms | 26.74% | 正向 |
| Cold navigation P95 | 4481.33 ms | 3102.18 ms | 30.77% | 正向 |
| Cold alloc mean | 182352.31 KB | 182395.39 KB | -0.02% | 基本不变 |
| Repeated navigation mean | 1571.65 ms | 1380.94 ms | 12.13% | 正向 |
| Repeated navigation median | 1562.07 ms | 1371.01 ms | 12.23% | 正向 |
| Repeated navigation P95 | 1649.63 ms | 1434.52 ms | 13.04% | 正向 |
| Repeated alloc mean | 172780.27 KB | 172841.63 KB | -0.04% | 基本不变 |

本轮判断：有效。主收益是关闭态过滤 popup 内容不再提前构造；Gallery timing 正向但不是唯一依据，后续 DataGrid 核心仍应继续拆 row/cell/column header 专项。

### 2.3 Column header filter indicator binding 复测

本轮优化点：把 `DataGridFilterIndicator.IsVisible` 从 XAML 三路 `MultiBinding` 改为 `DataGridColumnHeader.FilterIndicatorVisible` DirectProperty + `{TemplateBinding}`，并删除专用 converter。

控件级命令：baseline 使用临时 worktree `HEAD=9bd85e754`，optimized 使用当前工作区；两边均运行两轮 `count=100` 后取平均。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 100
```

| Scenario | 指标 | baseline | optimized | 改善 | 结论 |
| --- | --- | ---: | ---: | ---: | --- |
| DataGrid.Basic | ms/item | 16.703 | 16.382 | 1.92% | 正向；单轮有波动，均值正向 |
| DataGrid.Basic | KB/item | 3032.6 | 2981.6 | 1.68% | 正向 |
| DataGrid.Filter.Menu.Closed | ms/item | 10.323 | 10.101 | 2.15% | 正向 |
| DataGrid.Filter.Menu.Closed | KB/item | 2652.6 | 2613.4 | 1.48% | 正向 |
| DataGrid.Filter.Tree.Closed | ms/item | 8.998 | 8.716 | 3.13% | 正向 |
| DataGrid.Filter.Tree.Closed | KB/item | 2651.3 | 2611.2 | 1.51% | 正向 |
| DataGrid.GalleryShape | ms/item | 45.936 | 44.477 | 3.18% | 正向 |
| DataGrid.GalleryShape | KB/item | 12778.7 | 12562.9 | 1.69% | 正向 |

真实 Gallery 同参数复测：

```bash
dotnet run -c Debug -f net10.0 --no-build \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase datagrid --cold-iterations 3 --iterations 8 --warmup 3
```

| 指标 | baseline | optimized | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Cold navigation mean | 3368.91 ms | 3332.93 ms | 1.07% | 小幅正向 |
| Cold navigation median | 3249.93 ms | 3352.99 ms | -3.17% | 3 个 cold 样本下波动，非本轮主判据 |
| Cold navigation P95 | 3682.07 ms | 3434.89 ms | 6.71% | 正向 |
| Cold alloc mean | 182245.07 KB | 179481.42 KB | 1.52% | 正向 |
| Repeated navigation mean | 1475.26 ms | 1382.78 ms | 6.27% | 正向 |
| Repeated navigation median | 1463.74 ms | 1383.66 ms | 5.47% | 正向 |
| Repeated navigation P95 | 1531.90 ms | 1405.09 ms | 8.28% | 正向 |
| Repeated alloc mean | 172426.37 KB | 170235.01 KB | 1.27% | 正向 |

本轮判断：有效。主要收益来自列头模板绑定图收敛；结构 visual/logical 计数不变，分配下降稳定，真实 Gallery repeated 指标正向。

### 2.4 Column header hover handler cleanup 复测

本轮优化点：删除 `DataGridColumnHeader` 构造路径中每个实例的 `PointerEntered +=` / `PointerExited +=`，保留已有 `OnPointerEntered` / `OnPointerExited` override，并在 override 中调用原 hover 状态逻辑。排序 tooltip 逻辑仍先执行，hover pseudo-class 与拖拽/resize 状态更新不改变。

控件级命令：baseline 使用临时 worktree `HEAD=2d869466e`，optimized 使用当前工作区；运行 `count=100` 三轮、`count=300` 两组配对（其中一组反向顺序）。`ms/item` 在本地复测中方向不稳定，不能作为本轮主收益；`KB/item` 每轮稳定下降，因此本轮只按结构/分配收益记录。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 300
```

| Scenario | 指标 | baseline | optimized | 改善 | 结论 |
| --- | --- | ---: | ---: | ---: | --- |
| DataGrid.Basic | KB/item | 3034.3 | 3030.2 | 0.13% | 稳定正向；约 4.1 KB/item |
| DataGrid.Filter.Menu.Closed | KB/item | 2628.5 | 2625.1 | 0.13% | 稳定正向；约 3.4 KB/item |
| DataGrid.Filter.Tree.Closed | KB/item | 2628.1 | 2624.8 | 0.13% | 稳定正向；约 3.3 KB/item |
| DataGrid.GalleryShape | KB/item | 12725.3 | 12707.8 | 0.14% | 稳定正向；约 17.5 KB/item |
| DataGrid.Basic | ms/item | 13.812 | 14.862 | -7.60% | 噪声/热状态内，不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | ms/item | 9.518 | 9.917 | -4.19% | 噪声/热状态内，不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | ms/item | 8.980 | 9.331 | -3.91% | 噪声/热状态内，不作为本轮收益 |
| DataGrid.GalleryShape | ms/item | 45.585 | 43.536 | 4.49% | 正向，但同样不单独作为主判据 |

`count=100` 三轮均值用于交叉确认：Basic/Menu/Tree/GalleryShape 的 allocation 分别下降约 `0.14% / 0.21% / 0.14% / 0.14%`；time 方向混合（Basic 小幅负向，GalleryShape 正向），结论保持为结构/分配优化。

### 2.5 Special column detach lifecycle

本轮修复点：`DataGridColumnCollection.RemoveItemPrivate()` 会调用 `NotifyOwningGridAboutToDetached()` 再把 `OwningGrid` 置空，但 `ClearItems()` 原先直接 `OwningGrid = null`，绕过 about-to-detach。对 `DataGridDetailExpanderColumn`、`DataGridRowReorderColumn`、`DataGridSelectionColumn`、`DataGridCheckBoxColumn`、`DataGridOperationColumn` 这类会缓存 `_owningGrid` 并订阅 grid 事件的列，`Columns.Clear()` 会留下 stale grid 引用和事件链。

修复后：

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| `DataGrid.Columns.Clear()` about-to-detach | skipped | called for every internal column | 与单列 remove 语义对齐 |
| Detail / row reorder / operation column grid row events | retained risk | released | `LoadingRow` / `UnloadingRow` 取消订阅 |
| Selection column grid events | retained risk | released | `LoadingRow` / `SelectionChanged` / `PropertyChanged` 取消订阅 |
| CheckBox column grid events | retained risk | released | `CurrentCellChanged` / `KeyDown` / `LoadingRow` 取消订阅 |
| Row reorder `ItemsSource` guard | retained risk | released | `PropertyChanged` 取消订阅 |

这轮不宣称页面加载提速；`datagrid --count 100` smoke 显示 visual/logical 计数不变。收益是清空列集合、重建列模型、动态切换 special columns 后不保留旧 grid 事件链。

### 2.6 Column reorder drag indicator render resource

本轮优化点：`DataGridColumnDraggingOverIndicator.Render()` 原先每次绘制都执行 `new Pen(TextElement.GetForeground(this), 1.0, DashStyle.Dash)`。列拖拽重排时这个 indicator 会跟随 pointer 移动重绘，`Pen` 是 Avalonia `StyledProperty` carrier，不应在 render loop 中重复创建。

修复后按当前 `TextElement.Foreground` 缓存 dashed `Pen`，并把 `TextElement.ForegroundProperty` 加入 `AffectsRender`；foreground 变化时清空缓存，下次 render 重建。

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| Drag indicator repeated render with same foreground | `new Pen` per render | reuse cached `Pen` | render 分配移除 |
| Drag indicator foreground change | implicit fresh pen per render | cached pen invalidated and rebuilt once | 颜色更新语义保留 |

这轮不影响页面加载和默认 DataGrid visual tree；收益只在用户拖拽重排列时发生。

---

## 3. 验证

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

结果：构建通过；保留既有 warning `DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-datagrid-states
```

结果：`DataGrid state verification passed.` 覆盖：无 filter items 不创建 shell、关闭态 filter content lazy、运行时 add/clear filter items 后 indicator 可见性和 flyout shell 同步、realized column header 不再注册本地 hover routed handlers、special columns 在 `Columns.Clear()` 后释放 cached owning grid、column reorder drag indicator 复用 cached render pen 且 foreground 变化后重建。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 20
```

关键结果：

| Scenario | ms/item | KB/item | Visual/root | Logical/root |
| --- | ---: | ---: | ---: | ---: |
| DataGrid.Basic | 33.713 | 2947.6 | 305.0 | 1.0 |
| DataGrid.Filter.Menu.Closed | 16.627 | 2657.6 | 267.0 | 1.0 |
| DataGrid.Filter.Tree.Closed | 11.390 | 2656.4 | 267.0 | 1.0 |
| DataGrid.GalleryShape | 61.388 | 12845.4 | 1260.0 | 5.0 |
