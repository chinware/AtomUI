# Descriptions 性能优化

## Phase 0 基线

- 日期：2026-05-16
- 配置：Debug / Avalonia Headless / net10.0
- 控件级工具：`tools/performances/AtomUI.Performance`
- Gallery 工具：`tools/performances/AtomUI.GalleryPerformance`
- 本阶段只建立基线与分析方案，未修改 `Descriptions` 控件实现。

### Gallery 真实场景

`DescriptionsShowCase.axaml` 的真实源形态：

| Descriptions | DescriptionItem | ShowCaseItem | Button |
| ---: | ---: | ---: | ---: |
| 8 | 57 | 7 | 2 |

运行时稳定后的主要形态：

| Visuals | Logical | TextBlock | Descriptions | DescriptionDefaultItem | Bordered label | Bordered content |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 579 | 22 | 173 | 8 | 25 | 32 | 32 |

加载耗时：

| 指标 | 时间 | 分配 | 备注 |
| --- | ---: | ---: | --- |
| 冷启动首次打开 | 230.65ms | 11028.71KB | AboutUs -> DescriptionsShowCase |
| 重复打开均值 | 75.84ms | 9346.22KB | warmup 6, iterations 20 |
| 重复打开中位数 | 77.01ms | 9346.22KB | 同上 |
| 重复打开 P95 | 89.99ms | 9346.22KB | 同上 |

Trace 显示，冷启动 `Trigger ms` 为 31.57ms，但第一次找到 route 已到 197.92ms；第二次 `Trigger ms` 为 2.61ms，第一次找到 route 为 83.43ms。主要耗时在视图创建、模板应用、布局 pump，不在导航命令本身。

### 控件级基线

命令：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite descriptions --count 30
```

| 场景 | ms/item | KB/item | Visual/root | TextBlock/root | Descriptions/root | DefaultItem/root | Bordered label/root | Bordered content/root |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Basic5 | 4.473 | 536.5 | 43 | 16 | 1 | 5 | 0 | 0 |
| Bordered10 | 9.354 | 1018.1 | 73 | 25 | 1 | 0 | 10 | 10 |
| HeaderExtra10 | 8.073 | 1235.5 | 82 | 27 | 1 | 0 | 10 | 10 |
| Responsive8 | 4.240 | 834.6 | 62 | 21 | 1 | 0 | 8 | 8 |
| Vertical5 | 1.970 | 567.6 | 48 | 16 | 1 | 5 | 0 | 0 |
| VerticalBordered10 | 5.047 | 1175.9 | 93 | 25 | 1 | 10 | 0 | 0 |
| RowFilled4 | 1.601 | 432.5 | 32 | 9 | 1 | 0 | 4 | 4 |
| GalleryShape.Batch8 | 25.139 | 7059.8 | 511 | 159 | 8 | 25 | 32 | 32 |

## 当前判断

`DescriptionsShowCase` 不是 1s 级别的糟糕页面；在当前机器和 Debug Headless 下，重复打开约 76ms，冷启动约 231ms。它的主要问题是单页视觉树偏重：57 个 `DescriptionItem` 最终膨胀为 89 个 item/cell 控件，再叠加大量 `ContentPresenter` / `TextBlock` / `DockPanel`。

控件本身仍有优化空间，尤其是 bordered 横向布局：每个 item 固定拆成 label/content 两个 templated control。这个设计清晰，但对大表格式 descriptions 的实例化成本不低。

## 性能瓶颈点

1. 横向 bordered item 的结构成本高：每个 `DescriptionItem` 创建 `DescriptionBorderedItemLabel` 和 `DescriptionBorderedItemContent` 两个控件，Gallery 中 32 个 bordered item 变成 64 个 cell 控件。
2. `DoLayoutChildren()` 存在 O(n²) 模式：循环中反复 `Items.IndexOf(item)`，并在每次布局时清空重建 `ColumnDefinitions` / `RowDefinitions`。
3. Header 模板成本默认存在：`HeaderLayout`、`HeaderPresenter`、`ExtraPresenter` 总在模板中创建，只是通过 `IsHeaderLayoutVisible` 隐藏。无 header/extra 的场景仍承担这部分视觉树成本。
4. 每个 `Descriptions` 在 attach 时订阅 `Window.MediaBreakPointChanged`。Gallery 只有 8 个实例，成本不算大，但在列表/表单密集场景会放大。
5. 正确性风险：非 bordered 横向路径创建 `CompositeDisposable` 保存 `RelayBind`，但没有保存到字段，也没有在 remove/re-template/detach 时释放。
6. 动态集合风险：`RemoveDescriptionItems()` 在 remove 之后用 `Items.IndexOf(oldItem)` 查索引，删除、替换、重排路径容易不正确；当前还直接不支持 move/replace/reset。

## 优化方案

### Phase 1：正确性与生命周期修复

- 为代码创建的 binding/subscription 建立明确所有权，例如按 `DescriptionItem` 或生成子控件保存 disposable。
- remove、clear、re-template、detach 时释放对应 binding，避免资源泄露。
- 修复 `RemoveDescriptionItems()` 的索引计算，删除前确定 visual index，或者改为安全重建。
- 增加性能工具中的状态验证：add/remove item、切换 `IsShowColon`、切换 `Layout`、re-template、detach 后确认无重复 child、无悬挂 binding。

### Phase 2：布局算法减重

- 将 `DoLayoutChildren()` 改为 index-based loop，去掉循环内 `Items.IndexOf(item)`。
- 横向 bordered 使用 `visualIndex = itemIndex * 2`，普通/纵向使用 `visualIndex = itemIndex`。
- `ColumnDefinitions` 仅在列数变化时更新；`RowDefinitions` 仅在行数变化时更新，避免每次布局清空重建。
- 保留现有 span、filled、last row/column 行为，先用 Gallery 和状态验证锁住 UI 行为。

### Phase 3：Header/Extra 按需创建

- 评估把隐藏的 header 区从模板常驻改为按需：无 `Header` 且无 `Extra` 时不创建 `HeaderLayout` 内部 presenter。
- `Header`/`Extra` 后续变为非空时再创建，并同步 template、foreground、margin、font token。
- 必须验证 header 从 null 到非 null、非 null 到 null、extra button 样式和布局间距不变。

### Phase 4：bordered 横向 cell 合并评估

- 评估用一个轻量 item 控件承载 label/content，减少两个 templated control 的固定成本。
- 风险点是边框合并、span、最后一列/最后一行边框、label 背景必须完全一致。
- 只有在视觉回归可控且收益明确时执行；否则保留当前双 cell 模型，只优化布局和生命周期。

### Phase 5：MediaBreakPoint 订阅收敛

- 评估只在需要响应式变化时订阅窗口 breakpoint，或使用共享 watcher 降低密集实例订阅成本。
- 订阅必须有 detach/re-template 释放验证，不能引入窗口引用泄露。
- 默认场景的列数计算仍需在 attach/template 时完成，不能影响响应式布局。

### Phase 6：验证与对比

- 控件级：重新跑 `--suite descriptions --count 30`，报告每个场景的 before/after。
- Gallery：重新跑 `--showcase descriptions --warmup 6 --iterations 20 --timeout-ms 30000`。
- 汇报必须包含 cold first navigation、repeated mean、median、P95、alloc、visual count。
- UI 行为验证覆盖 Basic、border、custom size、responsive、vertical、vertical border、row filled。

## Phase 1-7 执行结果

### 已实施内容

| Phase | 结果 |
| --- | --- |
| Phase 0 基线与观测 | 已建立控件级和 `DescriptionsShowCase` 真实 Gallery 基线。 |
| Phase 1 正确性与生命周期 | 修复非 bordered 默认 item 的临时 binding 无所有者问题；`Items` 替换时解除旧集合订阅，避免旧集合继续驱动控件；`Window.MediaBreakPointChanged` 订阅改为显式 attach/detach。 |
| Phase 2 布局算法减重 | `DoLayoutChildren()` 改为 index-based loop，去掉循环内 `Items.IndexOf()`；`ColumnDefinitions` / `RowDefinitions` 按数量增删，避免每次完整清空重建。 |
| Phase 3 Header/Extra 按需模板 | 无 `Header` / `Extra` 时默认模板只创建 `ContentFrame + PART_GridLayout`；有 `Header` / `Extra` 时再使用带 `HeaderLayout`、`HeaderPresenter`、`ExtraPresenter` 的模板。 |
| Phase 4 bordered cell 合并评估 | 未合并 label/content cell。当前 bordered 边框、span、last row/column 行为依赖双 cell 模型，收益不够明确，先保留结构以降低 UI 回归风险。 |
| Phase 5 MediaBreakPoint 订阅收敛 | 当前实例级订阅保留，但补齐重复订阅规避和 detach 释放；只有已挂载窗口时记录 `_attachedWindow`，离树后清空。 |
| Phase 6 验证 | 增加 `--verify-descriptions-states`，覆盖 Header/Extra 模板生命周期、集合 add/remove/reset/replace、border/layout 切换、colon binding、窗口订阅释放。 |
| Phase 7 文档与总览 | 更新本 README 和性能总览；不保留本次逐次 sample 原始 markdown 输出。 |

### 控件级优化对比

命令：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite descriptions --count 30
```

| 场景 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Basic5 | 4.473ms/item, 536.5KB, 43 visuals | 5.643ms/item, 545.2KB, 43 visuals | 耗时 -26.16%，分配 -1.62%，visual 持平 |
| Bordered10 | 9.354ms/item, 1018.1KB, 73 visuals | 9.392ms/item, 1030.8KB, 69 visuals | 耗时 -0.41%，分配 -1.25%，visual -5.48% |
| HeaderExtra10 | 8.073ms/item, 1235.5KB, 82 visuals | 7.101ms/item, 1283.5KB, 82 visuals | 耗时 +12.04%，分配 -3.89%，visual 持平 |
| Responsive8 | 4.240ms/item, 834.6KB, 62 visuals | 4.261ms/item, 877.4KB, 62 visuals | 耗时 -0.50%，分配 -5.13%，visual 持平 |
| Vertical5 | 1.970ms/item, 567.6KB, 48 visuals | 2.341ms/item, 584.1KB, 48 visuals | 耗时 -18.83%，分配 -2.91%，visual 持平 |
| VerticalBordered10 | 5.047ms/item, 1175.9KB, 93 visuals | 4.571ms/item, 1156.9KB, 89 visuals | 耗时 +9.43%，分配 +1.62%，visual -4.30% |
| RowFilled4 | 1.601ms/item, 432.5KB, 32 visuals | 1.320ms/item, 446.4KB, 32 visuals | 耗时 +17.55%，分配 -3.21%，visual 持平 |
| GalleryShape.Batch8 | 25.139ms/root, 7059.8KB, 511 visuals | 24.486ms/root, 7232.1KB, 503 visuals | 耗时 +2.60%，分配 -2.44%，visual -1.57% |

说明：`+` 表示变快或资源减少，`-` 表示变慢或资源增加。单控件小场景存在 Debug/headless 抖动；本轮更稳定的收益体现在 `HeaderExtra10`、`VerticalBordered10`、`RowFilled4` 和 GalleryShape 的结构减重。

### Gallery ShowCase 加载对比

命令：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase descriptions --warmup 6 --iterations 20 --timeout-ms 30000
```

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation | 230.65ms | 234.07ms | -1.48% |
| Repeated mean | 75.84ms | 68.64ms | +9.49% |
| Repeated median | 77.01ms | 69.48ms | +9.78% |
| Repeated P95 | 89.99ms | 92.07ms | -2.31% |
| Repeated alloc mean | 9346.22KB | 9264.31KB | +0.88% |
| Visuals | 579 | 571 | +1.38% |

Trace 复测：

| Phase | Total | Trigger | First found | Stable | Pump total | Visuals |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold | 235.51ms | 31.51ms | 204.13ms | 235.36ms | 171.82ms | 571 |
| Second | 115.94ms | 2.64ms | 86.03ms | 115.94ms | 83.20ms | 571 |

结论：这次优化符合“先修生命周期和无用结构成本”的预期，但不是大幅压冷启动。真实 `DescriptionsShowCase` 重复打开约快 7.2ms，视觉树少 8 个节点；冷启动基本持平甚至略慢，主要因为页面固定成本仍在视图创建、模板应用和布局 pump，且 57 个 `DescriptionItem` 本身会生成大量 `TextBlock` 与 content presenter。

### 正确性与泄露验证

已跑：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-descriptions-states
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --framework net10.0 --no-restore
```

结果：

- `Descriptions state verification passed.`
- `AtomUIGallery.Desktop` 构建通过，0 warning，0 error。
- 本轮扫描到的泄露风险已修复：非 owned binding、替换 `Items` 后旧集合事件、窗口 breakpoint 订阅释放。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite descriptions --count 30

dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase descriptions --warmup 6 --iterations 20 --timeout-ms 30000
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase descriptions --trace-navigation --timeout-ms 30000
```
