# Icon Phase 3 结果

Phase 3 聚焦 `AtomUI.Icons.AntDesign` 的静态几何元数据，并把同一策略同步到外部 `MaterialIconsPackages` 与 `IconParkIconsPackage`。本阶段不改现有 public API，不缓存 Control 实例。

## 改动范围

- `AntDesignGenerator` 在生成 `.g.cs` 图标类时直接写入 `ViewBox`、geometry bounds、zoom matrix。
- `AntDesignIcon` 优先读取生成类提供的 metadata，命中后不再运行时计算 bounds，也不需要 static dictionary。
- 外部自定义派生类或未生成 metadata 的派生类继续走原来的实例级 `_geometryBounds` fallback。
- generator 运行时会初始化 Avalonia Headless，以便生成期使用 Avalonia `StreamGeometry` 计算 bounds。
- `MaterialIconsPackages` 新增 `MaterialIcon` 基类，generated icon class 改为继承 `MaterialIcon`，由 generator 写入同样的 metadata override。
- `IconParkIconsPackage` 在既有 `IconParkIcon` 基类中加入 metadata fallback，generator 写入同样的 metadata override。
- 外部包 generator 与生成代码中的基础类型显式使用 `Avalonia.Application` / `Avalonia.Point`；其中 `IconParkIconsPackage` 已存在 `Application`、`Point` 这类 generated icon class 名称，必须避免名称遮蔽导致编译失败。

实现边界：

- `DrawingInstructions` 和 `ViewBox` 在 AntDesign 生成图标中是类型级固定数据，适合生成静态 metadata。
- 生成 metadata 是 value type literal，不持有控件实例、visual、binding、subscription 或 disposable 资源。
- 如果同一生成类型未来出现不同 `ViewBox`，会复用生成的 geometry bounds，并针对当前 `ViewBox` 重新计算 zoom matrix。
- Phase 3 曾评估运行时 `Type -> metadata` 字典缓存；最终移除该方案，避免每个实例保留字典命中相关字段。

外部包同步结果：

| 包 | 生成 metadata 的图标类 | 生成后构建 |
| --- | ---: | --- |
| `MaterialIconsPackages` | 10751 | `dotnet build Material.slnx -c Debug --no-restore` 通过 |
| `IconParkIconsPackage` | 2658 | `dotnet build IconPark.slnx -c Debug --no-restore` 通过，保留既有 generator unreachable-code warning |

## 控件级结果

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite icon --count 120 \
  --markdown /tmp/icon-micro-phase3-generated-metadata.md
```

| Scenario | Phase 2 ms/item | Phase 3 ms/item | Phase 2 KB/item | Phase 3 KB/item |
| --- | ---: | ---: | ---: | ---: |
| Icon.SearchOutlined.Direct | 0.114 | 0.126 | 31.9 | 30.5 |
| Icon.SearchOutlined.Presenter | 0.188 | 0.191 | 41.4 | 40.0 |
| Icon.SearchOutlined.Many10 | 1.796 | 1.776 | 330.9 | 317.0 |
| Icon.LoadingOutlined.Spin | 0.174 | 0.259 | 41.0 | 39.6 |
| Icon.TwoTone.Bulb | 0.139 | 0.195 | 31.8 | 30.1 |
| Icon.Provider.SearchOutlined | 0.136 | 0.195 | 33.2 | 31.8 |
| Icon.HiddenSlots.SelectDefault | 3.230 | 3.439 | 528.8 | 524.6 |
| Icon.HiddenSlots.MenuItemLeaf | 0.839 | 1.066 | 224.0 | 222.6 |

观察：

- allocation 在所有 micro 场景中都下降，符合 bounds / matrix 生成化预期。
- 时间在 micro 口径下仍有噪声；hidden slot 的主要成本仍是模板结构，不是 AntDesign metadata。
- `Many10` 更能体现同类图标重复实例化场景，`1.796ms/item -> 1.776ms/item`，allocation `330.9KB/item -> 317.0KB/item`。

## Gallery IconShowCase 结果

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase icon --label icon-phase3-generated-metadata \
  --iterations 10 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/icon-showcase-navigation-phase3-generated-metadata.md
```

| Set | Baseline mean ms | Phase 2 mean ms | Phase 3 mean ms | vs Baseline | vs Phase 2 | Baseline alloc KB | Phase 3 alloc KB | Alloc vs Baseline |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold first navigation | 756.28 | 641.22 | 679.11 | -10.2% | +5.9% | 66222.01 | 54496.70 | -17.7% |
| Repeated navigation | 193.31 | 151.15 | 149.08 | -22.9% | -1.4% | 57900.83 | 46322.72 | -20.0% |

运行时形态保持一致：

| Visuals | Logical | Icon | IconPresenter | PathIcon | IconGallery | IconInfoItem |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 3712 | 11 | 456 | 459 | 456 | 1 | 456 |

说明：

- Phase 3 后 `IconShowCase` repeated navigation 相比 Phase 2 小幅提升 `1.4%`，相比原始基线提升 `22.9%`。
- repeated allocation 相比 Phase 2 继续下降约 `1.5%`，相比原始基线下降 `20.0%`。
- 运行时字典方案本轮测得 repeated `152.88ms / 46360.01KB`；generator metadata 最终为 `149.08ms / 46322.72KB`。
- cold first navigation 单样本波动明显，本阶段仍以 repeated navigation 作为主要判断。

## LineEditShowCase 回归

复现命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase lineedit --label icon-phase3-generated-lineedit \
  --iterations 10 --warmup 10 --timeout-ms 30000 \
  --markdown /tmp/lineedit-showcase-navigation-icon-phase3-generated-metadata.md
```

结果：

| Set | Phase 1 mean ms | Phase 2 mean ms | Phase 3 mean ms | Phase 1 alloc KB | Phase 3 alloc KB | Visuals | Icon | IconPresenter | AddOnDecoratedBox |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Repeated navigation | 182.09 | 158.49 | 166.11 | 55070.89 | 54810.29 | 2358 | 80 | 34 | 93 |

判断：

- LineEditShowCase allocation 继续下降，但时间样本比 Phase 2 最快那轮慢。
- 结合多轮样本，当前只能判断为无结构性回归；LineEdit 真实场景仍主要受 AddOnDecoratedBox、模板与布局影响，Icon metadata cache 不是主导成本。

## 验证

```bash
dotnet build src/AtomUI.Icons.AntDesign.Generator/AtomUI.Icons.AntDesign.Generator.csproj -c Debug --no-restore
cd output/bin/Debug/net10.0 && dotnet AtomUI.Icons.AntDesign.Generator.dll
dotnet build src/AtomUI.Icons.AntDesign/AtomUI.Icons.AntDesign.csproj -c Debug --no-restore
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --no-restore
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --verify-antdesign-metadata
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --verify-accessories --verify-effective-brushes --verify-addon-states
```

外部包同步验证：

```bash
cd /Users/chinboy/Projects/dotnet/AtomUIV6/scripts
pwsh ./PublishToLocalSources.ps1 \
  -localSourcesDir /Users/chinboy/Data/nuget-repo \
  -buildType Debug

cd /Users/chinboy/Projects/dotnet/MaterialIconsPackages/output/bin/Debug/net10.0
dotnet AtomUI.Icons.Material.Generator.dll
cd /Users/chinboy/Projects/dotnet/MaterialIconsPackages
NUGET_PACKAGES=/tmp/atomui-material-local-verify-pkgs \
  dotnet restore Material.slnx --force-evaluate --no-cache
NUGET_PACKAGES=/tmp/atomui-material-local-verify-pkgs \
  dotnet build Material.slnx -c Debug --no-restore

cd /Users/chinboy/Projects/dotnet/IconParkIconsPackage/output/bin/Debug/net10.0
dotnet AtomUI.Icons.IconPark.Generator.dll
cd /Users/chinboy/Projects/dotnet/IconParkIconsPackage
NUGET_PACKAGES=/tmp/atomui-iconpark-local-verify-pkgs \
  dotnet restore IconPark.slnx --force-evaluate --no-cache
NUGET_PACKAGES=/tmp/atomui-iconpark-local-verify-pkgs \
  dotnet build IconPark.slnx -c Debug --no-restore
```

说明：

- `AtomUI.GalleryPerformance` 构建仍保留一个既有 DataGrid 未使用字段 warning：`DataGridColumn._clipboardContentBinding`。
- `--verify-antdesign-metadata` 全量比较了 843 个生成图标的 generated bounds / zoom matrix 与旧运行时计算结果。
- Material/IconPark 外部包本轮通过生成数量、generated override 扫描、临时 NuGet 缓存 restore/build、完整 solution build 和 `git diff --check` 验证。
- 本阶段没有新增事件订阅、binding 或 disposable；生成 metadata 不持有控件实例。

## 后续

- Phase 4：处理 Select/Menu/NavMenu/ToggleIconButton 等隐藏 icon slot 的按需创建。
- Phase 5 可继续评估生成代码移除不必要 using 和运行时 transform parse 路径。
