# ProgressBar 性能优化记录

## 范围

- 控件：`ProgressBar`、`StepsProgressBar`、`CircleProgress`、`DashboardProgress` 以及共享基类 `AbstractProgressBar`。
- Gallery：真实 `ProgressBarShowCase.axaml`，源码形态为 `ShowCaseItem=19`、`Button=3`。
- 工具：`tools/performances/AtomUI.Performance --suite progressbar`，`tools/performances/AtomUI.GalleryPerformance --showcase progressbar`。

## 主要瓶颈与风险

1. line/circle 模板默认为每个 ProgressBar 创建 success 和 exception 两套 `IconPresenter + PathIcon`。普通未完成、无错误、甚至 `IsProgressInfoVisible=false` 的控件也承担隐藏 icon 成本。
2. `ProgressBarShowCase` 实际运行包含 121 个进度控件，优化前 runtime 有 `242` 个 `IconPresenter`、`242` 个 `PathIcon`，绝大多数都处于未使用状态。
3. `Value` 原来参与 `AffectsMeasure`，进度动画和普通 value 更新会把本应 render-only 的变化放大到 layout 路径。
4. circle/dashboard render 热路径每次绘制创建 `Pen`，在大量 progress 同屏时增加分配和 GC 压力。
5. 非零 `Minimum` 的 percentage/success threshold 计算存在正确性风险，状态伪类也需要随 indeterminate/completed/label position 明确同步。
6. 懒创建 presenter 必须有 release path，不能因为性能优化留下 visual parent、templated parent 或 icon binding 泄露。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 建立 `progressbar` 控件级 suite、真实 `ProgressBarShowCase` route 和状态验证 | Done |
| Phase 1 | success/exception status icon presenter 按需创建，普通态不创建隐藏 icon | Done |
| Phase 2 | icon presenter release path 覆盖 visual parent、templated parent、Icon/brush binding 清理 | Done |
| Phase 3 | `Value` 从 measure invalidation 移到 render invalidation，百分比文本测量增加缓存 | Done |
| Phase 4 | circle/dashboard render 复用 `Pen`，减少热路径对象分配 | Done |
| Phase 5 | 修复 non-zero `Minimum`、success threshold、pseudo-class 和 size threshold 同步问题 | Done |
| Phase 6 | 补齐 state/lifecycle verification，覆盖 status toggle、re-template、range math | Done |
| Phase 7 | 控件级/Gallery 复测、文档和清理 | Done |

## 控件级结果

口径：Debug / Avalonia Headless / `--suite progressbar --count 80`。Before 为本轮 ProgressBar 修改前基线；After 为当前工作区最终复测结果。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `ProgressBar.Line.Basic` | `0.942 ms/item`, `203.0 KB/item`, `10 visuals` | `0.749 ms/item`, `92.4 KB/item`, `6 visuals` | 快 `20.49%`，内存少 `54.48%`，少 `4` visuals |
| `ProgressBar.Line.NoInfo` | `0.684 ms/item`, `165.2 KB/item`, `8 visuals` | `0.397 ms/item`, `60.2 KB/item`, `4 visuals` | 快 `41.96%`，内存少 `63.56%`，少 `4` visuals |
| `ProgressBar.Circle.Basic` | `0.498 ms/item`, `160.8 KB/item`, `10 visuals` | `0.308 ms/item`, `83.9 KB/item`, `6 visuals` | 快 `38.15%`，内存少 `47.82%`，少 `4` visuals |
| `ProgressBar.Dashboard.Basic` | `0.431 ms/item`, `160.8 KB/item`, `10 visuals` | `0.312 ms/item`, `84.2 KB/item`, `6 visuals` | 快 `27.61%`，内存少 `47.64%`，少 `4` visuals |
| `ProgressBar.GalleryShape.ProgressBarShowCase` | `85.807 ms/item`, `23374.5 KB/item`, `1223 visuals` | `46.269 ms/item`, `12875.2 KB/item`, `811 visuals` | 快 `46.08%`，内存少 `44.92%`，少 `412` visuals |

结构收益最核心：`ProgressBar.GalleryShape.ProgressBarShowCase` 的 `IconPresenter/PathIcon` 从 `242/242` 降到 `36/36`，少 `206` 组隐藏 icon，符合“不使用的功能不承担成本”。

## Gallery 加载对比

口径：Debug / Avalonia Headless / `1300x900` / `--cold-iterations 10 --warmup 6 --iterations 20 --timeout-ms 30000`。After 复测保留同口径多样本，cold 指标受进程调度波动影响更大；下表采用最终同口径复测数据。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `ProgressBarShowCase` cold first navigation mean | `497.08 ms` | `400.12 ms` | 快 `96.96 ms`，`19.51%` |
| `ProgressBarShowCase` cold first navigation median | `490.36 ms` | `375.78 ms` | 快 `114.58 ms`，`23.37%` |
| `ProgressBarShowCase` cold first navigation P95 | `541.72 ms` | `512.68 ms` | 快 `29.04 ms`，`5.36%` |
| `ProgressBarShowCase` cold alloc mean | `29562.99 KB` | `19521.72 KB` | 少 `10041.27 KB`，`33.97%` |
| `ProgressBarShowCase` repeated mean | `154.17 ms` | `111.26 ms` | 快 `42.91 ms`，`27.83%` |
| `ProgressBarShowCase` repeated median | `151.04 ms` | `104.92 ms` | 快 `46.12 ms`，`30.54%` |
| `ProgressBarShowCase` repeated P95 | `170.69 ms` | `145.26 ms` | 快 `25.43 ms`，`14.90%` |
| `ProgressBarShowCase` repeated alloc mean | `26877.00 KB` | `16951.20 KB` | 少 `9925.80 KB`，`36.93%` |
| Runtime visuals | `1389` | `977` | 少 `412`，`29.66%` |
| Runtime Icon/IconPresenter/PathIcon | `242 / 242 / 242` | `36 / 36 / 36` | 各少 `206`，`85.12%` |

用户可见效果：真实 `ProgressBarShowCase` repeated open 从约 `154ms` 降到约 `111ms`，少约 `43ms`；冷启动均值少约 `97ms`。剩余成本主要来自 121 个 label/text、19 个 ShowCaseItem 以及 Gallery 页面固定布局。

## 正确性与泄露验证

新增并通过：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  --framework net10.0 --no-build -- --verify-progressbar-states
```

覆盖内容：

- `IsIndeterminate`、completed、inner label position 伪类同步。
- 普通未完成 line/circle ProgressBar 不创建 success/exception icon presenter。
- exception 状态创建 exception icon，恢复 normal 后释放并清空 visual parent。
- completed 状态创建 success icon，退回 incomplete 后释放并清空 visual parent。
- re-template 不重复创建 status icon presenter。
- non-zero `Minimum` 的 percentage 计算正确。
- non-zero `Minimum` 的 inner label 位置按计算出的 percentage 排布，而不是直接使用原始 `Value`。

状态切换验证里显式关闭 `IsMotionEnabled`，避免 `Value` transition 尚未完成时把动画中间值误判成状态同步失败；这不改变控件运行时动画行为。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-progressbar-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite progressbar --count 80
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase progressbar --cold-iterations 10 --warmup 6 --iterations 20 --timeout-ms 30000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 ProgressBar 优化无关。
