# Carousel 性能优化

`Carousel` 本轮已完成 Phase 0-5。优化原则仍然是：未启用的 nav/progress/transition 不承担 visual、animation、timer 或事件成本；所有按需创建的节点都必须有释放路径和状态验证。

本目录只保留长期可读的方案、关键基线、最终结果和复现命令。控件级/Gallery 的逐次 sample 原始输出不入库；需要时用下方命令重新生成到 `/tmp`。

## Phase 0 基线

测试环境：Debug，Avalonia headless，`2026-05-15`。

控件级独立 suite：`tools/performances/AtomUI.Performance --suite carousel --count 60`。

| 场景 | ms/item | KB/item | Visual/root | Carousel | Indicator | NavButton | ProgressBorder | PageTransition | Timer | 关键结论 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `Carousel.Basic4` | `4.026` | `532.9` | `37` | `1` | `4` | `2` | `4` | `1` | `0` | 默认 4 页 Carousel 固定创建 nav button、pagination、progress border 和 transition |
| `Carousel.NoPagination4` | `2.564` | `290.5` | `18` | `1` | `0` | `2` | `0` | `1` | `0` | 关闭 pagination 后仍有隐藏 nav button 和 transition |
| `Carousel.NavButtons4` | `5.573` | `657.6` | `45` | `1` | `4` | `2` | `4` | `1` | `0` | 显示 nav button 会额外带出 icon/path/icon presenter 成本 |
| `Carousel.Progress4` | `1.641` | `540.0` | `37` | `1` | `4` | `2` | `4` | `1` | `1` | progress 场景也会为每个 indicator 预建 progress border |
| `Carousel.GalleryShape.Batch7` | `17.122` | `4503.1` | `305` | `7` | `28` | `14` | `28` | `7` | `2` | 复刻 Gallery 7 个 Carousel 的组合形态 |

真实 Gallery：`tools/performances/AtomUI.GalleryPerformance --showcase carousel --iterations 30 --warmup 10`。

| 场景 | Mean | Median | P95 | Alloc mean | Visuals | Logical | Carousel | Indicator | NavButton | LayoutTransform | ProgressBorder | PageTransition | Timer | IndicatorAnimation |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | `164.35ms` | `164.35ms` | `164.35ms` | `6844.96KB` | `369` | `53` | `7` | `28` | `14` | `7` | `28` | `7` | `2` | `4` |
| Repeated navigation | `38.13ms` | `37.30ms` | `52.81ms` | `6113.86KB` | `369` | `53` | `7` | `28` | `14` | `7` | `28` | `7` | `2` | `4` |

## 已执行 Phase

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 1 | 修复正确性与生命周期：`EffectivePreviousButtonMargin`/`EffectiveNextButtonMargin` DirectProperty raise 错误、模板重建旧按钮事件清理、timer 停止路径 | Done |
| Phase 2 | `CarouselNavButton` 按 `IsShowNavButtons` 创建；关闭后解绑 click、移除 visual parent、清 templated parent 和字段 | Done |
| Phase 3 | pagination/progress 减重：默认水平 pagination 不再创建 `LayoutTransformControl`；progress border 只在 progress 且 selected indicator 时创建 | Done |
| Phase 4 | `PageTransition` 延迟到首次真实切页且 motion enabled 时创建；关闭 motion 清空 transition | Done |
| Phase 5 | 状态收敛与 cleanup：selection/pagination 同步、`VirtualizingCarouselPanel` transition cancel/detach 清理、Arrange offset equality guard | Done |

## 优化后结果

控件级 suite：

| 场景 | ms/item | KB/item | Visual/root | NavButton | LayoutTransform | ProgressBorder | PageTransition | Timer | IndicatorAnimation | 结构结论 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `Carousel.Basic4` | `2.452` | `392.2` | `30` | `0` | `0` | `0` | `0` | `0` | `0` | 默认隐藏 nav/progress/transition 已去除 |
| `Carousel.NoPagination4` | `1.566` | `206.1` | `14` | `0` | `0` | `0` | `0` | `0` | `0` | 关闭 pagination 后只保留 Carousel + 当前 page 的必要结构 |
| `Carousel.NavButtons4` | `4.261` | `586.9` | `40` | `2` | `0` | `0` | `0` | `0` | `0` | 显式开启 nav 时才创建按钮和 icon |
| `Carousel.Progress4` | `2.102` | `408.9` | `31` | `0` | `0` | `1` | `0` | `1` | `1` | progress 只给当前 selected indicator 付费 |
| `Carousel.GalleryShape.Batch7` | `16.510` | `3624.6` | `262` | `4` | `1` | `1` | `0` | `2` | `1` | 结构与 alloc 明显下降，批量耗时小幅改善 |

真实 Gallery 复测：

| 场景 | Before | After | 变化 | Alloc before | Alloc after | Alloc 降幅 | Visuals | 关键结构变化 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| Cold first navigation | `164.35ms` | `149.05ms` | 提升 `9.31%` | `6844.96KB` | `5793.33KB` | `15.36%` | `369 -> 326` | nav `14 -> 4`，layout transform `7 -> 1`，progress `28 -> 1`，transition `7 -> 0` |
| Repeated navigation mean | `38.13ms` | `38.01ms` | 基本持平，提升 `0.31%` | `6113.86KB` | `5213.10KB` | `14.73%` | `369 -> 326` | timer 保持 `2`，indicator animation `4 -> 1` |
| Repeated navigation median | `37.30ms` | `38.76ms` | 回退 `3.91%` | - | - | - | `369 -> 326` | 该场景 timing 对 headless 调度噪声敏感，结构与 alloc 收益稳定 |

## 验证

- `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj`
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj`
- `dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-carousel-states`
- `dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite carousel --count 60 --markdown /tmp/atomui-carousel-optimized.md`
- `dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj`
- `dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase carousel --iterations 30 --warmup 10 --timeout-ms 30000 --label carousel-optimized --markdown /tmp/atomui-carousel-showcase-optimized.md`

## 后续观察

- Gallery repeated mean 在最终复测中基本持平，说明这轮 Carousel 优化主要兑现为结构和内存下降；打开耗时受 headless 调度噪声影响较大。
- 如果后续继续压 Carousel，应优先评估 pagination indicator 虚拟化或更轻量 indicator 模板，但这会触及交互与视觉一致性，收益需要重新基线证明。
