# DatePicker 性能优化

`DatePicker`/`RangeDatePicker` 是 Data Entry 下的高频输入控件，本轮优化边界是：关闭态不承担未打开 popup 的日历/时间面板成本，不改变 API、视觉、动画和交互语义；所有 lazy visual、binding、event handler 都必须有释放路径。

真实 Gallery 场景 `DatePickerShowCase.axaml` 当前包含：

- `15` 个 `DatePicker`
- `15` 个 `RangeDatePicker`
- `9` 个 `ShowCaseItem`

## 本轮执行

| Phase | 状态 | 结果 |
| --- | --- | --- |
| Phase 0 基线与观测 | Done | 新增 `tools/performances/AtomUI.Performance/Suites/DatePicker`，GalleryPerformance 支持 `--showcase datepicker` |
| Phase 1 Popup content 按需创建 | Done | `ControlTheme` 只保留轻量 `Popup` shell；`ArrowDecoratedBox`/`DualMonthArrowDecoratedBox` 和 presenter 首次打开前创建 |
| Phase 2 Window.Deactivated 订阅按打开态启用 | Done | 关闭态不订阅窗口事件；打开时订阅，关闭/detach/re-template 时释放 |
| Phase 3 默认 accessory 轻量化 | Done | 默认日历图标使用轻量 `IconPresenter`；clear、feedback、ContentRightAddOn 才创建 `PickerAccessoryHost` |
| Phase 4 Range popup 行为迁移 | Done | `DualMonthArrowDecoratedBox` 的 arrow、flip、range indicator offset 行为从 XAML 迁移到代码同步 |
| Phase 5 状态与泄露验证 | Done | 新增 `--verify-datepicker-states`，覆盖 closed、accessory、popup materialize/reuse/detach、window subscription |
| Phase 6 Gallery 真实场景验证 | Done | 用真实 `DatePickerShowCase` 跑导航稳定耗时和视觉树计数 |

## 控件级结果

基线来自 Phase 0；优化后为本轮 `--suite datepicker --count 30`。Debug/headless 下单控件 timing 会波动，主要看关闭态结构和分配。

| 场景 | 指标 | 优化前 | 优化后 | 变化 |
| --- | --- | ---: | ---: | ---: |
| `DatePicker.Default.Closed` | KB/item | `612.7KB` | `580.8KB` | `-31.9KB / -5.21%` |
| `DatePicker.Default.Closed` | Visual/root | `34` | `33` | `-1` |
| `RangeDatePicker.Default.Closed` | KB/item | `1039KB` | `997.2KB` | `-41.8KB / -4.02%` |
| `RangeDatePicker.Default.Closed` | Visual/root | `60` | `59` | `-1` |
| `DatePicker.Default.PresenterOnly` | Visual/root | `362` | `362` | 持平 |
| `RangeDatePicker.Default.PresenterOnly` | Visual/root | `671` | `671` | 持平 |

结论：本轮没有优化打开后的日历 presenter 自身，收益集中在关闭态默认路径。`PresenterOnly` 持平是预期结果。

## ShowCase 加载时间优化对比

Phase 0 baseline：真实 `DatePickerShowCase`，Debug/headless，1300x900。优化后长样本：`--warmup 6 --iterations 20`，用于降低短样本抖动。Cold 是单样本，波动较大，不作为主要优化结论。

| 指标 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation | `502.66ms` | `549.60ms` | `+46.94ms / +9.34%` |
| Repeated mean | `174.77ms` | `154.21ms` | `-20.56ms / -11.76%` |
| Repeated median | `163.08ms` | `153.16ms` | `-9.92ms / -6.08%` |
| Repeated P95 | `215.52ms` | `159.77ms` | `-55.75ms / -25.87%` |

短样本 `--warmup 2 --iterations 6` 在本机出现过 `188ms` 到 `240ms` 的 repeated mean 抖动；结构数据稳定一致：`PickerHost 30 -> 0`、visual `1570 -> 1540`、alloc 约下降 `1.17MB`。

## Gallery 结构变化

| 指标 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| Alloc mean | `30959.16KB` | `29762.89KB` | `-1196.27KB / -3.86%` |
| Visuals | `1570` | `1540` | `-30 / -1.91%` |
| PickerHost | `30` | `0` | removed |
| DatePickerPresenter | `0` | `0` | 持平，closed route 未创建 |
| RangePickerPresenter | `0` | `0` | 持平，closed route 未创建 |

## 结论

本轮符合“未使用功能不承担成本”的方向：未打开的 DatePicker 不再创建 popup content，不再提前持有 presenter，不再为默认日历图标创建复合 accessory host，也不再在关闭态订阅窗口 deactivated 事件。

但这不是 50% 级别的页面优化。`DatePickerShowCase` 剩余成本主要来自 `30` 个输入 shell：`AddOnDecoratedBox`、内部 `TextBox`、默认 icon/button 组合、`ShowCaseItem` 页面结构和布局稳定过程。继续压冷启动需要拆这些共享基础成本，或者再针对日历 presenter 首开路径单独做打开态专项。

## 验证命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-datepicker-states
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite datepicker --count 30
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase datepicker --warmup 6 --iterations 20 --timeout-ms 30000
```
