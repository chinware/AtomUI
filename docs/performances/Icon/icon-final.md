# Icon 最终结果

本文件汇总 Icon Phase 0 到 Phase 6 的最终结果。所有数据均为 Debug / `net10.0` 下的性能工具结果，Gallery 口径为 headless route navigation，不等同于真实 GPU 上屏耗时。

## 完成范围

- Phase 1：修正 `strokeIndex`，Pen / Transition 按需创建，收敛 `IconPresenter` 和 `IconTemplatePresenter` 生命周期。
- Phase 2：Render 热路径使用 `DrawingContext.PushTransform(...)`，不再每帧创建 `MatrixTransform`，不再修改共享 geometry transform。
- Phase 3：AntDesign / Material / IconPark generated icon 写入静态 bounds 与 zoom matrix。
- Phase 4：SelectHandle、MenuItem、NavMenu header、ToggleIconButton、Button 的隐藏 icon slot 按需创建。
- Phase 5：Provider cache 清理语义补齐，generated transform 改为 `Matrix` literal。
- Phase 6：补齐 micro、Gallery、外部包 restore/build 与文档。

未做事项：

- 没有缓存任何 `Icon` / `PathIcon` / `IconPresenter` / Button 等 Control 实例。
- 没有生成 enum -> factory switch。当前 provider micro 数据不是瓶颈，收益不支撑增加生成代码复杂度。
- 没有处理 Gallery 页面懒加载、route 容器、TabControl materialization 等上层问题。

## 控件级对比

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite icon --count 300 \
  --markdown /tmp/icon-phase6-final-micro.md
```

| Scenario | Baseline ms/item | Final ms/item | Timing | Baseline KB/item | Final KB/item | Allocation |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| `Icon.SearchOutlined.Direct` | 0.156 | 0.126 | -19.2% | 47.0 | 30.6 | -34.9% |
| `Icon.SearchOutlined.Presenter` | 0.251 | 0.184 | -26.7% | 65.5 | 40.3 | -38.5% |
| `Icon.SearchOutlined.Many10` | 2.031 | 1.537 | -24.3% | 476.3 | 314.7 | -33.9% |
| `Icon.Provider.SearchOutlined` | 0.182 | 0.065 | -64.3% | 48.0 | 30.9 | -35.6% |
| `Icon.HiddenSlots.SelectDefault` | 3.548 | 1.954 | -44.9% | 594.7 | 427.6 | -28.1% |

结构性收益：

| Scenario | Baseline | Final |
| --- | --- | --- |
| `Icon.SearchOutlined.Direct` | `Visual/root 2` | `Visual/root 1` |
| `Icon.SearchOutlined.Many10` | `Visual/root 21` | `Visual/root 11` |
| `Icon.HiddenSlots.SelectDefault` | `Icon/root 3`, `Button/root 1` | `Icon/root 1`, `Button/root 0` |
| `Icon.HiddenSlots.ButtonDefault` | hidden loading icon 常驻 | `Icon/root 0` |
| `Icon.HiddenSlots.ButtonLoading` | loading 时创建 | `Icon/root 1` |

结论：

- micro 口径符合优化预期，尤其是默认模板节点、初始化成本、hidden slot 的 allocation 都稳定下降。
- `Provider.SearchOutlined` 当前已经很轻，因此不继续做 enum switch。

## Gallery 对比

`IconShowCase` 复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase icon --warmup 3 --iterations 10 \
  --label icon-phase6-final \
  --markdown /tmp/gallery-icon-phase6-final.md
```

| Set | Baseline mean | Final mean | Timing | Baseline alloc | Final alloc | Allocation |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | 756.28ms | 604.13ms | -20.1% | 66222.01KB | 54484.45KB | -17.7% |
| Repeated navigation | 193.31ms | 173.36ms | -10.3% | 57900.83KB | 46331.65KB | -20.0% |

`IconShowCase` 运行时结构：

| Visuals | Icon | IconPresenter | PathIcon |
| ---: | ---: | ---: | ---: |
| 3712 | 456 | 459 | 456 |

解释：

- Phase 3 曾测得 repeated `149.08ms / 46322.72KB`，最终 repeated 为 `173.36ms / 46331.65KB`。
- allocation 相比 baseline 的 `20.0%` 下降是稳定收益。
- timing 在 headless route navigation 里波动较大，最终样本仍比 baseline 快 `10.3%`，但不应解读为每次都能稳定提升 20% 以上。

## Gallery 受影响场景

复现命令均使用 `--warmup 3 --iterations 10 --label icon-phase6-final`。

| Showcase | Cold mean | Repeated mean | Median | P95 | Alloc KB mean | Runtime structure |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| `IconShowCase` | 604.13ms | 173.36ms | 160.32ms | 222.80ms | 46331.65 | `Visuals=3712`, `Icon=456`, `IconPresenter=459` |
| `LineEditShowCase` | 658.89ms | 210.98ms | 201.86ms | 255.36ms | 52064.13 | `Visuals=2357`, `Icon=51`, `IconPresenter=32`, `LineEdit=82`, `AddOnDecoratedBox=93` |
| `ButtonShowCase` | 383.67ms | 116.28ms | 116.65ms | 131.10ms | 32179.26 | `Visuals=1128`, `Button=89`, `Icon=52`, `IconPresenter=92` |
| `SelectShowCase` | 506.45ms | 153.40ms | 157.61ms | 176.70ms | 28053.95 | `Visuals=1483`, `Select=33`, `Icon=46`, `IconPresenter=59`, `AddOnDecoratedBox=33` |
| `MenuShowCase` | 257.15ms | 101.67ms | 101.19ms | 147.83ms | 16713.95 | `Visuals=791`, `Menu=5`, `MenuItem=10`, `NavMenuHeader=36`, `Icon=34`, `IconPresenter=36` |

这些场景的最终数据主要用于确认真实 Gallery XAML 的运行时形态：Button 默认不再常驻 loading icon，Select 默认不再同时 materialize loading/search/clear，Menu leaf 不再创建 submenu arrow。

## 外部包状态

| 包 | 同步内容 | 生成数量 | 验证 |
| --- | --- | ---: | --- |
| `AtomUI.Icons.AntDesign` | generated metadata + matrix literal | 843 | AntDesign metadata verifier 通过 |
| `MaterialIconsPackages` | `MaterialIcon` fallback + generated metadata + matrix literal | 10751 | 临时 NuGet cache restore/build 通过 |
| `IconParkIconsPackage` | `IconParkIcon` fallback + generated metadata + matrix literal | 2658 | 临时 NuGet cache restore/build 通过 |

## 最终验证命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --no-restore
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --no-restore

dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-accessories --verify-effective-brushes --verify-addon-states \
  --verify-antdesign-metadata --verify-icon-hidden-slots --verify-icon-provider-cache

cd /Users/chinboy/Projects/dotnet/MaterialIconsPackages
NUGET_PACKAGES=/tmp/atomui-material-local-verify-pkgs \
  dotnet restore Material.slnx --force-evaluate --no-cache
NUGET_PACKAGES=/tmp/atomui-material-local-verify-pkgs \
  dotnet build Material.slnx -c Debug --no-restore

cd /Users/chinboy/Projects/dotnet/IconParkIconsPackage
NUGET_PACKAGES=/tmp/atomui-iconpark-local-verify-pkgs \
  dotnet restore IconPark.slnx --force-evaluate --no-cache
NUGET_PACKAGES=/tmp/atomui-iconpark-local-verify-pkgs \
  dotnet build IconPark.slnx -c Debug --no-restore
```

验证结果：

- `AtomUI.Performance` 构建通过。
- `AtomUI.GalleryPerformance` 构建通过，0 warning / 0 error。
- Icon / AddOn 相关专项验证通过。
- `MaterialIconsPackages` 与 `IconParkIconsPackage` 使用临时 NuGet cache restore/build 通过。

## 后续风险

- Gallery timing 的噪声仍明显，后续要看趋势应保留同一机器、同一配置、同一 warmup/iteration 口径。
- `IconShowCase` 仍一次 materialize 456 个 icon item，剩余成本更多来自 item/control/tree materialization，而不是单个 icon render。
- `Icon` 默认模板已减重，但 Button、Select、Menu 之外的控件仍可能存在隐藏 icon slot，需要在对应控件优化时继续按“未使用不付费”原则检查。

## Gallery Lazy Load Follow-up

2026-05-14 后续按 `MaterialIconsPackages` Gallery 的做法，将 AtomUI Gallery 的 `IconGallery` 改为首批加载加滚动增量加载：

- 初始加载 `96` 个 icon item。
- 滚动接近底部时每次追加 `96` 个 icon item。
- `IconShowCase` 移除外层 `ScrollViewer`，由 `IconGallery` 内部 `ScrollViewer` 提供真实懒加载视口。
- 增加 Gallery 内部 `SearchEdit`，过滤时重新建立匹配列表并只激活首批结果。

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase icon --warmup 3 --iterations 10 \
  --label icon-gallery-lazy-load \
  --markdown /tmp/icon-gallery-lazy-load-10.md
```

结果：

| Set | Before lazy load | After lazy load | Timing | Before alloc | After alloc | Allocation |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | 604.13ms | 277.73ms | -54.0% | 54484.45KB | 15769.03KB | -71.1% |
| Repeated navigation | 173.36ms | 76.67ms | -55.8% | 46331.65KB | 11771.88KB | -74.6% |

运行时结构：

| Metric | Before lazy load | After lazy load |
| --- | ---: | ---: |
| Visuals | 3712 | 863 |
| Icon | 456 | 97 |
| IconPresenter | 459 | 100 |
| IconInfoItem | 456 | 96 |

说明：

- 这是 Gallery 页面物化策略优化，不计入前面 Icon 控件架构优化收益。
- 懒加载后 `IconShowCase` 首屏打开成本主要来自 96 个 item、搜索框与容器；滚动后的增量成本应按滚动路径单独测量。
