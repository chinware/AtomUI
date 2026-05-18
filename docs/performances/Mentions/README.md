# Mentions 性能优化记录

## 目标

`Mentions` 是基于 `MentionTextArea`、`Popup`、`CandidateList` 的输入型控件。真实 `MentionsShowCase` 包含 `15` 个 `Mentions`，优化前每个控件在关闭态都会提前创建 popup 重内容，并且 `OptionsSource` 会在设置时立即复制成本地 `_items`。

本轮原则：

- 未打开 Popup 的 `Mentions` 不创建 `CandidateList`、`PopupFrame`、loading `Spin`。
- 未触发候选列表前不复制 `OptionsSource`，也不执行筛选。
- 首次打开时创建 popup content，关闭后复用；re-template/detach 时清理视觉父级、事件、binding、timer 和 open-state subscriptions。
- 不改变主题视觉、选项模板、loading 状态、打开关闭事件和键盘交互语义。

## Phase 0：基线与观测

新增：

- 控件级套件：`tools/performances/AtomUI.Performance/Suites/Mentions`
- 状态/生命周期验证：`--verify-mentions-states`
- Gallery 真实场景：`tools/performances/AtomUI.GalleryPerformance --showcase mentions`

优化前关键数据：

| 场景 | 指标 | 优化前 |
| --- | --- | ---: |
| `Mentions.Default.Closed` | ms/item | `4.424` |
| `Mentions.Default.Closed` | KB/item | `511.5` |
| `Mentions.OptionsSource.Closed` | ms/item | `4.021` |
| `Mentions.OptionsSource.Closed` | KB/item | `512.3` |
| `Mentions.GalleryShape` | ms/item | `31.300` |
| `Mentions.GalleryShape` | KB/item | `7800.6` |
| `MentionsShowCase` | runtime Mentions | `15` |
| `MentionsShowCase` | `_candidateList` fields | `15` |
| `MentionsShowCase` | cold mean | `212.53ms` |
| `MentionsShowCase` | repeated mean | `110.05ms` |

## Phase 1-7：优化内容

- `MentionsTheme.axaml` 只保留轻量 `Popup#PART_Popup` shell。
- `Mentions.EnsurePopupContent()` 首次打开时创建 `CandidateList`、`PopupFrame` 和 popup content panel。
- `Spin` loading indicator 只在 popup 已 materialized 且 `IsLoading=true` 时创建，loading 结束或 detach 时释放 binding 和 visual parent。
- `OptionsSource` 改为 dirty 标记，只有打开/刷新候选列表时才复制 `_items`。
- 默认 `Contains` filter 改为静态复用，避免每次 `RefreshView()` 重建。
- `Window.Deactivated` 订阅改为仅 dropdown 打开期间启用，关闭/detach 释放。
- `AsyncLoadDebounce` timer 有明确 `ClearDelayTimer()`，detach 时停止并解绑 Tick。
- `CandidateList` 事件仍由 `CandidateList` 属性统一接入/移除，lazy content 清理时解除事件和 ItemsSource。
- `MentionTextArea` trigger 扫描避免反复 `ch.ToString()` 后再 `Contains`。
- 修正 popup close 路径下 pseudo class 状态刷新，并避免旧的 `Debug.Assert + nullable guard` 写法。

## 最终结果

控件级结果：

| 场景 | 指标 | 优化前 | 优化后 | 提升 |
| --- | --- | ---: | ---: | ---: |
| `Mentions.Default.Closed` | ms/item | `4.424` | `2.781` | `37.14%` |
| `Mentions.Default.Closed` | KB/item | `511.5` | `460.7` | `9.93%` |
| `Mentions.OptionsSource.Closed` | ms/item | `4.021` | `3.239` | `19.45%` |
| `Mentions.OptionsSource.Closed` | KB/item | `512.3` | `460.8` | `10.05%` |
| `Mentions.GalleryShape` | ms/item | `31.300` | `25.130` | `19.71%` |
| `Mentions.GalleryShape` | KB/item | `7800.6` | `7041.9` | `9.73%` |

真实 Gallery `MentionsShowCase` 结果：

| 指标 | 优化前 | 优化后 | 提升 |
| --- | ---: | ---: | ---: |
| cold mean | `212.53ms` | `201.88ms` | `5.01%` |
| cold median | `213.28ms` | `202.37ms` | `5.12%` |
| cold P95 | `216.72ms` | `213.67ms` | `1.41%` |
| cold alloc mean | `11193.51KB` | `10250.71KB` | `8.42%` |
| repeated mean | `110.05ms` | `106.95ms` | `2.82%` |
| repeated median | `115.72ms` | `113.64ms` | `1.80%` |
| repeated P95 | `133.03ms` | `125.98ms` | `5.30%` |
| repeated alloc mean | `10115.79KB` | `9326.47KB` | `7.80%` |
| runtime `_candidateList` fields | `15` | `0` | removed |

采样策略：`--cold-iterations 5 --warmup 3 --iterations 10 --timeout-ms 15000`，Debug/headless/`1300x900`，真实 route 从 AboutUs 导航到 `MentionsShowCase` 并等待视觉树稳定。

结论：控件级关闭态收益明确，真实 Gallery 页面耗时小幅改善，分配下降更明显。页面耗时提升比例不大，主要因为 `MentionsShowCase` 只有 `15` 个控件，剩余成本更多来自 Gallery route、ShowCasePanel/ShowCaseItem、TextArea/AddOnDecoratedBox 基础成本和主题加载。

## 验证命令

```bash
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug -f net10.0 --no-restore
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-build -- --verify-mentions-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 --no-build -- --suite mentions --count 60
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-build -- --showcase mentions --trace-navigation --timeout-ms 15000
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 --no-build -- --showcase mentions --cold-iterations 5 --warmup 3 --iterations 10 --timeout-ms 15000
```

## 后续观察

- 若继续压低 Gallery 页面耗时，应优先拆 `MentionTextArea`、`TextArea`、`AddOnDecoratedBox` 和 `ShowCaseItem` 的固定成本。
- `OptionsSource` 当前仍保留 collection weak subscription；这是状态同步路径，不属于关闭态重 visual 成本。后续如做更激进的 detach 策略，需要单独验证 detached 后源集合变化再 attach 的行为。
