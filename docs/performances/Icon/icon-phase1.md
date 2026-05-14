# Icon Phase 1 结果

Phase 1 聚焦低风险修复，不改 public API，不缓存 Control 实例，不处理模板节点减重和隐藏 slot 按需创建。

## 改动范围

- 修正 `Icon.OnInitialized()` 中 `strokeIndex` 使用 `Fallback` 的正确性问题。
- 初始化阶段只为实际被 stroke instruction 使用的 brush 创建 `DrawPens`，避免普通 fill icon 默认创建 5 个 pen。
- brush transition 从固定 5 个收敛为按 drawing instruction 需要创建；primary brush 为兼容 IconPark 主题映射，同时保留 `StrokeBrush` 与 `FillBrush` transition。
- `IconPresenter` 避免 attach 阶段重复配置同一个 icon，detach 时释放 relay binding。
- `IconTemplatePresenter` 改为集中管理 relay binding，并在 template 替换 / detach 时释放 binding、解除 visual/logical parent。

## 控件级结果

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite icon --count 60 \
  --markdown /tmp/icon-micro-phase1-final.md
```

| Scenario | Baseline ms/item | Phase 1 ms/item | Time change | Baseline KB/item | Phase 1 KB/item | Alloc change |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Icon.SearchOutlined.Direct | 0.156 | 0.112 | -28.2% | 47.0 | 29.2 | -37.9% |
| Icon.SearchOutlined.Presenter | 0.251 | 0.179 | -28.7% | 65.5 | 43.4 | -33.7% |
| Icon.SearchOutlined.Many10 | 2.031 | 1.398 | -31.2% | 476.3 | 339.3 | -28.8% |
| Icon.LoadingOutlined.Spin | 0.247 | 0.145 | -41.3% | 58.0 | 40.4 | -30.3% |
| Icon.TwoTone.Bulb | 0.194 | 0.145 | -25.3% | 49.0 | 36.0 | -26.5% |
| Icon.Provider.SearchOutlined | 0.182 | 0.141 | -22.5% | 48.0 | 34.4 | -28.3% |
| Icon.HiddenSlots.SelectDefault | 3.548 | 3.833 | +8.0% | 594.7 | 531.2 | -10.7% |
| Icon.HiddenSlots.MenuItemLeaf | 1.313 | 1.238 | -5.7% | 239.4 | 225.8 | -5.7% |

观察：

- Icon 自身场景的分配下降比较稳定，主要来自少创建 pen 和 transition。
- `SelectDefault` 隐藏 slot 场景分配下降，但时间样本有波动且本轮略慢；这个场景的主要问题仍是模板默认创建多个隐藏 icon，不属于 Phase 1 能根治的部分。
- visual/logical 数量没有变化，符合本阶段预期；节点减重应放到后续模板阶段。

## Gallery IconShowCase 结果

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase icon --label icon-phase1-final \
  --iterations 10 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/icon-showcase-navigation-phase1-final.md
```

| Set | Baseline mean ms | Phase 1 mean ms | Time change | Baseline alloc KB | Phase 1 alloc KB | Alloc change |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | 756.28 | 715.61 | -5.4% | 66222.01 | 56293.60 | -15.0% |
| Repeated navigation | 193.31 | 160.81 | -16.8% | 57900.83 | 47871.49 | -17.3% |

运行时形态保持一致：

| Visuals | Logical | Icon | IconPresenter | PathIcon | IconGallery | IconInfoItem |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 3712 | 11 | 456 | 459 | 456 | 1 | 456 |

## LineEditShowCase 回归

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase lineedit --label icon-phase1-lineedit \
  --iterations 10 --warmup 10 --timeout-ms 30000 \
  --markdown /tmp/lineedit-showcase-navigation-icon-phase1.md
```

结果：

| Set | Mean ms | Alloc KB mean | Visuals | Icon | IconPresenter | AddOnDecoratedBox |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Repeated navigation | 182.09 | 55070.89 | 2358 | 80 | 34 | 93 |

判断：

- 与上一轮 LineEdit 优化后的 `183.98ms` 口径基本持平，未观察到明确回退。
- allocation 比之前观测值继续下降，符合 Icon 初始化成本收敛预期。

## 验证

```bash
dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Debug --no-restore
dotnet build src/AtomUI.Controls/AtomUI.Controls.csproj -c Debug --no-restore
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --no-restore
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --verify-accessories --verify-effective-brushes --verify-addon-states
```

说明：

- 曾并行构建两个 performance 工具时遇到同一 pdb 文件锁，顺序重跑通过；不是代码错误。
- `AtomUI.GalleryPerformance` 构建保留一个既有 DataGrid 未使用字段 warning。

## 后续

- Phase 2：处理 render 热路径，避免每次 render 创建 `MatrixTransform` 并修改共享 geometry。
- Phase 3：评估 AntDesign / Material / IconPark 的静态 bounds 或 matrix 元数据缓存。
- Phase 4：处理 Select/Menu/NavMenu/ToggleIconButton 等隐藏 icon slot 的按需创建。
