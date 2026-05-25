# DataGrid 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Phase F / Tier 4
> 状态：T4.1 cell header-state binding + row/row group header pointer handler + core input handler + rows presenter scroll gesture / clip geometry + row gridline clip geometry cleanup partial done；T4.2 column filter flyout lifecycle + filter indicator binding + column/group header pointer handler cleanup partial done；T4.3 special column detach lifecycle + row expander details binding partial done；T4.4 column reorder drag indicator render partial done；T4.5 row details presenter measure registration partial done；DataGrid core / row / virtualization 仍待后续专项。

---

## 0. 结论

本轮优化有效，但不是大幅减少 DataGrid 主视觉树的优化。收益集中在关闭态过滤列头、列/行生命周期和 row details binding：

- 无过滤项的列头不再创建空 filter flyout shell。
- 有过滤项的列头只保留轻量 flyout shell，菜单/树过滤项延迟到首次打开前创建。
- filter indicator detach 时释放 flyout shell，并补齐 `CollectionView.FilterDescriptions` 订阅切换/释放。
- filter indicator 可见性不再为每个列头创建 3 路 `MultiBinding` + converter，改为列头 DirectProperty + `{TemplateBinding}`。
- column header hover 不再为每个 realized `DataGridColumnHeader` 注册本地 `PointerEntered` / `PointerExited` handler，改走 Avalonia 的 virtual override 路径。
- column header press/release/move 不再为每个 realized `DataGridColumnHeader` 注册本地 handler；内部 resize/reorder/sort 逻辑和 `DataGridColumn.HeaderPointerPressed/Released` 转发都改为 class handler。
- column group header press/release 不再为每个 realized `DataGridColumnGroupHeader` 注册本地 forwarding lambda，改为 class handler 转发到 `DataGridColumnGroupItem.HeaderPointerPressed/Released`。
- row header press 不再为每个 realized `DataGridRowHeader` 注册本地 `PointerPressed` handler，改为 class handler，同时保留 focus / selection / current slot 行为。
- row group header press 不再为每个 realized `DataGridRowGroupHeader` 注册本地 `PointerPressed` lambda，改为 class handler，同时保留 current slot 行为。
- `DataGrid` core `KeyDown` / `KeyUp` / `GotFocus` / `LostFocus` 不再为每个 grid 实例注册本地 routed handlers，改为 Avalonia virtual override 路径。
- `DataGridRowsPresenter` 不再为每个 presenter 注册本地 `ScrollGesture` handler，改为 class handler，同时保留滚动手势更新 `VerticalOffset` 的行为。
- `DataGridRowsPresenter` 不再在每次 arrange 时新建 clip `RectangleGeometry`，改为 presenter 级复用并在 `Rect` 变化时显式 invalidation。
- `DataGridRow` 底部分割线不再在每次 arrange 时新建 clip `RectangleGeometry`，改为 row 级复用并在模板重套 / reset 时释放引用。
- `DataGridCell` 跟随 column header 的 sort / reorder 状态绑定不再通过 `BindUtils.RelayBind` 创建每 cell 捕获 lambda，改为直接 observable 绑定 + 静态 converter delegate。
- `Columns.Clear()` 现在和单列 remove 一样走 column about-to-detach，释放 special columns 缓存的 grid 事件订阅。
- column reorder dragging-over indicator 不再在每次 `Render()` 时创建 dashed `Pen`，改为按 foreground 缓存。
- `DataGridDetailsPresenter.ContentHeight` 的 `AffectsMeasure` 注册从实例构造函数移到静态构造函数，避免每个 details presenter 重复添加 class-level property observer。
- `DataGridRowExpander` 不再为每个 realized row expander 创建 `Binding + Path` 的 `BindUtils.RelayBind` 双向绑定，改为 direct observable binding + checked-state writeback，并在 visual detach 时兜底释放 row 引用。
- 运行时增删 `DataGridColumn.Filters` 会同步刷新 filter indicator 可见性和 flyout shell。
- 修复 `DataGridColumnHeader.IsMiddleVisible` 错误 raise 到 `IsLastVisibleProperty` 的正确性问题。
- 修复分组行头模板中 `DataGridRowHeader` 可能先于 `Owner` 设置完成 `OnApplyTemplate()` 导致空引用的问题。

Avalonia 依据：

- `PopupFlyoutBase.ShowAtCore` 只有在 `Popup.Child == null` 时才调用 `CreatePresenter()`：`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/PopupFlyoutBase.cs:292`。
- `MenuFlyout.CreatePresenter()` 把 `Items` 交给 presenter：`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/MenuFlyout.cs:85-92`。
- `ItemsControl.Items` 变更会进入 logical children / item count 路径：`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsControl.cs:639-656`。
- `TemplateBinding` 直接订阅 templated parent 的 `PropertyChanged`：`.referenceprojects/Avalonia/src/Avalonia.Base/Data/TemplateBindingExpression.cs:37-43`。
- `MultiBindingExpression` 在子 observable 变更后进入 converter 路径：`.referenceprojects/Avalonia/src/Avalonia.Base/Data/Core/MultiBindingExpression.cs:23-49`、`:86-98`。
- `PointerEntered` / `PointerExited` 是 `Direct` routed event：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:144-155`。
- Avalonia 已通过 class handler 调用 `OnPointerEnteredCore` / `OnPointerExitedCore`，再进入 virtual `OnPointerEntered` / `OnPointerExited`：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:242-243`、`:1019-1035`。
- `PointerMoved` / `PointerPressed` / `PointerReleased` 是 tunnel + bubble routed events：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:158-179`。
- 事件属性 `PointerXxx +=` 会走 `AddHandler`，本地订阅进入 `_eventHandlers` 字典；class handler 通过 `RoutedEvent.Raised` 按类型订阅一次：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:353-389`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/Interactive.cs:176-190`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:84-99`。
- Avalonia 已通过 `InputElement` class handlers 把 `GotFocus` / `LostFocus` / `KeyDown` / `KeyUp` 分发到 virtual `OnGotFocus` / `OnLostFocus` / `OnKeyDown` / `OnKeyUp`：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:235-240`、`:596-657`。
- `KeyDown` / `KeyUp` 是 tunnel + bubble routed events，事件属性 `+=` 默认只添加 direct/bubble 本地 handler；`AddClassHandler` 默认也覆盖 direct/bubble route，因此 override 路径与原实例订阅的 bubble 处理口径一致：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:106-118`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/Interactive.cs:30-64`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:78-99`。
- `ScrollGestureEvent` 是 bubble routed event，事件属性 `+=` 同样走 `AddHandler` 进入本地 `_eventHandlers`；`AddClassHandler` 默认覆盖 direct/bubble route，因此 rows presenter 的 class handler 与原本地 handler 路由口径一致：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.Gestures.cs:65-69`、`:197-200`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/Interactive.cs:30-64`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:78-99`。
- `Visual.Clip` 是 `StyledProperty<Geometry?>`，且 `Visual` static constructor 把 `ClipProperty` 注册进 `AffectsRender`；`InvalidateVisual()` 会把 visual 加入 renderer dirty 集合：`.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:52-53`、`:141-149`、`:418-421`。
- `RectangleGeometry.RectProperty` 通过 `AffectsGeometry` 标记 geometry dirty；复用同一个 geometry 时本轮在 `Rect` 变化后显式调用 `InvalidateVisual()`，不依赖 `Clip` 换引用来触发重绘：`.referenceprojects/Avalonia/src/Avalonia.Base/Media/RectangleGeometry.cs:25-33`、`.referenceprojects/Avalonia/src/Avalonia.Base/Media/Geometry.cs:146-172`。
- `PressedMixin.Attach<T>` 本身通过 class handler 维护 `:pressed`，本轮 column header class handler 注册在 `PressedMixin.Attach<DataGridColumnHeader>()` 之后，避免影响 pressed 状态：`.referenceprojects/Avalonia/src/Avalonia.Controls/Mixins/PressedMixin.cs:14-33`。
- `AvaloniaObjectExtensions.Bind(IObservable<T>)` 对 DirectProperty 进入 `DirectBindingObserver<T>` 路径：`.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObjectExtensions.cs:188-225`、`.referenceprojects/Avalonia/src/Avalonia.Base/PropertyStore/DirectBindingObserver.cs:7-84`。
- `Pen` 是 mutable `AvaloniaObject`，`Brush` / `Thickness` / `DashStyle` 都是 styled properties：`.referenceprojects/Avalonia/src/Avalonia.Base/Media/Pen.cs:17-40`。
- `Visual.AffectsRender` 会在注册属性变更时 invalidate render：`.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:446-500`。
- `AffectsMeasure<T>` 注释要求在控件 static constructor 中调用，且实现会对 `property.Changed` 订阅 observer：`.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:502-512`。
- `AvaloniaObjectExtensions.GetObservable` 为属性提供直接 observable；`AvaloniaObject.Bind(StyledProperty<T>, IObservable<T>)` 直接走 value store binding，非 `BindingBase` 时不创建 path binding expression：`.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObjectExtensions.cs:59-77`、`.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObject.cs:511-522`。
- `BindUtils.RelayBind` 会创建 `Binding`、设置 `Source` 和 `Path`，再调用 `target.Bind(...)`：`src/AtomUI.Core/Data/BindUtils.cs:8-36`。

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
| Column header local press/release/move handlers | 5 per standard header | 0 per standard header | 100.00% removed | 结构/分配优化；internal handler + public column event forwarding 均迁到 class handler |
| Column group header forwarding handlers | 2 per group header | 0 per group header | 100.00% removed | 结构/分配优化；Gallery group header 示例有 4 个 group items |
| Row header local press handler | 1 per row header | 0 per row header | 100.00% removed | 结构/分配优化；行头点击 selection / current slot 语义已验证 |
| Row group header local press handler | 1 per row group header | 0 per row group header | 100.00% removed | 结构/分配优化；分组头点击 current slot 语义已验证 |
| Row header owner/template ordering | template assumed owner ready | owner-dependent state retries from `Owner` setter | null-ref path removed | 正确性修复；分组行头覆盖验证 |
| DataGrid core input handlers | 4 local routed handlers per grid | 0 local core input handlers per grid | 100.00% removed | 结构/分配优化；KeyDown / GotFocus 行为已验证 |
| Rows presenter scroll gesture handler | 1 local handler per rows presenter | 0 local handlers per rows presenter | 100.00% removed | 结构/分配优化；ScrollGesture 滚动行为已验证 |
| Rows presenter clip geometry allocation | 1 new `RectangleGeometry` per arrange | 0 new `RectangleGeometry` per arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；clip instance 复用和 Rect 更新已验证 |
| Row bottom grid-line clip geometry allocation | 1 new `RectangleGeometry` per realized row arrange | 0 new `RectangleGeometry` per row arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；gridline clip instance 复用和 horizontal offset Rect 更新已验证 |
| Cell header-state binding converters | 2 captured lambdas per realized data cell | 2 cached static converter delegates | per-cell converter closures removed | 结构/分配优化；sort / reorder header state 语义保留 |
| Special column grid subscriptions after `Columns.Clear()` | retained via Reset path | released | leak path removed | 正确性/生命周期修复；覆盖 detail / reorder / selection / checkbox / operation columns |
| Column reorder drag indicator `Pen` allocations | 1 per render | 1 per foreground value | render allocation removed | 结构优化；只影响列拖拽重排交互 |
| Details presenter measure registration | 1 `property.Changed` subscription per presenter instance | 1 static registration per control type | duplicate class-level subscriptions removed | 结构/生命周期优化；`ContentHeight` 仍触发 measure invalidation |
| Row expander details binding | 1 `Binding + Path` TwoWay relay per row expander | 1 direct observable binding + checked-state writeback | path binding removed | 结构/生命周期优化；checked ↔ row details 双向同步和 detach 释放均验证 |

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

### 2.7 Cell header-state binding converters

本轮优化点：`DataGridCell.EnsureSortBindings()` 原先对每个 realized cell 使用 `BindUtils.RelayBind` 建立两条转换订阅：

- `DataGridColumnHeader.CurrentSortingState -> DataGridCell.IsSorting`
- `DataGridColumnHeader.HeaderDragMode -> DataGridCell.OwningColumnDragging`

`DataGrid.GalleryShape` 控件级场景有 `115` 个 realized data cells；真实 `DataGridShowCase` 声明 `65` 个 DataGrid、`210` 个 columns，因此 per-cell setup 成本会按页面实例量放大。修复后保留 DirectProperty binding 路径，但直接使用 `headerCell.GetObservable(...)` + `this.Bind(...)`，并把转换函数缓存为静态 delegate，避免每 cell 创建捕获 lambda 和 `BindUtils` 注册检查。

控件级命令：baseline 使用临时 worktree `HEAD=af9e97309`，optimized 使用当前工作区；两边各运行两轮 `count=300` 后取平均。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 300
```

| Scenario | 指标 | baseline | optimized | 改善 | 结论 |
| --- | --- | ---: | ---: | ---: | --- |
| DataGrid.Basic | ms/item | 15.019 | 13.630 | 9.25% | 正向 |
| DataGrid.Basic | KB/item | 3030.3 | 3028.5 | 0.06% | 小幅正向 |
| DataGrid.Filter.Menu.Closed | ms/item | 10.262 | 9.538 | 7.06% | 正向 |
| DataGrid.Filter.Menu.Closed | KB/item | 2625.3 | 2624.0 | 0.05% | 小幅正向 |
| DataGrid.Filter.Tree.Closed | ms/item | 9.600 | 9.445 | 1.61% | 小幅正向 |
| DataGrid.Filter.Tree.Closed | KB/item | 2625.0 | 2623.6 | 0.05% | 小幅正向 |
| DataGrid.GalleryShape | ms/item | 46.846 | 43.783 | 6.54% | 正向 |
| DataGrid.GalleryShape | KB/item | 12708.7 | 12701.2 | 0.06% | 小幅正向 |

本轮判断：保留。主收益仍按结构/分配记录；visual/logical 计数不变，sort / reorder state propagation 由 `--verify-datagrid-states` 覆盖。

### 2.8 Details presenter measure registration

本轮优化点：`DataGridDetailsPresenter` 原先在实例构造函数里调用 `AffectsMeasure<DataGridDetailsPresenter>(ContentHeightProperty)`。Avalonia 的 `AffectsMeasure<T>` 是 class-level 注册：源码注释要求在控件 static constructor 中调用，实现会对 `property.Changed` 订阅 observer。因此每个 realized `DataGridDetailsPresenter` 都会重复注册同一个类型/属性的 observer。

`DataGridRowTheme` 中每个 realized row 都包含一个 `DataGridDetailsPresenter` 槽位，即使 details frame 通过 `IsDetailsVisible` 折叠；Gallery 的 `Expandable Row`、`Order Specific Column`、`Row Header` 等场景会使用 `RowDetailsTemplate`。修复后只保留一次静态注册，所有实例共享同一条 class-level observer。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| `DataGridDetailsPresenter` 构造 | 每实例调用 `AffectsMeasure` | 不调用 | 构造路径去掉 class-level observer 订阅 | 有效；row 越多，重复订阅越多 |
| `ContentHeight` measure invalidation | 依赖重复注册 | 依赖静态注册 | 行为保留 | `--verify-datagrid-states` 覆盖 `ContentHeight` 改变后 `IsMeasureValid=false` |
| 默认 DataGrid visual/logical tree | 不变 | 不变 | 无结构变化 | 本轮不宣称页面导航或 visual count 下降 |

当前工作区 smoke：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 100
```

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 16.477 | 2975.2 | 305.0 | 1.0 | smoke 通过 |
| DataGrid.Filter.Menu.Closed | 9.892 | 2608.5 | 267.0 | 1.0 | smoke 通过 |
| DataGrid.Filter.Tree.Closed | 8.769 | 2606.1 | 267.0 | 1.0 | smoke 通过 |
| DataGrid.GalleryShape | 42.884 | 12536.5 | 1260.0 | 5.0 | smoke 通过 |

### 2.9 Row expander details visibility binding

本轮优化点：`DataGridRowExpander.NotifyLoadingRow()` 原先用 `BindUtils.RelayBind(this, IsCheckedProperty, row, DataGridRow.IsDetailsVisibleProperty, BindingMode.TwoWay)`。这会为每个 realized row expander 创建一个 `Binding`、字符串 `Path` 和完整 binding expression。Gallery `Expandable Row`、`Order Specific Column`、`Hidden Columns` 等详情列场景会在页面加载时创建 row expander；用户展开/折叠详情时还会反复走 `IsChecked` ↔ `IsDetailsVisible` 同步。

修复后：

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| Row expander loading | `BindUtils.RelayBind` TwoWay path binding | row `IsDetailsVisible` direct observable -> expander `IsChecked` | 构造路径更轻 |
| 用户点击 expander | Binding engine 写回 row | `OnPropertyChanged(IsChecked)` 写回 row `IsDetailsVisible` | 双向语义保留 |
| Row details programmatic change | Binding engine 更新 expander | direct observable 更新 expander | 双向语义保留 |
| Row unload / visual detach | 依赖 `UnloadingRow` | `UnloadingRow` + `OnDetachedFromVisualTree` 兜底释放 | 修复 detach-only 路径 stale row 引用风险 |

本轮还给性能工具补了 `DataGrid.RowDetails.Collapsed` 场景，用于覆盖关闭态详情列和 row expander binding setup。当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.RowDetails.Collapsed | 10.266 | 3046.7 | 320.0 | 1.0 | smoke 通过；本轮新增场景，无历史 timing 对比 |

### 2.10 Column header press/release/move handler cleanup

本轮优化点：`DataGridColumnHeader` 构造函数原先为每个 realized header 注册 `PointerPressed` / `PointerReleased` / `PointerMoved` 三个本地 routed handlers；`DataGridColumn.CreateHeader()` 还为 `HeaderPointerPressed` / `HeaderPointerReleased` 转发注册两个 per-header lambda。DataGrid 页面列声明多、列头频繁 realized，这类 handler 会进入每个控件实例的 `_eventHandlers` 字典。

修复后把内部 resize / reorder / sort 逻辑和 column public event 转发迁到 `DataGridColumnHeader` static constructor 的 class handlers：

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| Header press/move/release internal logic | 3 local handlers / realized header | 3 class handlers / control type | per-header `_eventHandlers` 记录移除 |
| `DataGridColumn.HeaderPointerPressed/Released` 转发 | 2 per-header lambda | 2 class handlers / control type | per-header lambda 移除 |
| Routed strategy | event adders 默认 Direct/Bubble | class handler 默认 Direct/Bubble | 与原 `PointerXxx +=` 路由策略一致 |
| `HeaderPointerPressed` 转发 | local lambda | class handler | 状态验证覆盖 exactly once 和 sender 保持为 `DataGridColumn` |

这轮属于结构/分配和生命周期清理，不把 `datagrid --count 100` timing smoke 当成确定加速。主收益是 standard column header 的本地 routed handlers 从 `5/header` 收敛到 `0/header`，并且验证 realized header 不再持有 `PointerPressed` / `PointerReleased` / `PointerMoved` 本地订阅。

同口径复测：baseline 使用临时 worktree `HEAD=ac3db9f2e`，optimized 使用当前工作区；两边均运行 `datagrid --count 100`。提升公式：`(baseline - optimized) / baseline`。

| Scenario | 指标 | baseline | optimized | 提升 |
| --- | --- | ---: | ---: | ---: |
| DataGrid.Basic | ms/item | 17.471 | 14.494 | 17.04% |
| DataGrid.Basic | KB/item | 2975.3 | 2969.5 | 0.19% |
| DataGrid.Filter.Menu.Closed | ms/item | 10.317 | 10.312 | 0.05% |
| DataGrid.Filter.Menu.Closed | KB/item | 2607.7 | 2604.8 | 0.11% |
| DataGrid.Filter.Tree.Closed | ms/item | 9.141 | 8.662 | 5.24% |
| DataGrid.Filter.Tree.Closed | KB/item | 2605.3 | 2600.8 | 0.17% |
| DataGrid.GalleryShape | ms/item | 44.224 | 43.936 | 0.65% |
| DataGrid.GalleryShape | KB/item | 12536.7 | 12512.4 | 0.19% |

`DataGrid.RowDetails.Collapsed` 是本轮新增 scenario，baseline 没有同名场景，不能给 timing 百分比；它对应的收益按 RowExpander details binding 结构项记录。

### 2.11 Column group header pointer forwarding cleanup

本轮优化点：`DataGridColumnGroupItem.CreateHeader()` 原先为每个 group header 注册两个本地 forwarding lambda：

- `PointerPressed -> HeaderPointerPressed`
- `PointerReleased -> HeaderPointerReleased`

Gallery `Grouping table head` 示例声明 4 个 `DataGridColumnGroupItem`，页面导航时会创建对应的 `DataGridColumnGroupHeader`。修复后由 `DataGridColumnGroupHeader` static constructor 注册 class handlers，再通过 `OwningGroupItem` 转发事件；路由策略与原 `PointerXxx +=` 保持一致，都是 Direct/Bubble。

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| Group header pointer forwarding | 2 per-header lambda | 2 class handlers / control type | per-header `_eventHandlers` 记录移除 |
| `HeaderPointerPressed` sender | `DataGridColumnGroupItem` | `DataGridColumnGroupItem` | 状态验证覆盖 exactly once 和 sender 保持 |
| Group header smoke scenario | n/a | `DataGrid.GroupHeaders` | 本轮新增，用于覆盖 group header 初始化路径 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.GroupHeaders | 9.618 | 2595.5 | 266.0 | 1.0 | smoke 通过；本轮新增场景，无历史 timing 对比 |

### 2.12 Row header pointer handler cleanup

本轮优化点：`DataGridRowHeader` 构造函数原先为每个 row header 调用 `AddHandler(PointerPressedEvent, HandlePointerPressed, handledEventsToo: true)`。Row header 是 `DataGridRowTheme` 的模板槽位，Gallery 的 `Row Header`、row details、特殊列等场景会随 realized rows 创建这些 header；即使后续通过 `HeadersVisibility` 控制可见性，本地 handler 仍已经进入每个 header 实例的 `_eventHandlers` 字典。

修复后把 press 逻辑迁到 `DataGridRowHeader` static constructor 的 class handler，并继续使用 `handledEventsToo: true`。保留原有行为：左键点击行头会 focus grid、更新 selection 和 `CurrentSlot`；右键路径仍调用原来的 grid 状态更新。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Row header `PointerPressed` 注册 | 1 local handler / row header | 1 class handler / control type | 100.00% per-row handler removed | 有效；per-instance `_eventHandlers` 记录移除 |
| `handledEventsToo` 语义 | true | true | preserved | 保留子元素已处理事件后的行头选择路径 |
| 左键点击行头 | focus + select row + update current slot | focus + select row + update current slot | 行为保留 | `--verify-datagrid-states` 覆盖 |
| Row header smoke scenario | n/a | `DataGrid.RowHeaders` | 新增覆盖 | 本轮新增，无历史 timing 百分比 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.RowHeaders | 9.838 | 3127.2 | 336.0 | 1.0 | smoke 通过；本轮新增场景，无历史 timing 对比 |

### 2.13 Row group header pointer handler cleanup

本轮优化点：`DataGridRowGroupHeader` 构造函数原先为每个 row group header 注册一个本地 `PointerPressed` lambda：

- `AddHandler(PointerPressedEvent, (s, e) => HandlePointerPressed(e), handledEventsToo: true)`

分组数据网格会按可视分组头创建 `DataGridRowGroupHeader`，并通过回收池复用；handler 注册发生在每个新建 group header 实例上。修复后由 `DataGridRowGroupHeader` static constructor 注册 class handler，保留 `handledEventsToo: true`，左键点击分组头仍更新 `CurrentSlot`。

新增验证同时打出了一个正确性问题：分组头模板内的 `DataGridRowHeader` 可能先完成自己的 `OnApplyTemplate()`，随后父 `DataGridRowGroupHeader` 才设置 `Owner`。原实现直接在 `OnApplyTemplate()` 中读取 `OwningGrid.BorderThickness`，会在这个顺序下空引用。修复后 `DataGridRowHeader.Owner` setter 和 `OnApplyTemplate()` 都走同一个 owner-dependent state 刷新路径；如果模板先到，等 `Owner` 设置后再补齐 separator / pseudo-class 状态。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Row group header `PointerPressed` 注册 | 1 local lambda / row group header | 1 class handler / control type | 100.00% per-header handler removed | 有效；per-instance `_eventHandlers` 记录移除 |
| `handledEventsToo` 语义 | true | true | preserved | 保留子元素已处理事件后的分组头状态更新路径 |
| 左键点击分组头 | update current slot | update current slot | 行为保留 | `--verify-datagrid-states` 覆盖 |
| Row header owner/template order | `OnApplyTemplate()` assumes owner ready | `Owner` setter retries owner-dependent state | null-ref path removed | 正确性修复 |
| Row group smoke scenario | n/a | `DataGrid.RowGroups` | 新增覆盖 | 本轮新增，无历史 timing 百分比 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.RowGroups | 13.517 | 3003.0 | 321.0 | 1.0 | smoke 通过；本轮新增场景，无历史 timing 对比 |

### 2.14 DataGrid core input handler cleanup

本轮优化点：`DataGrid` 构造函数原先为每个 grid 实例注册 4 个本地 routed event handlers：

- `KeyDown += HandleKeyDown`
- `KeyUp += HandleKeyUp`
- `GotFocus += HandleGotFocus`
- `LostFocus += HandleLostFocus`

真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，页面导航时会放大这类每实例 `_eventHandlers` 记录。Avalonia `InputElement` 已经通过 class handlers 把 `KeyDown` / `KeyUp` / `GotFocus` / `LostFocus` 分发到 virtual override；修复后改为 `OnKeyDown` / `OnKeyUp` / `OnGotFocus` / `OnLostFocus` 调用原处理函数，并保留 `!e.Handled` 口径。`DataGridCheckBoxColumn` 等按需附加到 grid 的功能订阅不属于本轮 core constructor cleanup，仍按对应功能生命周期管理。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Core input routed handler registration | 4 local handlers / grid | 0 local handlers / grid | 100.00% removed | 有效；`_eventHandlers` 记录移除 |
| `KeyDown` navigation | constructor instance handler | `OnKeyDown` override | behavior preserved | `Down` key 从 slot `0 -> 1` 覆盖 |
| `GotFocus` state | constructor instance handler | `OnGotFocus` override | behavior preserved | `ContainsFocus` 更新覆盖 |
| `KeyUp` / `LostFocus` dispatch | constructor instance handler | virtual override calls same handler body | handler body preserved | 本地 handler 移除覆盖；交互语义未改 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.101 | 2967.1 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.487 | 2601.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.805 | 2598.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 11.064 | 3125.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 12.351 | 3044.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.580 | 2594.6 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.986 | 3001.7 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.297 | 12490.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.15 Rows presenter scroll gesture handler cleanup

本轮优化点：`DataGridRowsPresenter` 构造函数原先为每个 presenter 注册一个本地 `ScrollGesture` handler：

- `AddHandler(InputElement.ScrollGestureEvent, OnScrollGesture)`

每个 `DataGrid` 模板会创建一个 `DataGridRowsPresenter`，真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，页面导航时会同步创建 rows presenter。修复后由 `DataGridRowsPresenter` static constructor 注册 class handler，继续调用原滚动逻辑 `OwningGrid.UpdateScroll(-e.Delta)`，不改变滚动手势方向和 handled 语义。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Rows presenter `ScrollGesture` 注册 | 1 local handler / rows presenter | 1 class handler / control type | 100.00% per-presenter handler removed | 有效；`_eventHandlers` 记录移除 |
| Scroll gesture handled 状态 | handled when grid scrolls | handled when grid scrolls | preserved | `--verify-datagrid-states` 覆盖 |
| Scroll gesture vertical offset | updates `VerticalOffset` | updates `VerticalOffset` | 行为保留 | 40-row grid 触发滚动覆盖 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.096 | 2966.6 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.287 | 2602.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.460 | 2598.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.872 | 3124.8 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.518 | 3044.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.734 | 2593.5 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.038 | 3001.2 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 45.845 | 12488.7 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.16 Rows presenter clip geometry allocation

本轮优化点：`DataGridRowsPresenter.ArrangeOverride()` 原先每次布局都会创建新的 `RectangleGeometry` 并赋给 `Clip`。每个 `DataGrid` 模板会创建一个 rows presenter，真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`；页面导航、窗口尺寸变化、滚动带来的布局 pass 都会触发 rows presenter arrange。

修复后 presenter 持有一个 `_clipGeometry`，首次 arrange 创建，后续只更新 `Rect`。由于换新 `Clip` 对象会触发 `Visual.ClipProperty` 的 `AffectsRender`，但复用同一个 geometry 后不应依赖 property 引用变化，本轮在 `Rect` 变化时显式 `InvalidateVisual()`，保持裁剪刷新语义。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Rows presenter clip allocation / repeated arrange | 1 new `RectangleGeometry` / arrange | 0 new `RectangleGeometry` / arrange after first | 100.00% repeated allocation removed | 有效；每 presenter 生命周期内从 `N` 次分配降到 `1` 次 |
| 65-grid Gallery shape estimated clip allocation / repeated arrange pass | 65 new `RectangleGeometry` / pass | 0 new `RectangleGeometry` / pass after first | 100.00% repeated allocation removed | 结构收益；按真实 `DataGridShowCase` 声明数量估算 |
| Clip geometry instance | replaced every arrange | reused across repeated arrange and resize | reuse verified | `--verify-datagrid-states` 覆盖 |
| Clip geometry rect | implicit by new geometry | reused geometry updates `Rect` | 行为保留 | 尺寸变化后 `Rect` 更新已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.596 | 2966.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.496 | 2601.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.789 | 2598.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.236 | 3124.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.969 | 3043.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.684 | 2592.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.989 | 3000.6 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.319 | 12486.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.17 Row bottom grid-line clip geometry allocation

本轮优化点：`DataGridRow.ArrangeOverride()` 原先只要模板存在 `PART_BottomGridLine`，每个 realized row 的每次 arrange 都会创建新的 `RectangleGeometry`：

- `new RectangleGeometry()`
- `gridlineClipGeometry.Rect = new Rect(...)`
- `_bottomGridLine.Clip = gridlineClipGeometry`

真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，导航时会同步 realize 多个 row；滚动、水平偏移变化、尺寸变化和重排都会触发 row arrange。修复后每个 row 缓存 `_bottomGridLineClipGeometry`，重复 arrange 只更新 `Rect`，并在 `OnApplyTemplate()` / `ResetGridLine()` 清掉旧引用，避免跨模板持有旧 visual 状态。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Row bottom grid-line clip allocation / repeated arrange | 1 new `RectangleGeometry` / realized row arrange | 0 new `RectangleGeometry` / realized row arrange after first | 100.00% repeated allocation removed | 有效；每 row/template 生命周期内从 `N` 次分配降到 `1` 次 |
| 8-row basic grid repeated arrange pass | 8 new `RectangleGeometry` / pass | 0 new `RectangleGeometry` / pass after first | 100.00% repeated allocation removed | 结构收益；按 `DataGrid.Basic` 默认 row 数估算 |
| Clip geometry instance | replaced every arrange | reused across repeated arrange and horizontal offset change | reuse verified | `--verify-datagrid-states` 覆盖 |
| Clip geometry rect | implicit by new geometry | reused geometry updates `Rect` | 行为保留 | 水平偏移变化后 `Rect` 更新已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.237 | 2966.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.223 | 2600.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.460 | 2597.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.171 | 3124.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 12.117 | 3043.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.429 | 2592.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.279 | 3000.1 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.514 | 12486.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

---

## 3. 验证

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

结果：构建通过。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-datagrid-states
```

结果：`DataGrid state verification passed.` 覆盖：无 filter items 不创建 shell、关闭态 filter content lazy、运行时 add/clear filter items 后 indicator 可见性和 flyout shell 同步、realized column header 不再注册本地 hover / press / release / move routed handlers、`HeaderPointerPressed` class handler 转发 exactly once 且 sender 保持为 `DataGridColumn`、realized column group header 不再注册本地 press / release forwarding handlers、group `HeaderPointerPressed` class handler 转发 exactly once 且 sender 保持为 `DataGridColumnGroupItem`、realized row header 不再注册本地 press handler 且左键点击仍选中行并更新 `CurrentSlot`、realized row group header 不再注册本地 press handler 且左键点击仍更新 `CurrentSlot`、realized DataGrid core 不再注册本地 `KeyDown` / `KeyUp` / `GotFocus` / `LostFocus` handlers，且 `GotFocus` 更新 `ContainsFocus`、`Down` key 仍推动 current slot、realized rows presenter 不再注册本地 `ScrollGesture` handler，且滚动手势仍 handled 并更新 `VerticalOffset`、rows presenter clip `RectangleGeometry` 跨重复 arrange 和尺寸变化复用且 `Rect` 会更新、row bottom grid-line clip `RectangleGeometry` 跨重复 arrange 和水平偏移变化复用且 `Rect` 会更新、分组行头内 row header 的 owner/template 顺序不再空引用、DataGridCell 跟随 header sort / reorder state 且 detach 后释放订阅、special columns 在 `Columns.Clear()` 后释放 cached owning grid、column reorder drag indicator 复用 cached render pen 且 foreground 变化后重建、DetailsPresenter `ContentHeight` 改变后触发 measure invalidation、RowExpander checked/details visibility 双向同步和 detach 释放。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 100
```

关键结果：

| Scenario | ms/item | KB/item | Visual/root | Logical/root |
| --- | ---: | ---: | ---: | ---: |
| DataGrid.Basic | 14.237 | 2966.0 | 305.0 | 1.0 |
| DataGrid.Filter.Menu.Closed | 9.223 | 2600.0 | 267.0 | 1.0 |
| DataGrid.Filter.Tree.Closed | 8.460 | 2597.4 | 267.0 | 1.0 |
| DataGrid.RowHeaders | 10.171 | 3124.3 | 336.0 | 1.0 |
| DataGrid.RowDetails.Collapsed | 12.117 | 3043.3 | 320.0 | 1.0 |
| DataGrid.GroupHeaders | 9.429 | 2592.3 | 266.0 | 1.0 |
| DataGrid.RowGroups | 10.279 | 3000.1 | 321.0 | 1.0 |
| DataGrid.GalleryShape | 43.514 | 12486.5 | 1260.0 | 5.0 |
