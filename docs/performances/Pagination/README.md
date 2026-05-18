# Pagination 性能优化

`Pagination` 是导航类高频控件，真实 Gallery 页面 `PaginationShowCase` 同时包含常规分页、size changer、quick jumper、total info 和 `SimplePagination` 示例。本轮优化目标是让未使用的图标、文本转换、quick jumper、size changer 等能力不承担默认成本，同时保证 API、视觉状态、动画和交互行为不变。

本轮已完成 Phase 0-7。

## 主要结论

- 页码项不再固定创建隐藏 `IconPresenter` 和 `ContentPresenter`；页码只创建轻量 `TextBlock`，上一页/下一页/省略号才创建 `IconPresenter`。
- `SimplePagination` 只在可编辑模式创建 `QuickJumpEdit`，只读模式不再承担 `LineEdit` 成本。
- `Pagination` 的 size changer 与 quick jumper 在关闭状态会释放对应控件、事件和 binding。
- `PaginationNav` 用 routed click handler 替代每个容器的 lambda 订阅，减少容器级订阅成本。
- `QuickJumperBar` 在 re-template/detach 时释放 `LineEdit.KeyUp` 订阅。
- `SimplePagination` next item 的 `PaginationItemType` 修正为 `Next`，这是正确性修复。

## 优化前瓶颈

1. **页码项隐藏 visual 成本高**

   旧模板每个 `PaginationNavItem` 都固定创建 `IconPresenter` 和 `ContentPresenter`，再通过 selector 控制显示。真实 `PaginationShowCase` 有大量页码项，最终产生 `120` 个 `IconPresenter` 和 `189` 个 `ContentPresenter`，多数只是隐藏或转换成本。

2. **只读 `SimplePagination` 仍创建 quick jumper**

   `SimplePagination` 只读形态不需要输入框，但旧模板固定创建 `QuickJumpEdit`。真实 Gallery 中只读示例会提前创建 `LineEdit` 体系。

3. **可选功能关闭后没有完整释放**

   `SizeChanger`、`QuickJumperBar`、`QuickJumpEdit`、`QuickJumperBar.LineEdit` 都是可选或模板生命周期对象，旧代码缺少完整的 re-template/detach/状态切换释放验证。

4. **容器点击订阅按项创建**

   `PaginationNav` 为每个 `PaginationNavItem` 容器创建 lambda 订阅，容器重建时会带来额外分配和释放复杂度。

## 本轮实现

- `PaginationNavItemTheme.axaml` 模板保留 `PART_RootLayout` 和背景 `Border`，移除固定 `IconPresenter`、`ContentPresenter` 和 `StringToTextBlockConverter`。
- `PaginationNavItem` 根据 `PaginationItemType` 按需创建显示 slot：
  - `PageIndicator` 创建 `PART_ContentTextBlock`。
  - `Previous`、`Next`、`Ellipses` 创建 `IconPresenter`。
  - 类型切换、re-template、detach 时移除旧 visual、清空内容和 templated parent。
- `SimplePaginationTheme.axaml` 移除模板里的固定 `QuickJumpEdit`，保留原有 `PART_QuickJumper` 样式给代码创建的输入框复用。
- `SimplePagination` 在 `IsReadOnly=false` 时创建 `QuickJumpEdit`，在切回只读、re-template、detach 时释放事件、binding 和 visual parent。
- `Pagination` 在 `IsShowSizeChanger=false` / `IsShowQuickJumper=false` 时释放对应控件；re-template/detach 时释放 nav、事件和 binding。
- `PaginationNav` 使用 `AddHandler(PaginationNavItem.ClickEvent, ...)`，避免每个容器独立 lambda 订阅。
- `PaginationTheme.axaml` 和 `QuickJumperBarTheme.axaml` 用 `TextBlock` 替代简单字符串 `ContentPresenter` + converter。
- 增加 `--verify-pagination-states`，覆盖可选 visual 创建/释放、事件释放、next item 类型、page item slot、visual parent 清理。
- `GalleryPerformance` 增加真实 `PaginationShowCase` 路由，按 Gallery 实际 XAML 形态测量。

## 控件级结果

命令：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite pagination --count 100
```

| 场景 | 指标 | 优化前 | 优化后 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `Pagination.Basic.Total50` | ms/item | `4.782` | `3.870` | 快 `19.07%` |
| `Pagination.Basic.Total50` | KB/item | `1081.2` | `906.6` | 少 `16.15%` |
| `Pagination.Basic.Total50` | Visual/root | `57` | `45` | 少 `12` |
| `Pagination.Basic.Total50` | ContentPresenter/root | `10` | `2` | 少 `8` |
| `Pagination.Basic.Total50` | IconPresenter/root | `7` | `2` | 少 `5` |
| `Pagination.More.Total500` | ms/item | `6.859` | `5.407` | 快 `21.17%` |
| `Pagination.More.Total500` | KB/item | `1694.0` | `1412.2` | 少 `16.64%` |
| `Pagination.SizeChanger` | ms/item | `4.602` | `3.880` | 快 `15.69%` |
| `Pagination.QuickJumper` | ms/item | `5.657` | `5.202` | 快 `8.04%` |
| `Pagination.TotalInfo` | ms/item | `3.620` | `3.320` | 快 `8.29%` |
| `Pagination.Small` | ms/item | `2.698` | `2.263` | 快 `16.12%` |
| `SimplePagination.ReadOnly` | ms/item | `0.594` | `0.465` | 快 `21.72%` |
| `SimplePagination.ReadOnly` | KB/item | `322.8` | `269.4` | 少 `16.54%` |
| `SimplePagination.ReadOnly` | ContentPresenter/root | `2` | `0` | 少 `2` |
| `SimplePagination.Editable` | ms/item | `1.678` | `1.509` | 快 `10.07%` |
| `Pagination.GalleryShape.PaginationShowCase` | ms/item | `80.428` | `66.871` | 快 `16.86%` |
| `Pagination.GalleryShape.PaginationShowCase` | KB/item | `22452.7` | `19664.6` | 少 `12.42%` |
| `Pagination.GalleryShape.PaginationShowCase` | Visual/root | `1188` | `991` | 少 `197` |
| `Pagination.GalleryShape.PaginationShowCase` | ContentPresenter/root | `189` | `55` | 少 `134` |
| `Pagination.GalleryShape.PaginationShowCase` | IconPresenter/root | `120` | `51` | 少 `69` |

说明：`QuickJumper` 场景仍包含 `LineEdit` 和 `ComboBox` 固定成本，因此耗时提升小于结构下降幅度。本轮没有继续拆 `LineEdit` 或 `ComboBox`，避免为了小收益扩大复杂度。

## PaginationShowCase 加载时间

真实 Gallery 命令：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase pagination --warmup 3 --iterations 20 --cold-iterations 10 --timeout-ms 20000
```

测试口径：Debug headless，`cold-iterations=10` 独立进程样本，`warmup=3`，`iterations=20`。第一次 after 短样本出现 P95 噪声，因此按相同口径复跑；最终复跑没有可重复性能回退。

| 指标 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | `461.68ms` | `368.87ms` | 快 `20.10%` |
| Cold first navigation median | `449.79ms` | `361.87ms` | 快 `19.55%` |
| Cold first navigation P95 | `546.33ms` | `413.20ms` | 快 `24.37%` |
| Cold alloc mean | `25905.45KB` | `22827.62KB` | 少 `11.88%` |
| Repeated navigation mean | `168.92ms` | `131.76ms` | 快 `21.99%` |
| Repeated navigation median | `153.38ms` | `119.40ms` | 快 `22.15%` |
| Repeated navigation P95 | `237.16ms` | `197.44ms` | 快 `16.75%` |
| Repeated alloc mean | `24148.01KB` | `21327.05KB` | 少 `11.68%` |
| Runtime visuals | `1262` | `1065` | 少 `197` |
| Runtime IconPresenter | `120` | `51` | 少 `69` |
| Runtime LineEdit | `11` | `8` | 少 `3` |
| Runtime ComboBox | `7` | `7` | 不变 |
| Runtime AddOnDecoratedBox | `15` | `15` | 不变 |

直观看，`PaginationShowCase` 重复打开从约 `169ms` 降到约 `132ms`，一次打开大约少 `37ms`；冷启动均值从约 `462ms` 降到约 `369ms`，少约 `93ms`。

## 生命周期与正确性覆盖

- 只读 `SimplePagination` 不创建 `QuickJumpEdit`。
- `IsReadOnly=false` 创建 `QuickJumpEdit`，切回只读后移除，旧实例无 visual parent；再次切回可编辑能重新创建。
- `SimplePagination` 的 `PART_NextNavItem` 类型是 `Next`。
- `Pagination.IsShowSizeChanger` 和 `Pagination.IsShowQuickJumper` 关闭后释放 child、事件和 binding，旧 child 无 visual parent；重新开启能重新创建。
- 页码 `PaginationNavItem` 不创建 `IconPresenter`；上一页/下一页 item 创建 `IconPresenter` 且不创建文本 slot。
- `QuickJumperBar` detach 后释放内部 `_lineEdit` 引用和 `KeyUp` 订阅。

## Phase 记录

| Phase | 状态 | 内容 |
| --- | --- | --- |
| Phase 0: 基线与观测 | Done | 新增 `pagination` 控件 suite、真实 `PaginationShowCase` Gallery 路由和 baseline 数据 |
| Phase 1: `PaginationNavItem` 模板减重 | Done | 移除固定 `IconPresenter`、`ContentPresenter`、字符串 converter |
| Phase 2: 页码/icon slot 按需创建 | Done | 根据 item 类型创建对应 slot，类型切换和 detach 释放 |
| Phase 3: `SimplePagination` quick jumper 按需创建 | Done | 只在可编辑模式创建 `QuickJumpEdit`，只读模式释放 |
| Phase 4: 可选 feature 生命周期收敛 | Done | size changer、quick jumper、nav、LineEdit 事件和 binding 释放路径补齐 |
| Phase 5: 容器点击订阅收敛 | Done | 改用 routed event handler，减少每项 lambda 订阅 |
| Phase 6: 状态验证 | Done | 新增 `--verify-pagination-states` 覆盖创建/释放/visual parent/类型正确性 |
| Phase 7: 复测与文档 | Done | 记录控件级和真实 Gallery 前后对比，保留复现命令 |

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-pagination-states
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --framework net10.0 --no-restore
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase pagination --warmup 3 --iterations 20 --cold-iterations 10 --timeout-ms 20000
git diff --check
```

`AtomUI.GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 Pagination 改动无关。
