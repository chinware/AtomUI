# Cascader 性能优化

`Cascader` 的主要成本不在默认单个关闭态控件，而在 filter、multiple、popup view、filter result list、checkbox/loading slot 这些可选能力提前创建。优化边界仍然是：不改变 API、功能、视觉状态和交互行为；所有按需创建的 visual、binding、event handler 都必须有释放路径。

## 本轮执行

| Phase | 状态 | 结果 |
| --- | --- | --- |
| Phase 0 基线与观测 | Done | 拆出 `tools/performances/AtomUI.Performance/Suites/Cascader`，补 `--verify-cascader-states` |
| Phase 1 closed popup content | Done | 关闭态 `Cascader` 不创建 `CascaderView`，首次打开时创建并在 detach/re-template 清理 |
| Phase 2 mode-specific input | Done | filter `SelectFilterTextBox`、multiple `SelectTagAwareTextBox` 按需创建 |
| Phase 3 CascaderView filter list | Done | `CascaderViewFilterList` 仅在 filtering 时创建，清空 filter 后解除事件和 visual parent |
| Phase 4 item optional slots | Done | `ToggleCheckbox` 与 `LoadingIconPresenter` 按需创建，保留 `ExpandIconPresenter` 在 XAML 以避免额外 host 成本 |
| Phase 5 snapshot/cache | Done | `Options` path 解析复用 snapshot，避免 repeated `ToList()` |
| Phase 6 cleanup/lifecycle | Done | 补充 popup、filter list、checkbox、loading、empty indicator 的释放验证 |
| Phase 7 Gallery verification | Done | 用真实 `CascaderShowCase.axaml` 跑导航稳定耗时 |

## 控件级结果

基线来自 Phase 0 的 `--suite cascader --count 30`；优化后为本轮最终 `--suite cascader --count 30`。

| 场景 | before ms/item | after ms/item | before KB/item | after KB/item | before visual | after visual |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| `Cascader.Single.Filter.Closed` | 4.37 | 2.95 | 658.6 | 309.5 | 41 | 20 |
| `Cascader.Multiple.Empty` | 3.42 | 2.07 | 375.5 | 311.6 | 25 | 20 |
| `Cascader.Multiple.Selected` | 4.77 | 2.62 | 542.1 | 553.1 | 36 | 36 |
| `CascaderView.Default.Direct` | 3.97 | 3.89 | 826.2 | 700.4 | 51 | 47 |
| `CascaderView.Checkable.Direct` | 5.53 | 4.90 | 1085.8 | 982.1 | 71 | 69 |

`Cascader.Default.Closed` 本来已经是 20 visuals，本轮没有为了追求数字继续改动这条路径。`DefaultExpanded` 的 visual/alloc 有下降，但 timing 在 Debug/headless 下波动较大，后续如继续压展开路径，应单独分析展开层级、item materialization 和 async loader 路径。

## Gallery 结果

真实 `CascaderShowCase` 源形态：23 个 `Cascader`，2 个 `LineEdit`，2 个 `SearchEdit`，20 个 `ShowCaseItem`。

| 指标 | Phase 0 baseline | after | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation | 622.86ms | 610.65ms | -1.96% |
| Repeated mean | 141.81ms | 133.27ms | -6.02% |
| Repeated P95 | 179.94ms | 167.32ms | -7.01% |
| Alloc mean | ~22.83MB | 21155.42KB | 下降约 7-10% |
| Visuals | 1250 | 1169 | -81 |
| Icon | 54 | 41 | -13 |
| IconPresenter | 42 | 42 | 持平 |

结论：本轮符合“未使用功能不承担成本”的预期，主要收益体现在 filter/multiple closed 场景、直接 `CascaderView` 和 Gallery visual/alloc 下降。Gallery navigation timing 是小幅收益，不是 50% 级别；剩余成本主要来自 23 个 `Cascader` 的主体 AddOnDecoratedBox、SelectHandle/Icon 组合、20 个 ShowCaseItem 页面结构，以及默认展开/过滤时的 level list materialization。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-cascader-states
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-select-states
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite cascader --count 30
dotnet run --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase cascader --iterations 20 --warmup 6 --timeout-ms 30000
```
