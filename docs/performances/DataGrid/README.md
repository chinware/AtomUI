# DataGrid 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Phase F / Tier 4
> 状态：T4.2 column filter flyout lifecycle partial done；DataGrid core / row / cell / virtualization 仍待后续专项。

---

## 0. 结论

本轮优化有效，但不是大幅减少 DataGrid 主视觉树的优化。收益集中在关闭态过滤列头：

- 无过滤项的列头不再创建空 filter flyout shell。
- 有过滤项的列头只保留轻量 flyout shell，菜单/树过滤项延迟到首次打开前创建。
- filter indicator detach 时释放 flyout shell，并补齐 `CollectionView.FilterDescriptions` 订阅切换/释放。
- 修复 `DataGridColumnHeader.IsMiddleVisible` 错误 raise 到 `IsLastVisibleProperty` 的正确性问题。

Avalonia 依据：

- `PopupFlyoutBase.ShowAtCore` 只有在 `Popup.Child == null` 时才调用 `CreatePresenter()`：`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/PopupFlyoutBase.cs:292`。
- `MenuFlyout.CreatePresenter()` 把 `Items` 交给 presenter：`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/MenuFlyout.cs:85-92`。
- `ItemsControl.Items` 变更会进入 logical children / item count 路径：`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsControl.cs:639-656`。

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

结果：`DataGrid state verification passed.`

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
